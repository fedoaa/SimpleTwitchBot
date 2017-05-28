using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnUserSubscribedArgs : EventArgs
    {
        public TwitchSubscription Subscription { get; set; }
    }
}