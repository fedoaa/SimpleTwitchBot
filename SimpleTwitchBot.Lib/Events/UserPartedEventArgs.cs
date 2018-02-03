using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserPartedEventArgs : EventArgs
    {
        public string UserName { get; }

        public string Channel { get; }

        public UserPartedEventArgs(string userName, string channel)
        {
            UserName = userName;
            Channel = channel;
        }
    }
}