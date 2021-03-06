﻿using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class PingReceivedEventArgs : EventArgs
    {
        public string ServerAddress { get; }

        public PingReceivedEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
    }
}