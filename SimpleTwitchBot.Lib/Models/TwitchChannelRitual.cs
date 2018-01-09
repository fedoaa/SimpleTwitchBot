using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchChannelRitual : TwitchMessage
    {
        public IDictionary<string, string> Badges { get; set; }

        public string Body { get; set; }

        public string Channel { get; set; }

        public string ChannelId { get; set; }

        public string DisplayName { get; set; }

        public string Emotes { get; set; }

        public bool IsModerator { get; set; }

        public bool IsSubscriber { get; set; }

        public bool IsTurbo { get; set; }

        public string MessageId { get; set; }

        public string RitualName { get; set; }

        public string SystemMessage { get; set; }

        public long Timestamp { get; set; }

        public string UserColor { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public TwitchUserType UserType { get; set; }

        public TwitchChannelRitual(IrcMessage message)
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
                    case "id":
                        MessageId = tag.Value;
                        break;
                    case "login":
                        Username = tag.Value;
                        break;
                    case "message":
                        Body = tag.Value;
                        break;
                    case "mod":
                        IsModerator = tag.Value.Equals("1");
                        break;
                    case "msg-param-ritual-name":
                        RitualName = tag.Value;
                        break;
                    case "room-id":
                        ChannelId = tag.Value;
                        break;
                    case "subscriber":
                        IsSubscriber = tag.Value.Equals("1");
                        break;
                    case "system-msg":
                        SystemMessage = tag.Value.Replace("\\s", " ");
                        break;
                    case "tmi-sent-ts":
                        Timestamp = long.Parse(tag.Value);
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

            Channel = message.Params[0];
        }
    }
}