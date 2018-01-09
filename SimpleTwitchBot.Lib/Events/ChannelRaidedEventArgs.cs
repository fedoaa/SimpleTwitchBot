using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelRaidedEventArgs : EventArgs
    {
        public TwitchChannelRaid ChannelRaid { get; }

        public ChannelRaidedEventArgs(TwitchChannelRaid channelRaid)
        {
            ChannelRaid = channelRaid;
        }
    }
}