using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelRitualPerformedEventArgs : EventArgs
    {
        public TwitchChannelRitual ChannelRitual { get; }

        public ChannelRitualPerformedEventArgs(TwitchChannelRitual channelRitual)
        {
            ChannelRitual = channelRitual;
        }
    }
}