using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib
{
    public class IrcClient : IDisposable
    {
        private readonly List<string> _joinedChannels = new List<string>();

        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public IList<string> JoinedChannels => _joinedChannels.AsReadOnly();
        public bool IsConnected { get; private set; } = false;

        private bool _disposed = false;
        private TcpClient _tcpClient;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;

        public event EventHandler Connected;
        public event EventHandler LoggedIn;
        public event EventHandler Disconnected;
        public event EventHandler<IrcMessageReceivedEventArgs> IrcMessageReceived;
        public event EventHandler<PingReceivedEventArgs> PingReceived;
        public event EventHandler<UserJoinedEventArgs> UserJoined;
        public event EventHandler<ChannelJoinedEventArgs> ChannelJoined;
        public event EventHandler<ChannelPartedEventArgs> ChannelParted;

        public IrcClient(string host, int port)
        {
            Hostname = host;
            Port = port;
        }

        public async Task ConnectAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username cannot be empty", nameof(username));
            }

            Username = username.ToLower();
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(Hostname, Port);

                NetworkStream networkStream = _tcpClient.GetStream();
                _inputStream = new StreamReader(networkStream);
                _outputStream = new StreamWriter(networkStream);

                OnConnected();

                _outputStream.WriteLine($"PASS {password}");
                _outputStream.WriteLine($"NICK {username}");
                _outputStream.WriteLine($"USER {username} 8 * :{username}");
                _outputStream.Flush();

                await StartListenAsync();
            }
            catch (IOException ex) when ((ex.InnerException as SocketException)?.SocketErrorCode == SocketError.OperationAborted)
            {
                Debug.WriteLine("IOException: Operation aborted");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                OnDisconnected();
            }
        }

        protected virtual void OnConnected()
        {
            IsConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public void Disconnect()
        {
            _tcpClient?.Close();
            _inputStream?.Close();
            _outputStream?.Close();
        }

        private async Task StartListenAsync()
        {
            while (_tcpClient.Connected)
            {
                string rawMessage = await _inputStream.ReadLineAsync();
                if (string.IsNullOrEmpty(rawMessage))
                {
                    break;
                }

                var ircMessage = new IrcMessage(rawMessage);
                switch (ircMessage.Command)
                {
                    case "001":
                        OnLoggedIn();
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

        protected virtual void OnUserJoined(string username, string channel)
        {
            UserJoined?.Invoke(this, new UserJoinedEventArgs(username, channel));
        }

        protected virtual void OnChannelParted(string channel)
        {
            _joinedChannels.Remove(channel);
            ChannelParted?.Invoke(this, new ChannelPartedEventArgs(channel));
        }

        protected virtual void OnPingReceived(string serverAddress)
        {
            PingReceived?.Invoke(this, new PingReceivedEventArgs(serverAddress));
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
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _tcpClient?.Dispose();
                _inputStream?.Dispose();
                _outputStream?.Dispose();
            }
            _disposed = true;
        }
    }
}