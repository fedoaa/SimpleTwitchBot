using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnChannelPartedArgs : EventArgs
    {
        public string Channel { get; set; }
    }
}