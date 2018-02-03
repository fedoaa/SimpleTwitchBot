namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchHostedChannel : TwitchMessageBase
    {
        public string HosterDisplayName { get; set; }

        public bool IsAutohost { get; set; }

        public int NumberOfViewers { get; set; }

        public string TargetChannel { get; set; }

        public TwitchHostedChannel(IrcMessage message)
        {
            string channel = message.GetChannel();
            TargetChannel = $"#{channel}";

            string jtvMessage = message.GetParameterByIndex(1);

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