using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class IrcMessage
    {
        public List<IrcMessageTag> Tags { get; set; }

        public string Prefix { get; set; }

        public string Command { get; set; }

        public List<string> Params { get; set; }

        public string Raw { get; set; }

        public static IrcMessage Parse(string message)
        {
            var ircMessage = new IrcMessage();
            ircMessage.Raw = message;
            
            bool hasTags = message.StartsWith("@");
            if (hasTags)
            {
                var ircMessageTags = new List<IrcMessageTag>();

                int endOfTags = message.IndexOf(" ");
                string[] messageTags = message.Substring(1, endOfTags - 1).Split(';');

                for (int i = 0; i < messageTags.Length; i++)
                {
                    string[] tagData = messageTags[i].Split('=');
                    var messageTag = new IrcMessageTag
                    {
                        Name = tagData[0],
                        Value = tagData[1]
                    };
                    ircMessageTags.Add(messageTag);
                }
                ircMessage.Tags = ircMessageTags;
                message = message.Remove(0, endOfTags + 1);
            }
            bool hasPrefix = message.StartsWith(":");
            if (hasPrefix)
            {
                int endOfPrefix = message.IndexOf(' ');
                string prefix = message.Substring(1, endOfPrefix - 1);

                ircMessage.Prefix = prefix;
                message = message.Remove(0, endOfPrefix + 1);
            }
            int endOfCommand = message.IndexOf(' ');
            if (endOfCommand != -1)
            {
                ircMessage.Command = message.Substring(0, endOfCommand);
                message = message.Remove(0, endOfCommand + 1);

                var ircMessageParams = new List<string>();
                
                int startOfTrailing = message.IndexOf(':');
                if (startOfTrailing != -1)
                {
                    string middle = message.Substring(0, startOfTrailing - 1);
                    string trailing = message.Substring(startOfTrailing + 1);

                    ircMessageParams.AddRange(middle.Split(' '));
                    ircMessageParams.Add(trailing);
                }
                else
                {
                    ircMessageParams.AddRange(message.Split(' '));
                }
                ircMessage.Params = ircMessageParams;
            }
            else
            {
                ircMessage.Command = message;
            }
            return ircMessage;
        }
    }
}