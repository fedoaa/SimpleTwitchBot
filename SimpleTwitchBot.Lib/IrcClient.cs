using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib
{
    public class IrcClient: IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private readonly TcpClient _tcpClient;
        private readonly List<string> _joinedChannels;

        public string Username { get; private set; }
        public IList<string> JoinedChannels => _joinedChannels.AsReadOnly();
        public bool IsConnected { get; private set; }

        private bool _disposed = false;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler<OnIrcMessageReceivedArgs> OnIrcMessageReceived;
        public event EventHandler<OnPingArgs> OnPing;
        public event EventHandler<OnChannelJoinedArgs> OnChannelJoined;
        public event EventHandler<OnChannelPartedArgs> OnChannelParted;

        public IrcClient(string host, int port)
        {
            _host = host;
            _port = port;
            _tcpClient = new TcpClient();
            _joinedChannels = new List<string>();
            IsConnected = false;
        }

        public async Task ConnectAsync(string username, string password)
        {
            Username = username.ToLower();

            await _tcpClient.ConnectAsync(_host, _port);

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

                var ircMessage = IrcMessage.Parse(message);
                switch (ircMessage.Command)
                {
                    case "001":
                        CallOnConnect();
                        break;
                    case "JOIN":
                        CallOnChannelJoined(channel: ircMessage.Params[0]);
                        break;
                    case "PART":
                        CallOnChannelParted(channel: ircMessage.Params[0]);
                        break;
                    case "PING":
                        CallOnPing(serverAddress: ircMessage.Params[0]);
                        break;
                    default:
                        CallOnIrcMessageReceived(ircMessage);
                        break;
                }
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

        private void CallOnChannelJoined(string channel)
        {
            _joinedChannels.Add(channel);
            OnChannelJoined?.Invoke(this, new OnChannelJoinedArgs { Channel = channel });
        }

        private void CallOnChannelParted(string channel)
        {
            _joinedChannels.Remove(channel);
            OnChannelParted?.Invoke(this, new OnChannelPartedArgs { Channel = channel });
        }

        private void CallOnPing(string serverAddress)
        {
            OnPing?.Invoke(this, new OnPingArgs { ServerAddress = serverAddress });
        }

        private void CallOnIrcMessageReceived(IrcMessage message)
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
            SendIrcMessage($"JOIN {channel}");
        }

        public void PartChannel(string channel)
        {
            channel = channel.ToLower();
            SendIrcMessage($"PART {channel}");
        }

        public void SendIrcMessage(string message)
        {
            _outputStream.WriteLine(message);
            _outputStream.Flush();
        }

        public void SendChatMessage(string target, string message)
        {
            SendIrcMessage($"PRIVMSG {target} :{message}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _tcpClient.Dispose();
                _inputStream?.Dispose();
                _outputStream?.Dispose();
            }
            _disposed = true;
        }
    }
}