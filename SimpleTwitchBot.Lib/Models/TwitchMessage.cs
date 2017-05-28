using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public abstract class TwitchMessage
    {
        protected TwitchUserType ConvertToUserType(string tagValue)
        {
            switch (tagValue)
            {
                case "mod":
                    return TwitchUserType.Moderator;
                case "global_mod":
                    return TwitchUserType.GlobalModerator;
                case "admin":
                    return TwitchUserType.Admin;
                case "staff":
                    return TwitchUserType.Staff;
                default:
                    return TwitchUserType.Viewer;
            }
        }

        protected Dictionary<string, string> ParseBadges(string tagValue)
        {
            var messageBadges = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(tagValue))
            {
                return messageBadges;
            }

            string[] badges = tagValue.Split(',');
            for (int i = 0; i < badges.Length; i++)
            {
                string[] badgeData = badges[i].Split('/');
                string badgeName = badgeData[0], badgeVersion = badgeData[1];

                messageBadges.Add(badgeName, badgeVersion);
            }
            return messageBadges;
        }
    }
}