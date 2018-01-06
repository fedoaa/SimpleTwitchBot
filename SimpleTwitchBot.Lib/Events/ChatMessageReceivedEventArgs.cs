using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class ChatMessageReceivedEventArgs : EventArgs
    {
        public TwitchChatMessage Message { get; }

        public ChatMessageReceivedEventArgs(TwitchChatMessage message)
        {
            Message = message;
        }
    }
}