using SimpleTwitchBot.Lib;
using SimpleTwitchBot.Lib.Events;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new TwitchIrcClient("irc.chat.twitch.tv", 6667))
            {
                client.LoggedIn += Client_LoggedIn;
                client.ChannelJoined += Client_ChannelJoined;
                client.UserJoined += Client_UserJoined;
                client.IrcMessageReceived += Client_IrcMessageReceived;
                client.ChatMessageReceived += Client_ChatMessageReceived;
                client.UserSubscribed += Client_UserSubscribed;
                client.Disconnected += Client_Disconnected;

                Task.Run(async () => await client.ConnectAsync("username", "oauth:token"));

                Console.ReadLine();
                client.Disconnect();
            }
            Console.ReadLine();
        }

        private static void Client_LoggedIn(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            client?.JoinChannel("#channelName");
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