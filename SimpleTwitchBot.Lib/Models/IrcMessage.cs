using System;
using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class IrcMessage
    {
        public string Command { get; set; }

        public IList<string> Params { get; set; }

        public string Prefix { get; set; }

        public string Raw { get; set; }

        public IDictionary<string, string> Tags { get; set; }

        public IrcMessage()
        {

        }

        public IrcMessage(string message)
        {
            IrcMessage ircMessage = Parse(message);

            Command = ircMessage.Command;
            Params = ircMessage.Params;
            Prefix = ircMessage.Prefix;
            Raw = ircMessage.Raw;
            Tags = ircMessage.Tags;
        }

        public static IrcMessage Parse(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(message);
            }

            var ircMessage = new IrcMessage { Raw = message };

            int position = 0;
            bool hasTags = message.StartsWith("@");
            var ircMessageTags = new Dictionary<string, string>();

            if (hasTags)
            {
                int endOfTags = message.IndexOf(' ');
                string[] tags = message.Substring(1, endOfTags - 1).Split(';');

                for (int i = 0; i < tags.Length; i++)
                {
                    string[] tagData = tags[i].Split('=');
                    string tagName = tagData[0], tagValue = tagData[1];

                    ircMessageTags.Add(tagName, tagValue);
                }
                position = endOfTags + 1;
            }
            ircMessage.Tags = ircMessageTags;

            bool hasPrefix = message[position].Equals(':');
            if (hasPrefix)
            {
                int endOfPrefix = message.IndexOf(' ', position);
                int lengthOfPrefix = endOfPrefix - position - 1;
                string prefix = message.Substring(position + 1, lengthOfPrefix);

                ircMessage.Prefix = prefix;
                position = endOfPrefix + 1;
            }

            var ircMessageParams = new List<string>();
            int endOfCommand = message.IndexOf(' ', position);
            if (endOfCommand != -1)
            {
                int lengthOfCommand = endOfCommand - position;
                ircMessage.Command = message.Substring(position, lengthOfCommand);
                position = endOfCommand + 1;
                
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
            }
            else
            {
                ircMessage.Command = message.Substring(position);
            }
            ircMessage.Params = ircMessageParams;
            return ircMessage;
        }
    }
}