using System;
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
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(message);
            }

            var ircMessage = new IrcMessage { Raw = message };
            int position = 0;
            
            bool hasTags = message.StartsWith("@");
            if (hasTags)
            {
                var ircMessageTags = new List<IrcMessageTag>();

                int endOfTags = message.IndexOf(' ');
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
                position = endOfTags + 1;
            }
            bool hasPrefix = message[position].Equals(':');
            if (hasPrefix)
            {
                int endOfPrefix = message.IndexOf(' ', position);
                int lengthOfPrefix = endOfPrefix - position - 1;
                string prefix = message.Substring(position + 1, lengthOfPrefix);

                ircMessage.Prefix = prefix;
                position = endOfPrefix + 1;
            }
            int endOfCommand = message.IndexOf(' ', position);
            if (endOfCommand != -1)
            {
                int lengthOfCommand = endOfCommand - position;
                ircMessage.Command = message.Substring(position, lengthOfCommand);
                position = endOfCommand + 1;

                var ircMessageParams = new List<string>();
                
                int startOfTrailing = message.IndexOf(':', position);
                if (startOfTrailing != -1)
                {
                    int lengthOfMiddle = startOfTrailing - position - 1;
                    string middle = message.Substring(position, lengthOfMiddle);
                    string trailing = message.Substring(startOfTrailing + 1);

                    ircMessageParams.AddRange(middle.Split(' '));
                    ircMessageParams.Add(trailing);
                }
                else
                {
                    string middle = message.Substring(position);
                    ircMessageParams.AddRange(middle.Split(' '));
                }
                ircMessage.Params = ircMessageParams;
            }
            else
            {
                ircMessage.Command = message.Substring(position);
            }
            return ircMessage;
        }
    }
}