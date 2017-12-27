using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UserSubscribedEventArgs : EventArgs
    {
        public TwitchSubscription Subscription { get; set; }
    }
}