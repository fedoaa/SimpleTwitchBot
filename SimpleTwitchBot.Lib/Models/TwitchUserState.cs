using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchUserState: TwitchMessage
    {
        public string DisplayName { get; set; }

        public string UserColor { get; set; }

        public bool Moderator { get; set; }

        public bool Subscriber { get; set; }

        public TwitchUserType UserType { get; set; }

        public string Emotesets { get; set; }

        public string Badges { get; set; }

        public string Channel { get; set; }

        public TwitchUserState(IrcMessage message)
        {
            foreach (KeyValuePair<string, string> tag in message.Tags)
            {
                switch (tag.Key)
                {
                    case "badges":
                        Badges = tag.Value;
                        break;
                    case "color":
                        UserColor = tag.Value;
                        break;
                    case "display-name":
                        DisplayName = tag.Value;
                        break;
                    case "emote-sets":
                        Emotesets = tag.Value;
                        break;
                    case "mod":
                        Moderator = tag.Value.Equals("1");
                        break;
                    case "subscriber":
                        Subscriber = tag.Value.Equals("1");
                        break;
                    case "user-type":
                        UserType = ConvertToUserType(tag.Value);
                        break;
                }
            }

            Channel = message.Params[0];
        }
    }
}