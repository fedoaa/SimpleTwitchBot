namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchChannelHost
    {
        public string HostingChannel { get; set; }

        public string TargetChannel { get; set; }

        public int NumberOfViewers { get; set; }

        public TwitchChannelHost(IrcMessage message)
        {
            HostingChannel = message.Channel;

            string[] parameters = message.Params[1].Split(' ');
            if (!parameters[0].Equals("-"))
            {
                TargetChannel = $"#{parameters[0]}";
            }
            if (int.TryParse(parameters[1], out int numberOfViewers))
            {
                NumberOfViewers = numberOfViewers;
            }
        }
    }
}