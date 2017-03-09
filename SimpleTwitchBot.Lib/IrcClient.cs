using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleTwitchBot
{
    public class IrcClient
    {
        private readonly TcpClient _tcpClient;

        private string _username;
        private string _channel;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;

        public IrcClient()
        {
            _tcpClient = new TcpClient();
        }

        public async Task ConnectAsync(string ip, int port, string username, string password)
        {
            _username = username;

            await _tcpClient.ConnectAsync(ip, port);
            _inputStream = new StreamReader(_tcpClient.GetStream());
            _outputStream = new StreamWriter(_tcpClient.GetStream());

            _outputStream.WriteLine("PASS " + password);
            _outputStream.WriteLine("NICK " + username);
            _outputStream.WriteLine($"USER {username} 8 * :{username}");
            _outputStream.Flush();
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
            SendIrcMessage(":" + _username + "!" + _username + "@" + _username + ".tmi.twitch.tv PRIVMSG #" + _channel + " :" + message);
        }

        public string ReadMessage()
        {
            string message = _inputStream.ReadLine();
            return message;
        }
    }
}