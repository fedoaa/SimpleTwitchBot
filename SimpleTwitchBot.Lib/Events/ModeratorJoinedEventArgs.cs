using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ModeratorJoinedEventArgs : EventArgs
    {
        public string UserName { get; }

        public string Channel { get; }

        public ModeratorJoinedEventArgs(string userName, string channel)
        {
            UserName = userName;
            Channel = channel;
        }
    }
}