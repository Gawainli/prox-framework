using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProxFramework.Network
{
    public class TcpTestChannel
    {
        private System.Net.Sockets.TcpClient _tcpClient;
        private ConcurrentQueue<DefaultNetPackage> _sendQueue = new ConcurrentQueue<DefaultNetPackage>();
        private ConcurrentQueue<DefaultNetPackage> _receiveQueue = new ConcurrentQueue<DefaultNetPackage>();
        
        private readonly RingBuffer _encodeBuffer = new RingBuffer(DefaultNetPackage.PkgMaxSize * 4);
        private readonly RingBuffer _decodeBuffer = new RingBuffer(DefaultNetPackage.PkgMaxSize * 4);
        
        private INetPackageEncoder _encoder;
        private INetPackageDecoder _decoder;

        public TcpTestChannel(string address, int port)
        {
            _tcpClient = new System.Net.Sockets.TcpClient();
            _tcpClient.ConnectAsync(address, port);
        }

        private async void SendProcess()
        {
            var stream = _tcpClient.GetStream();
            while (_tcpClient.Connected && _sendQueue.Count > 0)
            {
                if (_sendQueue.TryDequeue(out var pkg))
                {
                    if (_encodeBuffer.WriteableBytes < DefaultNetPackage.PkgMaxSize)
                    {
                        break;
                    }

                    _encoder.Encode(_encodeBuffer, pkg);
                }
            }

            if (_encodeBuffer.ReadableBytes > 0)
            {
                await stream.WriteAsync(_encodeBuffer.ReadBytes(_encodeBuffer.ReadableBytes), 0,
                    _encodeBuffer.ReadableBytes);
            }
        }

        private async void ReceiveProcess()
        {
            var stream = _tcpClient.GetStream();
            var buffer = new byte[8196];
            var tempPackages = new List<INetPackage>();
            while (_tcpClient.Connected)
            {
                if (!stream.DataAvailable) continue;
                var recvBytesCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (recvBytesCount == 0 || !_decodeBuffer.IsWriteable(recvBytesCount))
                {
                    continue;
                }
                
                _decodeBuffer.WriteBytes(buffer, 0, recvBytesCount);
                _decoder.Decode(_decodeBuffer, tempPackages);
                foreach (var pkg in tempPackages)
                {
                    _receiveQueue.Enqueue((DefaultNetPackage)pkg);
                }
            }
        }
    }
}