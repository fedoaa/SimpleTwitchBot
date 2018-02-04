using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class UnprocessedIrcMessageReceivedEventArgs : EventArgs
    {
        public IrcMessage Message { get; }

        public UnprocessedIrcMessageReceivedEventArgs(IrcMessage message)
        {
            Message = message;
        }
    }
}