using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchChannelState: TwitchMessage
    {
        public string BroadcasterLanguage { get; set; }

        public bool EmoteOnly { get; set; }

        public int FollowersOnly { get; set; }

        public bool R9k { get; set; }

        public bool SlowMode { get; set; }

        public bool SubOnly { get; set; }

        public string Channel { get; set; }

        public TwitchChannelState(IrcMessage message)
        {
            foreach (KeyValuePair<string, string> tag in message.Tags)
            {
                switch (tag.Key)
                {
                    case "broadcaster-lang":
                        BroadcasterLanguage = tag.Value;
                        break;
                    case "emote-only":
                        EmoteOnly = tag.Value.Equals("1");
                        break;
                    case "followers-only":
                        FollowersOnly = int.Parse(tag.Value);
                        break;
                    case "r9k":
                        R9k = tag.Value.Equals("1");
                        break;
                    case "slow":
                        SlowMode = tag.Value.Equals("1");
                        break;
                    case "subs-only":
                        SubOnly = tag.Value.Equals("1");
                        break;
                }
            }

            Channel = message.Params[0];
        }
    }
}