using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnChannelJoinedArgs : EventArgs
    {
        public string Channel { get; set; }
    }
}