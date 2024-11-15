using System;

namespace ProxFramework.Network
{
    public enum SocketClientState : ushort
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        Closing = 3,
    }

    public interface ISocketClient : IDisposable
    {
        void ConnectAsync();
        void CloseAsync();
        void Send(INetPackage pkg);
        INetPackage PickPkg();
        void Update();

        string Address { get; }
        int Port { get; }
        string[] SubProtocols { get; }
        SocketClientState State { get; }

        INetPackageDecoder Decoder { get; }
        INetPackageEncoder Encoder { get; }
        
        event EventHandler<OpenEventArgs> Opened; 
        event EventHandler<CloseEventArgs> Closed;
        event EventHandler<ErrorEventArgs> Error;
    }
}