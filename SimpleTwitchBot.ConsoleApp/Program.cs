using SimpleTwitchBot.Lib;
using SimpleTwitchBot.Lib.Events;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot.ConsoleApp
{
    class Program
    {
        private static string channelName = "kappa";

        private static TwitchIrcClient _client;

        static void Main(string[] args)
        {
            _client = new TwitchIrcClient("irc.twitch.tv", 6667);
            _client.OnConnect += Client_OnConnect;
            _client.OnChannelJoin += Client_OnChannelJoin;
            _client.OnIrcMessageReceived += Client_OnIrcMessageReceived;
            _client.OnUserMessageReceived += Client_OnUserMessageReceived;
            _client.OnDisconnect += Client_OnDisconnect;

            var connectionTask = _client.ConnectAsync("username", "oauth:token");
            Task.WaitAll(connectionTask);
           
            Console.ReadLine();
            _client.Disconnect();
            Console.ReadLine();
        }

        private static void Client_OnConnect(object sender, EventArgs e)
        {
            _client.JoinChannel(channelName);
        }

        private static void Client_OnChannelJoin(object sender, OnChannelJoinArgs e)
        {
            _client.SendChatMessage(e.Channel, "Keepo");
        }

        private static void Client_OnIrcMessageReceived(object sender, OnIrcMessageReceivedArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void Client_OnUserMessageReceived(object sender, OnUserMessageReceivedArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void Client_OnDisconnect(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }
    }
}