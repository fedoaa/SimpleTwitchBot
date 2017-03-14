namespace SimpleTwitchBot.Lib.Models
{
    public class TwitchUserMessage
    {
        public long Timestamp { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string UserColor { get; set; }
        public bool Subscriber { get; set; }
        public bool Turbo { get; set; }
        public string UserType { get; set; }
        public string Emotes { get; set; }
        public string Body { get; set; }
        public string Badges { get; set; }
        public int? Bits { get; set; }
        public string Type { get; set; }
    }
}
