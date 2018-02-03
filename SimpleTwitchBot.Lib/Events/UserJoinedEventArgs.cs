using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserJoinedEventArgs : EventArgs
    {
        public string UserName { get; }

        public string Channel { get; }

        public UserJoinedEventArgs(string userName, string channel)
        {
            UserName = userName;
            Channel = channel;
        }
    }
}