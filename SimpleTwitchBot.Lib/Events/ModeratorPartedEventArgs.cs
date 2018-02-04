using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ModeratorPartedEventArgs : EventArgs
    {
        public string UserName { get; }

        public string Channel { get; }

        public ModeratorPartedEventArgs(string userName, string channel)
        {
            UserName = userName;
            Channel = channel;
        }
    }
}