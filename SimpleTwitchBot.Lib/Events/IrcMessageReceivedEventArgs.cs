using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class IrcMessageReceivedEventArgs : EventArgs
    {
        public IrcMessage Message { get; }

        public IrcMessageReceivedEventArgs(IrcMessage message)
        {
            Message = message;
        }
    }
}