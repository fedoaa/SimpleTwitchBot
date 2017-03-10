using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnChannelPartArgs : EventArgs
    {
        public string Channel { get; set; }
    }
}