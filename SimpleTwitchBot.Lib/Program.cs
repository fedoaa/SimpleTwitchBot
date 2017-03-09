using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            const string channelName = "kappa";

            var client = new IrcClient();
            
            var connectionTask = client.ConnectAsync("irc.twitch.tv", 6667, "username", "oauth:token");
            Task.WaitAll(connectionTask);
            client.JoinRoom(channelName);

            while (true)
            {
                string response = client.ReadMessage();
                if (response.StartsWith("PING"))
                {
                    client.SendIrcMessage("PONG :tmi.twitch.tv");
                    continue;
                }
                Console.WriteLine(response);
            }

        }
    }
}