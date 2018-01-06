using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelStateChangedEventArgs : EventArgs
    {
        public TwitchChannelState ChannelState { get; }

        public ChannelStateChangedEventArgs(TwitchChannelState channelState)
        {
            ChannelState = channelState;
        }
    }
}