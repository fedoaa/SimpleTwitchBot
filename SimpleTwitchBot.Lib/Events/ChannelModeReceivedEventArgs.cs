using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelModeReceivedEventArgs : EventArgs
    {
        public ChannelMode ChannelMode { get; }

        public ChannelModeReceivedEventArgs(ChannelMode channelMode)
        {
            ChannelMode = channelMode;
        }
    }
}