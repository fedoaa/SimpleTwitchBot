using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib
{
    public class TwitchIrcClient : IrcClient
    {
        public event EventHandler<OnChatMessageReceivedArgs> OnChatMessageReceived;
        public event EventHandler<OnWhisperMessageReceivedArgs> OnWhisperMessageReceived;

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
            switch(e.Message.Command)
            {
                case "PRIVMSG":
                    var chatMessage = new TwitchChatMessage(e.Message);
                    CallOnChatMessageReceived(chatMessage);
                    break;
                case "WHISPER":
                    var whisperMessage = new TwitchWhisperMessage(e.Message);
                    CallOnWhisperMessageReceived(whisperMessage);
                    break;
            }
        }

        private void CallOnChatMessageReceived(TwitchChatMessage message)
        {
            OnChatMessageReceived?.Invoke(this, new OnChatMessageReceivedArgs { Message = message });
        }

        private void CallOnWhisperMessageReceived(TwitchWhisperMessage message)
        {
            OnWhisperMessageReceived?.Invoke(this, new OnWhisperMessageReceivedArgs { Message = message });
        }

        public void SendChatMessage(string channel, string message)
        {
            SendIrcMessage($":{Username}!{Username}@{Username}.tmi.twitch.tv PRIVMSG {channel} :{message}");
        }
    }
}