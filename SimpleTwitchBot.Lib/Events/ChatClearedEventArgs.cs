using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChatClearedEventArgs : EventArgs
    {
        public string Channel { get; }

        public ChatClearedEventArgs(string channel)
        {
            Channel = channel;
        }
    }
}