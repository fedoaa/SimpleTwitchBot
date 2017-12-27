using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib
{
    public class TwitchIrcClient : IrcClient
    {
        public event EventHandler<ChatMessageReceivedEventArgs> ChatMessageReceived;
        public event EventHandler<WhisperMessageReceivedEventArgs> WhisperMessageReceived;
        public event EventHandler<ChannelStateChangedEventArgs> ChannelStateChanged;
        public event EventHandler<UserStateReceivedEventArgs> UserStateReceived;
        public event EventHandler<UserSubscribedEventArgs> UserSubscribed;

        public TwitchIrcClient(string host, int port) : base(host, port)
        {
            PingReceived += Client_PingReceived;
            Connected += TwitchIrcClient_Connected;
            IrcMessageReceived += Client_IrcMessageReceived;
        }

        private void TwitchIrcClient_Connected(object sender, EventArgs e)
        {
            SendIrcMessage("CAP REQ :twitch.tv/tags");
            SendIrcMessage("CAP REQ :twitch.tv/commands");
            SendIrcMessage("CAP REQ :twitch.tv/membership");
        }

        private void Client_PingReceived(object sender, PingReceivedEventArgs e)
        {
            SendIrcMessage($"PONG {e.ServerAddress}");
        }

        private void Client_IrcMessageReceived(object sender, IrcMessageReceivedEventArgs e)
        {
            switch(e.Message.Command)
            {
                case "PRIVMSG":
                    var chatMessage = new TwitchChatMessage(e.Message);
                    OnChatMessageReceived(chatMessage);
                    break;
                case "WHISPER":
                    var whisperMessage = new TwitchWhisperMessage(e.Message);
                    OnWhisperMessageReceived(whisperMessage);
                    break;
                case "ROOMSTATE":
                    var channelState = new TwitchChannelState(e.Message);
                    OnChannelStateChanged(channelState);
                    break;
                case "USERSTATE":
                    var userState = new TwitchUserState(e.Message);
                    OnUserStateReceived(userState);
                    break;
                case "USERNOTICE":
                    var subscription = new TwitchSubscription(e.Message);
                    OnUserSubscribed(subscription);
                    break;
            }
        }

        protected virtual void OnChatMessageReceived(TwitchChatMessage message)
        {
            ChatMessageReceived?.Invoke(this, new ChatMessageReceivedEventArgs { Message = message });
        }

        protected virtual void OnWhisperMessageReceived(TwitchWhisperMessage message)
        {
            WhisperMessageReceived?.Invoke(this, new WhisperMessageReceivedEventArgs { Message = message });
        }

        protected virtual void OnChannelStateChanged(TwitchChannelState channelState)
        {
            ChannelStateChanged?.Invoke(this, new ChannelStateChangedEventArgs { ChannelState = channelState });
        }

        protected virtual void OnUserStateReceived(TwitchUserState userState)
        {
            UserStateReceived?.Invoke(this, new UserStateReceivedEventArgs { UserState = userState });
        }

        protected virtual void OnUserSubscribed(TwitchSubscription subscription)
        {
            UserSubscribed?.Invoke(this, new UserSubscribedEventArgs { Subscription = subscription });
        }

        public void SendWhisperMessage(string username, string message)
        {
            SendIrcMessage($"PRIVMSG #jtv :/w {username} {message}");
        }
    }
}