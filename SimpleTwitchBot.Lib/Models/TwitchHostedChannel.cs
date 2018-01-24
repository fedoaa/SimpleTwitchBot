namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchHostedChannel : TwitchMessage
    {
        public string HosterDisplayName { get; set; }

        public bool IsAutohost { get; set; }

        public int NumberOfViewers { get; set; }

        public string TargetChannel { get; set; }

        public TwitchHostedChannel(IrcMessage message)
        {
            TargetChannel = $"#{message.Channel}";

            string jtvMessage = message.Params[1];

            IsAutohost = jtvMessage.Contains("auto hosting");

            string[] parameters = jtvMessage.Split(' ');
            if (int.TryParse(parameters[parameters.Length - 1], out int numberOfViewers))
            {
                NumberOfViewers = numberOfViewers;
            }
            HosterDisplayName = parameters[0];
        }
    }
}