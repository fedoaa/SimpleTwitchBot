using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchUserBan : TwitchMessage
    {
        public string BanReason { get; set; }

        public string Channel { get; set; }

        public string ChannelId { get; set; }

        public string TargetUserId { get; set; }

        public string TargetUsername { get; set; }

        public long Timestamp { get; set; }

        public TwitchUserBan(IrcMessage message)
        {
            foreach (KeyValuePair<string, string> tag in message.Tags)
            {
                switch (tag.Key)
                {
                    case "ban-reason":
                        BanReason = tag.Value;
                        break;
                    case "room-id":
                        ChannelId = tag.Value;
                        break;
                    case "target-user-id":
                        TargetUserId = tag.Value;
                        break;
                    case "tmi-sent-ts":
                        Timestamp = long.Parse(tag.Value);
                        break;
                }
            }

            Channel = message.Channel;
            TargetUsername = message.Params[1];
        }
    }
}