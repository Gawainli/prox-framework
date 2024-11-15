using System;

namespace ProxFramework.Network
{
    public class SocketClientEventArgs : EventArgs
    {
        
    }

    public class CloseEventArgs : SocketClientEventArgs
    {
        
    }

    public class ErrorEventArgs : SocketClientEventArgs
    {
        
    }

    public class MessageEventArgs : SocketClientEventArgs
    {
        
    }

    public class OpenEventArgs : SocketClientEventArgs
    {
        
    }
}