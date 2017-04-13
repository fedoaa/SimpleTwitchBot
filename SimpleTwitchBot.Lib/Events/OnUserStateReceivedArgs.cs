using SimpleTwitchBot.Lib.Models;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnUserStateReceivedArgs
    {
        public TwitchUserState UserState { get; set; }
    }
}