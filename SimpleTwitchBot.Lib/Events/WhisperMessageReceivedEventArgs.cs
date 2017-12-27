using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class WhisperMessageReceivedEventArgs : EventArgs
    {
        public TwitchWhisperMessage Message { get; set; }
    }
}