using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnIrcMessageArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
