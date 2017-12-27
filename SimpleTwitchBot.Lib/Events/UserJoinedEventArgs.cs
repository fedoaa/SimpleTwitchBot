using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserJoinedEventArgs : EventArgs
    {
        public string Username { get; set; }

        public string Channel { get; set; }
    }
}