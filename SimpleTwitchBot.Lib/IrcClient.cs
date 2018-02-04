using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Events.Network;
using SimpleTwitchBot.Lib.Models;
using SimpleTwitchBot.Lib.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib
{
    public class IrcClient : IDisposable
    {
        private readonly List<string> _joinedChannels = new List<string>();

        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string UserName { get; private set; }
        public IList<string> JoinedChannels => _joinedChannels.AsReadOnly();
        public bool IsConnected { get; private set; } = false;

        private bool _disposed = false;
        private ISimpleTcpClient _client;
        private string _password;

        public event EventHandler Connected;
        public event EventHandler<ChannelJoinedEventArgs> ChannelJoined;
        public event EventHandler<ChannelModeReceivedEventArgs> ChannelModeReceived;
        public event EventHandler<ChannelPartedEventArgs> ChannelParted;
        public event EventHandler Disconnected;
        public event EventHandler<IrcMessageReceivedEventArgs> IrcMessageReceived;
        public event EventHandler LoggedIn;
        public event EventHandler<PingReceivedEventArgs> PingReceived;
        public event EventHandler<UserJoinedEventArgs> UserJoined;
        public event EventHandler<UserPartedEventArgs> UserParted;

        public IrcClient(string host, int port) : this(new SimpleTcpClient())
        {
            Hostname = host;
            Port = port;
        }

        internal IrcClient(ISimpleTcpClient client)
        {
            _client = client;
            _client.Connected += Client_Connected;
            _client.Disconnected += Client_Disconnected;
            _client.MessageReceived += Client_MessageReceived;
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            OnConnected();

            _client.WriteLine($"PASS {_password}");
            _client.WriteLine($"NICK {UserName}");
            _client.WriteLine($"USER {UserName} 8 * :{UserName}");
            _client.Flush();
        }

        protected virtual void OnConnected()
        {
            IsConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            OnDisconnected();
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var ircMessage = new IrcMessage(e.Message);
            switch (ircMessage.Command)
            {
                case "001":
                    OnLoggedIn();
                    break;
                case "JOIN":
                    {
                        string userName = ircMessage.GetUserName();
                        string channel = ircMessage.GetChannel();

                        if (UserName.Equals(userName))
                        {
                            OnChannelJoined(channel);
                        }
                        else
                        {
                            OnUserJoined(userName, channel);
                        }
                        break;
                    }
                case "PART":
                    {
                        string userName = ircMessage.GetUserName();
                        string channel = ircMessage.GetChannel();

                        if (UserName.Equals(userName))
                        {
                            OnChannelParted(channel);
                        }
                        else
                        {
                            OnUserParted(userName, channel);
                        }
                        break;
                    }
                case "PING":
                    string serverAddress = ircMessage.GetParameterByIndex(0);
                    OnPingReceived(serverAddress);
                    break;
                case "MODE":
                    var channelMode = new ChannelMode(ircMessage);
                    OnChannelModeReceived(channelMode);
                    break;
                default:
                    OnIrcMessageReceived(ircMessage);
                    break;
            }
        }

        protected virtual void OnLoggedIn()
        {
            LoggedIn?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnChannelJoined(string channel)
        {
            _joinedChannels.Add(channel);
            ChannelJoined?.Invoke(this, new ChannelJoinedEventArgs(channel));
        }

        protected virtual void OnUserJoined(string userName, string channel)
        {
            UserJoined?.Invoke(this, new UserJoinedEventArgs(userName, channel));
        }

        protected virtual void OnChannelParted(string channel)
        {
            _joinedChannels.Remove(channel);
            ChannelParted?.Invoke(this, new ChannelPartedEventArgs(channel));
        }

        protected virtual void OnUserParted(string userName, string channel)
        {
            UserParted?.Invoke(this, new UserPartedEventArgs(userName, channel));
        }

        protected virtual void OnPingReceived(string serverAddress)
        {
            PingReceived?.Invoke(this, new PingReceivedEventArgs(serverAddress));
        }

        protected virtual void OnChannelModeReceived(ChannelMode channelMode)
        {
            ChannelModeReceived?.Invoke(this, new ChannelModeReceivedEventArgs(channelMode));
        }

        protected virtual void OnIrcMessageReceived(IrcMessage message)
        {
            IrcMessageReceived?.Invoke(this, new IrcMessageReceivedEventArgs(message));
        }

        protected virtual void OnDisconnected()
        {
            if (IsConnected)
            {
                IsConnected = false;
                _joinedChannels.Clear();
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task ConnectAsync(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("Username cannot be empty", nameof(userName));
            }

            UserName = userName.ToLower();
            _password = password;

            await _client.ConnectAsync(Hostname, Port);
        }

        public void Disconnect()
        {
            _client?.Disconnect();
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
            _client.WriteLine(message);
            _client.Flush();
        }

        public void SendChatMessage(string target, string message)
        {
            SendIrcMessage($"PRIVMSG {target} :{message}");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _client?.Dispose();
            }
            _disposed = true;
        }
    }
}