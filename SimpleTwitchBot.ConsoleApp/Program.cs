using SimpleTwitchBot.Lib;
using SimpleTwitchBot.Lib.Events;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot.ConsoleApp
{
    class Program
    {
        private static string channelName = "#channelName";

        private static TwitchIrcClient _client;

        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        public static async Task MainAsync()
        {
            _client = new TwitchIrcClient("irc.chat.twitch.tv", 6667);
            _client.Connected += Client_Connected;
            _client.ChannelJoined += Client_ChannelJoined;
            _client.UserJoined += Client_UserJoined;
            _client.IrcMessageReceived += Client_IrcMessageReceived;
            _client.ChatMessageReceived += Client_ChatMessageReceived;
            _client.UserSubscribed += Client_UserSubscribed;
            _client.Disconnected += Client_Disconnected;

            await _client.ConnectAsync("username", "oauth:token");

            Console.ReadLine();
            _client.Disconnect();
            Console.ReadLine();
        }

        private static void Client_Connected(object sender, EventArgs e)
        {
            _client.JoinChannel(channelName);
        }

        private static void Client_ChannelJoined(object sender, ChannelJoinedEventArgs e)
        {
            Console.WriteLine($"Joined {e.Channel}");
        }

        private static void Client_UserJoined(object sender, UserJoinedEventArgs e)
        {
            Console.WriteLine($"{e.Username} has joined {e.Channel}");
        }

        private static void Client_IrcMessageReceived(object sender, IrcMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message.Raw);
        }

        private static void Client_ChatMessageReceived(object sender, ChatMessageReceivedEventArgs e)
        {
            Console.WriteLine($"{e.Message.Username}: {e.Message.Body}");
        }

        private static void Client_UserSubscribed(object sender, UserSubscribedEventArgs e)
        {
            Console.WriteLine($"{e.Subscription.Username} just subscribed!");
        }

        private static void Client_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }
    }
}