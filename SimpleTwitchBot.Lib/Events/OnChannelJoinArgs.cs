using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnChannelJoinArgs : EventArgs
    {
        public string Channel { get; set; }
    }
}