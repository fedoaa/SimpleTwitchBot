using SimpleTwitchBot.Lib.Events.Network;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib.Network
{
    public interface ISimpleTcpClient : IDisposable
    {
        event EventHandler Connected;
        event EventHandler Disconnected;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        Task ConnectAsync(string host, int port);
        void Disconnect();
        void WriteLine(string value);
        void Flush();
    }
}