using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserBannedEventArgs : EventArgs
    {
        public TwitchUserBan UserBan { get; }

        public UserBannedEventArgs(TwitchUserBan userBan)
        {
            UserBan = userBan;
        }
    }
}