using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnChatMessageReceivedArgs : EventArgs
    {
        public TwitchChatMessage Message { get; set; }
    }
}