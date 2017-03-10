using SimpleTwitchBot.Lib.Events;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot
{
    class Program
    {
        private static IrcClient _client;

        static void Main(string[] args)
        {
            const string channelName = "kappa";

            var client = new IrcClient("irc.twitch.tv", 6667);
            client.OnMessage += Client_OnMessage;
            client.OnPing += Client_OnPing;
            
            var connectionTask = client.ConnectAsync("username", "oauth:token");
            Task.WaitAll(connectionTask);
            client.SendIrcMessage("CAP REQ :twitch.tv/tags");
            client.SendIrcMessage("CAP REQ :twitch.tv/commands");
            client.JoinChannel(channelName);

            _client = client;

            client.SendChatMessage(channelName, "hello, world");

            Console.ReadLine();
            client.Disconnect();
        }

        private static void Client_OnMessage(object sender, OnIrcMessageArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void Client_OnPing(object sender, OnPingArgs e)
        {
            _client.SendIrcMessage($"PONG {e.ServerAddress}");
        }
    }
}