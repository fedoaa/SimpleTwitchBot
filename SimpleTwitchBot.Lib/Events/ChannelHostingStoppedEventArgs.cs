using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelHostingStoppedEventArgs : EventArgs
    {
        public TwitchChannelHost ChannelHost { get; }

        public ChannelHostingStoppedEventArgs(TwitchChannelHost channelHost)
        {
            ChannelHost = channelHost;
        }
    }
}