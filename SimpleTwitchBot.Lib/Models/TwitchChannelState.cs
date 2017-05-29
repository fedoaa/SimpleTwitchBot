using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchChannelState: TwitchMessage
    {
        public string BroadcasterLanguage { get; set; }

        public string Channel { get; set; }

        public int FollowersOnlyMinutes { get; set; }

        public bool IsFollowersOnly => FollowersOnlyMinutes != -1;

        public bool IsEmoteOnly { get; set; }

        public bool IsR9k { get; set; }

        public bool IsSlowMode { get; set; }

        public bool IsSubOnly { get; set; }

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
                        IsEmoteOnly = tag.Value.Equals("1");
                        break;
                    case "followers-only":
                        FollowersOnlyMinutes = int.Parse(tag.Value);
                        break;
                    case "r9k":
                        IsR9k = tag.Value.Equals("1");
                        break;
                    case "slow":
                        IsSlowMode = tag.Value.Equals("1");
                        break;
                    case "subs-only":
                        IsSubOnly = tag.Value.Equals("1");
                        break;
                }
            }

            Channel = message.Params[0];
        }
    }
}