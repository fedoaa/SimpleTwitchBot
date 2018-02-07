using SimpleTwitchBot.Lib.Events.Network;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib.Network
{
    public class NetworkClient : INetworkClient
    {
        private bool _disposed = false;

        public string Hostname { get; protected set; }
        public int Port { get; protected set; }
        public bool IsConnected { get; protected set; }

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        protected TcpClient _tcpClient;
        protected StreamReader _inputStream;
        protected StreamWriter _outputStream;

        public NetworkClient(string host, int port)
        {
            Hostname = host;
            Port = port;
        }

        public async Task ConnectAsync()
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(Hostname, Port);

            NetworkStream networkStream = _tcpClient.GetStream();
            _inputStream = new StreamReader(networkStream);
            _outputStream = new StreamWriter(networkStream);

            OnConnected();

            var listeningThread = new Thread(StartListening);
            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        protected virtual void OnConnected()
        {
            IsConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private void StartListening()
        {
            try
            {
                while (_tcpClient.Connected)
                {
                    string rawMessage = _inputStream.ReadLine();
                    if (string.IsNullOrEmpty(rawMessage))
                    {
                        break;
                    }
                    OnMessageReceived(rawMessage);
                }
            }
            catch (IOException ex) when ((ex.InnerException as SocketException)?.SocketErrorCode == SocketError.Interrupted)
            {
                // Ignore. Operation was cancelled by user
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                OnDisconnected();
            }
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        protected virtual void OnDisconnected()
        {
            if (IsConnected)
            {
                IsConnected = false;
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public void WriteLine(string value)
        {
            _outputStream.WriteLine(value);
        }

        public void Flush()
        {
            _outputStream.Flush();
        }

        public void Disconnect()
        {
            _tcpClient?.Close();
            _inputStream?.Close();
            _outputStream?.Close();
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