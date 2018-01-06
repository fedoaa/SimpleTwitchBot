using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchGlobalUserState : TwitchMessage
    {
        public IDictionary<string, string> Badges { get; set; }

        public string DisplayName { get; set; }

        public IList<string> Emotesets { get; set; }

        public string UserColor { get; set; }

        public string UserId { get; set; }

        public TwitchUserType UserType { get; set; }

        public TwitchGlobalUserState(IrcMessage message)
        {
            foreach (KeyValuePair<string, string> tag in message.Tags)
            {
                switch (tag.Key)
                {
                    case "badges":
                        Badges = ParseBadges(tag.Value);
                        break;
                    case "color":
                        UserColor = tag.Value;
                        break;
                    case "display-name":
                        DisplayName = tag.Value;
                        break;
                    case "emote-sets":
                        Emotesets = tag.Value.Split(',');
                        break;
                    case "user-id":
                        UserId = tag.Value;
                        break;
                    case "user-type":
                        UserType = ConvertToUserType(tag.Value);
                        break;
                }
            }
        }
    }
}