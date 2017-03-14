using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnIrcMessageReceivedArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
