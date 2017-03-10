using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnPingArgs : EventArgs
    {
        public string ServerAddress { get; set; }
    }
}