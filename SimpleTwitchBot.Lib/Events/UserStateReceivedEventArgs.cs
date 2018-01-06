using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserStateReceivedEventArgs : EventArgs
    {
        public TwitchUserState UserState { get; }

        public UserStateReceivedEventArgs(TwitchUserState userState)
        {
            UserState = userState;
        }
    }
}