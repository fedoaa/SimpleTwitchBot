﻿using SimpleTwitchBot.Lib.Models;
using System;

namespace SimpleTwitchBot.Lib.Events
{
    public class GlobalUserStateReceivedEventArgs : EventArgs
    {
        public TwitchGlobalUserState GlobalUserState { get; }

        public GlobalUserStateReceivedEventArgs(TwitchGlobalUserState globalUserState)
        {
            GlobalUserState = globalUserState;
        }
    }
}