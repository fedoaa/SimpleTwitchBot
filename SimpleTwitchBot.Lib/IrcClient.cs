using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleTwitchBot
{
    public class IrcClient
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly TcpClient _tcpClient;

        public string Username { get; private set; }

        private string _channel;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;

        public event Action<IrcClient, string> OnMessage;

        public IrcClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _tcpClient = new TcpClient();
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
            _tcpClient?.Dispose();
        }

        private async void StartListen()
        {
            while(_tcpClient.Connected)
            {
                string message = await ReadMessageAsync();
                CallOnMessage(message);
            }
        }

        private void CallOnMessage(string message)
        {
            OnMessage?.Invoke(this, message);
        }

        public void JoinRoom(string channel)
        {
            _channel = channel;

            _outputStream.WriteLine("JOIN #" + channel);
            _outputStream.WriteLine("CAP REQ :twitch.tv/tags");
            _outputStream.WriteLine("CAP REQ :twitch.tv/commands");
            _outputStream.Flush();
        }

        public void SendIrcMessage(string message)
        {
            _outputStream.WriteLine(message);
            _outputStream.Flush();
        }

        public void SendChatMessage(string message)
        {
            SendIrcMessage($":{Username}!{Username}@{Username}.tmi.twitch.tv PRIVMSG #{_channel} : {message}");
        }

        public async Task<string> ReadMessageAsync()
        {
            string message = await _inputStream.ReadLineAsync();
            return message;
        }
    }
}