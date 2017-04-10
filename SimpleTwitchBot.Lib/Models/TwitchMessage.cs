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
            }
            return TwitchUserType.Viewer;
        }
    }
}