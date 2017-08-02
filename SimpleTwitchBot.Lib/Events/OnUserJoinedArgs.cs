using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnUserJoinedArgs : EventArgs
    {
        public string Username { get; set; }

        public string Channel { get; set; }
    }
}