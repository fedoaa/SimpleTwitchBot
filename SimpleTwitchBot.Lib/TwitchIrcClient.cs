using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib
{
    public class TwitchIrcClient : IrcClient
    {
        public event EventHandler<OnUserMessageReceivedArgs> OnUserMessageReceived;

        public TwitchIrcClient(string ip, int port) : base(ip, port)
        {
            OnPing += Client_OnPing;
            OnConnect += TwitchIrcClient_OnConnect;
            OnIrcMessageReceived += Client_OnIrcMessageReceived;
        }

        private void TwitchIrcClient_OnConnect(object sender, EventArgs e)
        {
            EnableMessageTags();
        }

        private void EnableMessageTags()
        {
            SendIrcMessage("CAP REQ :twitch.tv/tags");
            SendIrcMessage("CAP REQ :twitch.tv/commands");
        }

        private void Client_OnPing(object sender, OnPingArgs e)
        {
            SendIrcMessage($"PONG {e.ServerAddress}");
        }

        private void Client_OnIrcMessageReceived(object sender, OnIrcMessageReceivedArgs e)
        {
            
        }

        public void SendChatMessage(string channel, string message)
        {
            SendIrcMessage($":{Username}!{Username}@{Username}.tmi.twitch.tv PRIVMSG #{channel} : {message}");
        }

        private void CallOnUserMessageReceived(string channel, TwitchUserMessage message)
        {
            OnUserMessageReceived?.Invoke(this, new OnUserMessageReceivedArgs { Message = message });
        }
    }
}