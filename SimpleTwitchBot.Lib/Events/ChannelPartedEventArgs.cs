using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChannelPartedEventArgs : EventArgs
    {
        public string Channel { get; set; }
    }
}