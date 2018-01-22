using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchWhisperMessage : TwitchMessage
    {
        public IDictionary<string, string> Badges { get; set; }

        public string Body { get; set; }

        public string DisplayName { get; set; }

        public string Emotes { get; set; }

        public bool IsTurbo { get; set; }

        public string MessageId { get; set; }

        public string ThreadId { get; set; }

        public string UserColor { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public TwitchUserType UserType { get; set; }

        public TwitchWhisperMessage(IrcMessage message)
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
                    case "emotes":
                        Emotes = tag.Value;
                        break;
                    case "message-id":
                        MessageId = tag.Value;
                        break;
                    case "thread-id":
                        ThreadId = tag.Value;
                        break;
                    case "turbo":
                        IsTurbo = tag.Value.Equals("1");
                        break;
                    case "user-id":
                        UserId = tag.Value;
                        break;
                    case "user-type":
                        UserType = ConvertToUserType(tag.Value);
                        break;
                }
            }

            Username = message.Username;
            Body = message.Params[1];
        }
    }
}