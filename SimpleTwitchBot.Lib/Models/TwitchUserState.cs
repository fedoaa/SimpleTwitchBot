using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchUserState: TwitchMessage
    {
        public IDictionary<string, string> Badges { get; set; }

        public string Channel { get; set; }

        public string DisplayName { get; set; }

        public IList<string> Emotesets { get; set; }

        public bool IsModerator { get; set; }

        public bool IsSubscriber { get; set; }

        public string UserColor { get; set; }

        public TwitchUserType UserType { get; set; }
       
        public TwitchUserState(IrcMessage message)
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
                    case "mod":
                        IsModerator = tag.Value.Equals("1");
                        break;
                    case "subscriber":
                        IsSubscriber = tag.Value.Equals("1");
                        break;
                    case "user-type":
                        UserType = ConvertToUserType(tag.Value);
                        break;
                }
            }

            Channel = message.Channel;
        }
    }
}