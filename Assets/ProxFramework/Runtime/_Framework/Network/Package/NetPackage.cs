using System.Net.WebSockets;

namespace ProxFramework.Network
{
    public interface INetPackage
    {
    }

    public class DefaultNetPackage : INetPackage
    {
        public const int MsgLengthSize = 4;
        public const int MsgIdSize = 4;
        public const int HeaderSize = MsgLengthSize + MsgIdSize;
        public const int BodyMaxSize = 1024 * 1024 * 2;
        public const int PkgMaxSize = HeaderSize + BodyMaxSize;

        public int MsgId { set; get; }
        public byte[] BodyBytes { set; get; }
    }

    public class WebSocketNetPackage : INetPackage
    {
        public const int MsgIdSize = 4;
        public const int PkgMaxSize = 1024 * 1024 * 2;
        public int MsgId { set; get; }
        public byte[] bodyBytes;
    }
}