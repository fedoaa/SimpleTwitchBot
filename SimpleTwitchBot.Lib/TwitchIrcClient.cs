﻿using SimpleTwitchBot.Lib.Events;
using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib
{
    public class TwitchIrcClient : IrcClient
    {
        public event EventHandler<ChannelBeingHostedEventArgs> ChannelBeingHosted;
        public event EventHandler<ChannelHostingStartedEventArgs> ChannelHostingStarted;
        public event EventHandler<ChannelHostingStoppedEventArgs> ChannelHostingStopped;
        public event EventHandler<ChatClearedEventArgs> ChatCleared;
        public event EventHandler<ChatMessageReceivedEventArgs> ChatMessageReceived;
        public event EventHandler<ChannelStateChangedEventArgs> ChannelStateChanged;
        public event EventHandler<ChannelRaidedEventArgs> ChannelRaided;
        public event EventHandler<ChannelRitualPerformedEventArgs> ChannelRitualPerformed;
        public event EventHandler<GlobalUserStateReceivedEventArgs> GlobalUserStateReceived;
        public event EventHandler<ModeratorJoinedEventArgs> ModeratorJoined;
        public event EventHandler<ModeratorPartedEventArgs> ModeratorParted;
        public event EventHandler<SubscriptionGiftedEventArgs> SubscriptionGifted;
        public event EventHandler<UserBannedEventArgs> UserBanned;
        public event EventHandler<UserSubscribedEventArgs> UserResubscribed;
        public event EventHandler<UserStateReceivedEventArgs> UserStateReceived;
        public event EventHandler<UserSubscribedEventArgs> UserSubscribed;
        public event EventHandler<UserTimedOutEventArgs> UserTimedOut;
        public event EventHandler<WhisperMessageReceivedEventArgs> WhisperMessageReceived;

        public TwitchIrcClient(string host, int port) : base(host, port)
        {
        }

        protected override void OnPingReceived(string serverAddress)
        {
            SendIrcMessage($"PONG {serverAddress}");
            base.OnPingReceived(serverAddress);
        }

        protected override void OnConnected()
        {
            SendIrcMessage("CAP REQ :twitch.tv/tags twitch.tv/commands twitch.tv/membership");
            base.OnConnected();
        }

        protected override void OnChannelModeReceived(ChannelMode channelMode)
        {
            if (channelMode.Modes.Contains("o"))
            {
                switch (channelMode.Action)
                {
                    case ChannelModeActionType.Add:
                        OnModeratorJoined(channelMode.ModeParams, channelMode.Channel);
                        break;
                    case ChannelModeActionType.Remove:
                        OnModeratorParted(channelMode.ModeParams, channelMode.Channel);
                        break;
                }
            }
            base.OnChannelModeReceived(channelMode);
        }

        protected virtual void OnModeratorJoined(string userName, string channel)
        {
            ModeratorJoined?.Invoke(this, new ModeratorJoinedEventArgs(userName, channel));
        }

        protected virtual void OnModeratorParted(string userName, string channel)
        {
            ModeratorParted?.Invoke(this, new ModeratorPartedEventArgs(userName, channel));
        }

        protected override void OnUnprocessedIrcMessageReceived(IrcMessage message)
        {
            switch (message.Command)
            {
                case "GLOBALUSERSTATE":
                    var globalUserState = new TwitchGlobalUserState(message);
                    OnGlobalUserStateReceived(globalUserState);
                    break;
                case "PRIVMSG":
                    if (message.GetUserName().Equals("jtv"))
                    {
                        var hostedChannel = new TwitchHostedChannel(message);
                        OnChannelBeingHosted(hostedChannel);
                    }
                    else
                    {
                        var chatMessage = new TwitchChatMessage(message);
                        OnChatMessageReceived(chatMessage);
                    }
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
                    FireAnEventBasedOnUserNoticeType(message);
                    break;
                case "HOSTTARGET":
                    var channelHost = new TwitchChannelHost(message);
                    if (!string.IsNullOrEmpty(channelHost.TargetChannel))
                    {
                        OnChannelHostingStarted(channelHost);
                    }
                    else
                    {
                        OnChannelHostingStopped(channelHost);
                    }
                    break;
                case "CLEARCHAT":
                    if (message.Tags.ContainsKey("ban-duration"))
                    {
                        var userTimeout = new TwitchUserTimeout(message);
                        OnUserTimedOut(userTimeout);
                    }
                    else if (message.Tags.ContainsKey("ban-reason"))
                    {
                        var userBan = new TwitchUserBan(message);
                        OnUserBanned(userBan);
                    }
                    else
                    {
                        string channel = message.GetChannel();
                        OnChatCleared(channel);
                    }
                    break;
                default:
                    base.OnUnprocessedIrcMessageReceived(message);
                    break;
            }
        }

        private void FireAnEventBasedOnUserNoticeType(IrcMessage message)
        {
            if (message.Tags.TryGetValue("msg-id", out string userNoticeType))
            {
                switch (userNoticeType)
                {
                    case "sub":
                        var subscription = new TwitchSubscription(message);
                        OnUserSubscribed(subscription);
                        break;
                    case "resub":
                        var resubscription = new TwitchSubscription(message);
                        OnUserResubscribed(resubscription);
                        break;
                    case "subgift":
                        var subscriptionGift = new TwitchSubscriptionGift(message);
                        OnSubscriptionGifted(subscriptionGift);
                        break;
                    case "raid":
                        var channelRaid = new TwitchChannelRaid(message);
                        OnChannelRaided(channelRaid);
                        break;
                    case "ritual":
                        var channelRitual = new TwitchChannelRitual(message);
                        OnChannelRitualPerformed(channelRitual);
                        break;
                }
            }
        }

        protected virtual void OnGlobalUserStateReceived(TwitchGlobalUserState globalUserState)
        {
            GlobalUserStateReceived?.Invoke(this, new GlobalUserStateReceivedEventArgs(globalUserState));
        }

        protected virtual void OnChannelBeingHosted(TwitchHostedChannel hostedChannel)
        {
            ChannelBeingHosted?.Invoke(this, new ChannelBeingHostedEventArgs(hostedChannel));
        }

        protected virtual void OnChatMessageReceived(TwitchChatMessage message)
        {
            ChatMessageReceived?.Invoke(this, new ChatMessageReceivedEventArgs(message));
        }

        protected virtual void OnWhisperMessageReceived(TwitchWhisperMessage message)
        {
            WhisperMessageReceived?.Invoke(this, new WhisperMessageReceivedEventArgs(message));
        }

        protected virtual void OnChannelStateChanged(TwitchChannelState channelState)
        {
            ChannelStateChanged?.Invoke(this, new ChannelStateChangedEventArgs(channelState));
        }

        protected virtual void OnUserStateReceived(TwitchUserState userState)
        {
            UserStateReceived?.Invoke(this, new UserStateReceivedEventArgs(userState));
        }

        protected virtual void OnUserSubscribed(TwitchSubscription subscription)
        {
            UserSubscribed?.Invoke(this, new UserSubscribedEventArgs(subscription));
        }

        protected virtual void OnUserResubscribed(TwitchSubscription resubscription)
        {
            UserResubscribed?.Invoke(this, new UserSubscribedEventArgs(resubscription));
        }

        protected virtual void OnSubscriptionGifted(TwitchSubscriptionGift subscriptionGift)
        {
            SubscriptionGifted?.Invoke(this, new SubscriptionGiftedEventArgs(subscriptionGift));
        }

        protected virtual void OnChannelRaided(TwitchChannelRaid channelRaid)
        {
            ChannelRaided?.Invoke(this, new ChannelRaidedEventArgs(channelRaid));
        }

        protected virtual void OnChannelRitualPerformed(TwitchChannelRitual channelRitual)
        {
            ChannelRitualPerformed?.Invoke(this, new ChannelRitualPerformedEventArgs(channelRitual));
        }

        protected virtual void OnChannelHostingStarted(TwitchChannelHost channelHost)
        {
            ChannelHostingStarted?.Invoke(this, new ChannelHostingStartedEventArgs(channelHost));
        }

        protected virtual void OnChannelHostingStopped(TwitchChannelHost channelHost)
        {
            ChannelHostingStopped?.Invoke(this, new ChannelHostingStoppedEventArgs(channelHost));
        }

        protected virtual void OnUserTimedOut(TwitchUserTimeout userTimeout)
        {
            UserTimedOut?.Invoke(this, new UserTimedOutEventArgs(userTimeout));
        }

        protected virtual void OnUserBanned(TwitchUserBan userBan)
        {
            UserBanned?.Invoke(this, new UserBannedEventArgs(userBan));
        }

        protected virtual void OnChatCleared(string channel)
        {
            ChatCleared?.Invoke(this, new ChatClearedEventArgs(channel));
        }

        public void SendWhisperMessage(string userName, string message)
        {
            SendIrcMessage($"PRIVMSG #jtv :/w {userName} {message}");
        }
    }
}