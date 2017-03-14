using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnUserMessageReceivedArgs : EventArgs
    {
        public TwitchUserMessage Message { get; set; }
    }
}