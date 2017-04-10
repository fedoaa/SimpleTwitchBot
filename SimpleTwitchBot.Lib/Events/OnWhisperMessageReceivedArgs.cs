using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnWhisperMessageReceivedArgs : EventArgs
    {
        public TwitchWhisperMessage Message { get; set; }
    }
}