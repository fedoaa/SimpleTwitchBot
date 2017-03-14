using SimpleTwitchBot.Lib.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib
{
    public class IrcClient
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly TcpClient _tcpClient;

        public string Username { get; protected set; }
        public List<string> JoinedChannels { get; protected set; }
        public bool IsConnected { get; protected set; }

        private StreamReader _inputStream;
        private StreamWriter _outputStream;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler<OnIrcMessageReceivedArgs> OnIrcMessageReceived;
        public event EventHandler<OnPingArgs> OnPing;
        public event EventHandler<OnChannelJoinArgs> OnChannelJoin;
        public event EventHandler<OnChannelPartArgs> OnChannelPart;

        public IrcClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _tcpClient = new TcpClient();
            JoinedChannels = new List<string>();
            IsConnected = false;
        }

        public async Task ConnectAsync(string username, string password)
        {
            Username = username.ToLower();

            await _tcpClient.ConnectAsync(_ip, _port);

            NetworkStream stream = _tcpClient.GetStream();
            _inputStream = new StreamReader(stream);
            _outputStream = new StreamWriter(stream);

            _outputStream.WriteLine($"PASS {password}");
            _outputStream.WriteLine($"NICK {username}");
            _outputStream.WriteLine($"USER {username} 8 * :{username}");
            _outputStream.Flush();
            StartListen();
        }

        public void Disconnect()
        {
            _tcpClient.Client.Shutdown(SocketShutdown.Both);
        }

        private async void StartListen()
        {
            while (_tcpClient.Connected)
            {
                string message = await ReadMessageAsync();
                if (message == null)
                {
                    continue;
                }
                if (!IsConnected && message.Contains("001"))
                {
                    CallOnConnect();
                    continue;
                }
                if (message.Contains("JOIN #"))
                {
                    string channel = message.Split('#')[1];
                    CallOnChannelJoin(channel);
                    continue;
                }
                if (message.Contains("PART #"))
                {
                    string channel = message.Split('#')[1];
                    CallOnChannelPart(channel);
                    continue;
                }
                if (message.StartsWith("PING"))
                {
                    string serverAddress = message.Split(' ')[1];
                    CallOnPing(serverAddress);
                    continue;
                }
                CallOnIrcMessageReceived(message);
            }
            CallOnDisconnect();
        }

        private async Task<string> ReadMessageAsync()
        {
            return await _inputStream.ReadLineAsync();
        }

        private void CallOnConnect()
        {
            IsConnected = true;
            OnConnect?.Invoke(this, EventArgs.Empty);
        }

        private void CallOnChannelJoin(string channel)
        {
            JoinedChannels.Add(channel);
            OnChannelJoin?.Invoke(this, new OnChannelJoinArgs { Channel = channel });
        }

        private void CallOnChannelPart(string channel)
        {
            JoinedChannels.Remove(channel);
            OnChannelPart?.Invoke(this, new OnChannelPartArgs { Channel = channel });
        }

        private void CallOnPing(string serverAddress)
        {
            OnPing?.Invoke(this, new OnPingArgs { ServerAddress = serverAddress });
        }

        private void CallOnIrcMessageReceived(string message)
        {
            OnIrcMessageReceived?.Invoke(this, new OnIrcMessageReceivedArgs { Message = message });
        }

        private void CallOnDisconnect()
        {
            IsConnected = false;
            OnDisconnect?.Invoke(this, EventArgs.Empty);
        }

        public void JoinChannel(string channel)
        {
            channel = channel.ToLower();
            SendIrcMessage($"JOIN #{channel}");
        }

        public void PartChannel(string channel)
        {
            channel = channel.ToLower();
            SendIrcMessage($"PART #{channel}");
        }

        public void SendIrcMessage(string message)
        {
            _outputStream.WriteLine(message);
            _outputStream.Flush();
        }
    }
}