using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelJoinedEventArgs : EventArgs
    {
        public string Channel { get; set; }
    }
}