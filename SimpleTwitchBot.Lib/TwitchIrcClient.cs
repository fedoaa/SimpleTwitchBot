using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Models;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib
{
    public class TwitchIrcClient
    {
        private IrcClient _client;

        public string Username
        {
            get { return _client?.Username; }
        }

        public event EventHandler<OnIrcMessageArgs> OnIrcMessage;
        public event EventHandler<OnUserMessageArgs> OnUserMessage;

        public TwitchIrcClient(string ip, int port)
        {
            var client = new IrcClient(ip, port);
            client.OnPing += Client_OnPing;
            client.OnIrcMessage += Client_OnIrcMessage;

            _client = client;
        }

        private void Client_OnPing(object sender, OnPingArgs e)
        {
            _client.SendIrcMessage($"PONG {e.ServerAddress}");
        }

        private void Client_OnIrcMessage(object sender, OnIrcMessageArgs e)
        {
            OnIrcMessage?.Invoke(this, e);
        }

        public async Task ConnectAsync(string username, string password)
        {
            await _client.ConnectAsync(username, password);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void EnableMessageTags()
        {
            _client.SendIrcMessage("CAP REQ :twitch.tv/tags");
            _client.SendIrcMessage("CAP REQ :twitch.tv/commands");
        }

        public void JoinChannel(string channel)
        {
            _client.JoinChannel(channel);
        }

        public void PartChannel(string channel)
        {
            _client.PartChannel(channel);
        }

        public void SendChatMessage(string channel, string message)
        {
            _client.SendIrcMessage($":{Username}!{Username}@{Username}.tmi.twitch.tv PRIVMSG #{channel} : {message}");
        }

        private void CallOnUserMessage(string channel, TwitchUserMessage message)
        {
            OnUserMessage?.Invoke(this, new OnUserMessageArgs { Message = message });
        }
    }
}