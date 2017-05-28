using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchSubscription: TwitchMessage
    {
        public IDictionary<string, string> Badges { get; set; }

        public string Body { get; set; }

        public string Channel { get; set; }

        public string ChannelId { get; set; }

        public string DisplayName { get; set; }

        public string Emotes { get; set; }

        public bool Moderator { get; set; }

        public string MessageId { get; set; }

        public int Months { get; set; }

        public TwitchSubscriptionType SubscriptionType { get; set; }

        public bool Subscriber { get; set; }

        public string SubscriptionPlanName { get; set; }

        public TwitchSubscriptionPlanType SubscriptionPlanType { get; set; }

        public string SystemMessage { get; set; }

        public long Timestamp { get; set; }

        public bool Turbo { get; set; }

        public string UserColor { get; set; }

        public string Username { get; set; }

        public string UserId { get; set; }

        public TwitchUserType UserType { get; set; }

        public TwitchSubscription(IrcMessage message)
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
                        Moderator = tag.Value.Equals("1");
                        break;
                    case "msg-id":
                        SubscriptionType = ConvertToSubscriptionType(tag.Value);
                        break;
                    case "msg-param-months":
                        Months = int.Parse(tag.Value);
                        break;
                    case "msg-param-sub-plan":
                        SubscriptionPlanType = ConvertToSubscriptionPlanType(tag.Value);
                        break;
                    case "msg-param-sub-plan-name":
                        SubscriptionPlanName = tag.Value.Replace("\\s", " ");
                        break;
                    case "room-id":
                        ChannelId = tag.Value;
                        break;
                    case "subscriber":
                        Subscriber = tag.Value.Equals("1");
                        break;
                    case "system-msg":
                        SystemMessage = tag.Value.Replace("\\s", " ");
                        break;
                    case "tmi-sent-ts":
                        Timestamp = long.Parse(tag.Value);
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
                }

                Channel = message.Params[0];
            }
        }

        private TwitchSubscriptionType ConvertToSubscriptionType(string tagValue)
        {
            switch (tagValue)
            {
                case "sub":
                    return TwitchSubscriptionType.FirstTime;
                case "resub":
                    return TwitchSubscriptionType.Resub;
                case "charity":
                    return TwitchSubscriptionType.Charity;
                default:
                    return TwitchSubscriptionType.Unknown;
            }
        }

        private TwitchSubscriptionPlanType ConvertToSubscriptionPlanType(string tagValue)
        {
            switch (tagValue)
            {
                case "Prime":
                    return TwitchSubscriptionPlanType.Prime;
                case "1000":
                    return TwitchSubscriptionPlanType.Tier1;
                case "2000":
                    return TwitchSubscriptionPlanType.Tier2;
                case "3000":
                    return TwitchSubscriptionPlanType.Tier3;
                default:
                    return TwitchSubscriptionPlanType.Unknown;
            }
        }
    }
}