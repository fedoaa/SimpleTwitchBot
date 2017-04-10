using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchChatMessage : TwitchMessage
    {
        public string MessageId { get; set; }

        public long Timestamp { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string UserColor { get; set; }

        public bool Moderator { get; set; }

        public bool Subscriber { get; set; }

        public bool Turbo { get; set; }

        public TwitchUserType UserType { get; set; }

        public string Emotes { get; set; }

        public string Body { get; set; }

        public string Badges { get; set; }

        public string RoomId { get; set; }

        public int Bits { get; set; }

        public bool IsAction { get; set; }

        public string Channel { get; set; }

        public TwitchChatMessage(IrcMessage message)
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
                    case "emotes":
                        Emotes = tag.Value;
                        break;
                    case "id":
                        MessageId = tag.Value;
                        break;
                    case "mod":
                        Moderator = tag.Value.Equals("1");
                        break;
                    case "room-id":
                        RoomId = tag.Value;
                        break;
                    case "tmi-sent-ts":
                        Timestamp = long.Parse(tag.Value);
                        break;
                    case "subscriber":
                        Subscriber = tag.Value.Equals("1");
                        break;
                    case "turbo":
                        Turbo = tag.Value.Equals("1");
                        break;
                    case "user-id":
                        UserId = tag.Value;
                        break;
                    case "user-type":
                        UserType = ConvertToUserType(tag.Value);
                        break;
                    case "bits":
                        Bits = int.Parse(tag.Value);
                        break;
                }
            }

            Username = message.Prefix.Split('!', '@')[1];
            Channel = message.Params[0];

            string messageBody = message.Params[1];
            Match match = Regex.Match(messageBody, @"\u0001ACTION\s.+\u0001");
            if (match.Success)
            {
                Body = match.Value;
                IsAction = true;
            }
            else
            {
                Body = messageBody;
            }
        }
    }
}