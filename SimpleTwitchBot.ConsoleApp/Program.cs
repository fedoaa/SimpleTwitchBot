using SimpleTwitchBot.Lib;
using SimpleTwitchBot.Lib.Events;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string channelName = "kappa";

            var client = new TwitchIrcClient("irc.twitch.tv", 6667);
            client.OnIrcMessage += Client_OnIrcMessage;
            client.OnUserMessage += Client_OnUserMessage;

            var connectionTask = client.ConnectAsync("username", "oauth:token");
            Task.WaitAll(connectionTask);
            client.EnableMessageTags();
            client.JoinChannel(channelName);

            client.SendChatMessage(channelName, "hello, world");

            Console.ReadLine();
            client.Disconnect();
        }

        private static void Client_OnIrcMessage(object sender, OnIrcMessageArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void Client_OnUserMessage(object sender, OnUserMessageArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}