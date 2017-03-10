using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnUserMessageArgs : EventArgs
    {
        public TwitchUserMessage Message { get; set; }
    }
}