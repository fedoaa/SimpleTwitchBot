using System;
using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class IrcMessage
    {
        public Dictionary<string, string> Tags { get; set; }

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
                var ircMessageTags = new Dictionary<string, string>();

                int endOfTags = message.IndexOf(' ');
                string[] messageTags = message.Substring(1, endOfTags - 1).Split(';');

                for (int i = 0; i < messageTags.Length; i++)
                {
                    string[] tagData = messageTags[i].Split('=');
                    string tagName = tagData[0], tagValue = tagData[1];

                    ircMessageTags.Add(tagName, tagValue);
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
                    if (lengthOfMiddle > 0)
                    {
                        string middle = message.Substring(position, lengthOfMiddle);
                        ircMessageParams.AddRange(middle.Split(' '));
                    }
                    
                    string trailing = message.Substring(startOfTrailing + 1);
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