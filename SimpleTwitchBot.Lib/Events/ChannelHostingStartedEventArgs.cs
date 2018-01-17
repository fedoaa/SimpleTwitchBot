using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelHostingStartedEventArgs : EventArgs
    {
        public TwitchChannelHost ChannelHost { get; }

        public ChannelHostingStartedEventArgs(TwitchChannelHost channelHost)
        {
            ChannelHost = channelHost;
        }
    }
}