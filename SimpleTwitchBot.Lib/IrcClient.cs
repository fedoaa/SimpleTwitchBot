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

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<IrcMessageReceivedEventArgs> IrcMessageReceived;
        public event EventHandler<PingReceivedEventArgs> PingReceived;
        public event EventHandler<UserJoinedEventArgs> UserJoined;
        public event EventHandler<ChannelJoinedEventArgs> ChannelJoined;
        public event EventHandler<ChannelPartedEventArgs> ChannelParted;

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

            await StartListen();
        }

        public void Disconnect()
        {
            _tcpClient.Client.Shutdown(SocketShutdown.Both);
        }

        private async Task StartListen()
        {
            while (_tcpClient.Connected)
            {
                string rawMessage = await _inputStream.ReadLineAsync();
                if (string.IsNullOrEmpty(rawMessage))
                {
                    continue;
                }

                var ircMessage = new IrcMessage(rawMessage);
                switch (ircMessage.Command)
                {
                    case "001":
                        OnConnected();
                        break;
                    case "JOIN":
                        string channel = ircMessage.Params[0];
                        string username = ircMessage.Prefix.Split('!', '@')[1];

                        if (Username.Equals(username))
                        {
                            OnChannelJoined(channel);
                        }
                        else
                        {
                            OnUserJoined(username, channel);
                        }
                        break;
                    case "PART":
                        OnChannelParted(channel: ircMessage.Params[0]);
                        break;
                    case "PING":
                        OnPingReceived(serverAddress: ircMessage.Params[0]);
                        break;
                    default:
                        OnIrcMessageReceived(ircMessage);
                        break;
                }
            }
            OnDisconnected();
        }

        protected virtual void OnConnected()
        {
            IsConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnChannelJoined(string channel)
        {
            _joinedChannels.Add(channel);
            ChannelJoined?.Invoke(this, new ChannelJoinedEventArgs { Channel = channel });
        }

        protected virtual void OnUserJoined(string username, string channel)
        {
            UserJoined?.Invoke(this, new UserJoinedEventArgs { Username = username, Channel = channel });
        }

        protected virtual void OnChannelParted(string channel)
        {
            _joinedChannels.Remove(channel);
            ChannelParted?.Invoke(this, new ChannelPartedEventArgs { Channel = channel });
        }

        protected virtual void OnPingReceived(string serverAddress)
        {
            PingReceived?.Invoke(this, new PingReceivedEventArgs { ServerAddress = serverAddress });
        }

        protected virtual void OnIrcMessageReceived(IrcMessage message)
        {
            IrcMessageReceived?.Invoke(this, new IrcMessageReceivedEventArgs { Message = message });
        }

        protected virtual void OnDisconnected()
        {
            IsConnected = false;
            _joinedChannels.Clear();
            Disconnected?.Invoke(this, EventArgs.Empty);
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