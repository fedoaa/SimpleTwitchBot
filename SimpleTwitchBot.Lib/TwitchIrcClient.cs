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
        }

        protected override void OnPingReceived(string serverAddress)
        {
            base.OnPingReceived(serverAddress);
            SendIrcMessage($"PONG {serverAddress}");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SendIrcMessage("CAP REQ :twitch.tv/tags");
            SendIrcMessage("CAP REQ :twitch.tv/commands");
            SendIrcMessage("CAP REQ :twitch.tv/membership");
        }

        protected override void OnIrcMessageReceived(IrcMessage message)
        {
            base.OnIrcMessageReceived(message);
            switch (message.Command)
            {
                case "PRIVMSG":
                    var chatMessage = new TwitchChatMessage(message);
                    OnChatMessageReceived(chatMessage);
                    break;
                case "WHISPER":
                    var whisperMessage = new TwitchWhisperMessage(message);
                    OnWhisperMessageReceived(whisperMessage);
                    break;
                case "ROOMSTATE":
                    var channelState = new TwitchChannelState(message);
                    OnChannelStateChanged(channelState);
                    break;
                case "USERSTATE":
                    var userState = new TwitchUserState(message);
                    OnUserStateReceived(userState);
                    break;
                case "USERNOTICE":
                    var subscription = new TwitchSubscription(message);
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