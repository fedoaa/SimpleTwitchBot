using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using SimpleTwitchBot.Lib.Events;

namespace SimpleTwitchBot
{
    public class IrcClient
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly TcpClient _tcpClient;

        public string Username { get; protected set; }
        public List<string> JoinedChannels { get; protected set; }

        private StreamReader _inputStream;
        private StreamWriter _outputStream;

        public event EventHandler<OnIrcMessageArgs> OnMessage;
        public event EventHandler<OnPingArgs> OnPing;
        public event EventHandler<OnChannelJoinArgs> OnChannelJoin;
        public event EventHandler<OnChannelPartArgs> OnChannelPart;

        public IrcClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _tcpClient = new TcpClient();
            JoinedChannels = new List<string>();
        }

        public async Task ConnectAsync(string username, string password)
        {
            Username = username;

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
                CallOnMessage(message);
            }
        }

        private async Task<string> ReadMessageAsync()
        {
            return await _inputStream.ReadLineAsync();
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

        private void CallOnMessage(string message)
        {
            OnMessage?.Invoke(this, new OnIrcMessageArgs { Message = message });
        }

        public void JoinChannel(string channel)
        {
            SendIrcMessage($"JOIN #{channel}");
        }

        public void PartChannel(string channel)
        {
            SendIrcMessage($"PART #{channel}");
        }

        public void SendIrcMessage(string message)
        {
            _outputStream.WriteLine(message);
            _outputStream.Flush();
        }

        public void SendChatMessage(string channel, string message)
        {
            SendIrcMessage($":{Username}!{Username}@{Username}.tmi.twitch.tv PRIVMSG #{channel} : {message}");
        }
    }
}