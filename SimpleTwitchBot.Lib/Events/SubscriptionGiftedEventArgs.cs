using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class SubscriptionGiftedEventArgs : EventArgs
    {
        public TwitchSubscriptionGift SubscriptionGift { get; }

        public SubscriptionGiftedEventArgs(TwitchSubscriptionGift subscriptionGift)
        {
            SubscriptionGift = subscriptionGift;
        }
    }
}