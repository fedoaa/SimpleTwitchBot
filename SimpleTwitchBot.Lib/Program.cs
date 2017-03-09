using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            const string channelName = "kappa";

            var client = new IrcClient("irc.twitch.tv", 6667);
            client.OnMessage += Client_OnMessage;
            
            var connectionTask = client.ConnectAsync("username", "oauth:token");
            Task.WaitAll(connectionTask);
            client.JoinRoom(channelName);

            Console.ReadLine();
            client.Disconnect();
        }

        private static void Client_OnMessage(IrcClient client, string message)
        {
            if (message.StartsWith("PING"))
            {
                client.SendIrcMessage("PONG :tmi.twitch.tv");
            }
            Console.WriteLine(message);
        }
    }
}