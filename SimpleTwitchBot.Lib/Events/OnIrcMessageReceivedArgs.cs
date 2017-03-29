using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnIrcMessageReceivedArgs : EventArgs
    {
        public IrcMessage Message { get; set; }
    }
}