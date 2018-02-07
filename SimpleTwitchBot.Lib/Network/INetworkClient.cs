using SimpleTwitchBot.Lib.Events.Network;
using System;
using System.Threading.Tasks;

namespace SimpleTwitchBot.Lib.Network
{
    public interface INetworkClient : IDisposable
    {
        string Hostname { get; }
        int Port { get; }
        bool IsConnected { get; }

        event EventHandler Connected;
        event EventHandler Disconnected;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        Task ConnectAsync();
        void Disconnect();
        void WriteLine(string value);
        void Flush();
    }
}