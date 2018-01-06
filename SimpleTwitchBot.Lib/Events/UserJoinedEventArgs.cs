using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserJoinedEventArgs : EventArgs
    {
        public string Username { get; }

        public string Channel { get; }

        public UserJoinedEventArgs(string username, string channel)
        {
            Username = username;
            Channel = channel;
        }
    }
}