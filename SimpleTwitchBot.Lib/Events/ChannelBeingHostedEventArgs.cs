using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelBeingHostedEventArgs : EventArgs
    {
        public TwitchHostedChannel HostedChannel { get; }

        public ChannelBeingHostedEventArgs(TwitchHostedChannel hostedChannel)
        {
            HostedChannel = hostedChannel;
        }
    }
}