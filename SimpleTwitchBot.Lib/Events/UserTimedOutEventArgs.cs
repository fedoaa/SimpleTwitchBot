using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserTimedOutEventArgs : EventArgs
    {
        public TwitchUserTimeout UserTimeout { get; }

        public UserTimedOutEventArgs(TwitchUserTimeout userTimeout)
        {
            UserTimeout = userTimeout;
        }
    }
}