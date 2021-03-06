﻿using System;
using System.Collections.Generic;

namespace SimpleTwitchBot.Lib.Models
{
    public class IrcMessage
    {
        public string Command { get; set; }

        public string[] Parameters { get; set; }

        public string Prefix { get; set; }

        public string Raw { get; set; }

        public IDictionary<string, string> Tags { get; set; }

        public IrcMessage()
        {
        }

        public IrcMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("Message cannot be empty", nameof(message));
            }

            Raw = message;

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
            Tags = ircMessageTags;

            bool hasPrefix = message[position].Equals(':');
            if (hasPrefix)
            {
                int endOfPrefix = message.IndexOf(' ', position);
                int lengthOfPrefix = endOfPrefix - position - 1;
                string prefix = message.Substring(position + 1, lengthOfPrefix);

                Prefix = prefix;
                position = endOfPrefix + 1;
            }

            var ircMessageParams = new List<string>();
            int endOfCommand = message.IndexOf(' ', position);
            if (endOfCommand != -1)
            {
                int lengthOfCommand = endOfCommand - position;
                Command = message.Substring(position, lengthOfCommand);
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
                Command = message.Substring(position);
            }
            Parameters = ircMessageParams.ToArray();
        }

        public string GetChannel()
        {
            return GetParameterByIndex(0);
        }

        public string GetParameterByIndex(int index)
        {
            return Parameters?.Length > index ? Parameters[index] : string.Empty;
        }

        public string GetUserName()
        {
            string[] parts = Prefix?.Split('!', '@');
            return parts?.Length > 1 ? parts[1] : string.Empty;
        }
    }
}