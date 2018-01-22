using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserPartedEventArgs : EventArgs
    {
        public string Username { get; }

        public string Channel { get; }

        public UserPartedEventArgs(string username, string channel)
        {
            Username = username;
            Channel = channel;
        }
    }
}