using System.Text.RegularExpressions;

namespace SimpleTwitchBot.Lib.Models
{
    public class ChannelMode
    {
        public ChannelModeActionType Action { get; set; }

        public string Channel { get; set; }

        public string Modes { get; set; }

        public string ModeParams { get; set; }

        public ChannelMode(IrcMessage message)
        {
            string modesWithAction = message.GetParameterByIndex(1);
            Match modesMatch = Regex.Match(modesWithAction, @"^([+-])?([a-zA-Z]{1,3})$");
            if (modesMatch.Success)
            {
                string rawActionValue = modesMatch.Groups[1].Value;

                Action = GetChannelModeActionType(rawActionValue);
                Modes = modesMatch.Groups[2].Value;
            }

            ModeParams = message.GetParameterByIndex(2);
            Channel = message.GetChannel();
        }

        private ChannelModeActionType GetChannelModeActionType(string symbol)
        {
            switch (symbol)
            {
                case "+":
                    return ChannelModeActionType.Add;
                case "-":
                    return ChannelModeActionType.Remove;
                default:
                    return ChannelModeActionType.NotSet;
            }
        }
    }
}