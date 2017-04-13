using SimpleTwitchBot.Lib.Models;

namespace SimpleTwitchBot.Lib.Events
{
    public class OnChannelStateChangedArgs
    {
        public TwitchChannelState ChannelState { get; set; }
    }
}