using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ProxFramework.Network
{
    public class WebSocketClient : ISocketClient
    {
        public string Address { get; private set; }
        public int Port { get; private set; }
        public string[] SubProtocols { get; private set; }

        public SocketClientState State
        {
            get
            {
                if (_socket == null)
                {
                    return SocketClientState.Disconnected;
                }

                return _socket.State switch
                {
                    WebSocketState.Aborted or WebSocketState.Closed or WebSocketState.None => SocketClientState
                        .Disconnected,
                    WebSocketState.CloseReceived or WebSocketState.CloseSent => SocketClientState.Closing,
                    WebSocketState.Connecting => SocketClientState.Connecting,
                    WebSocketState.Open => SocketClientState.Connected,
                    _ => SocketClientState.Disconnected
                };
            }
        }

        public INetPackageDecoder Decoder { get; private set; }
        public INetPackageEncoder Encoder { get; private set; }

        public event EventHandler<OpenEventArgs> Opened;
        public event EventHandler<CloseEventArgs> Closed;
        public event EventHandler<ErrorEventArgs> Error;

        private ClientWebSocket _socket;
        private CancellationTokenSource _cts;
        private ConcurrentQueue<INetPackage> _sendQueue = new ConcurrentQueue<INetPackage>();
        private ConcurrentQueue<INetPackage> _receiveQueue = new ConcurrentQueue<INetPackage>();

        private RingBuffer _encodeBuffer = new RingBuffer(DefaultNetPackage.PkgMaxSize * 4);
        private RingBuffer _decodeBuffer = new RingBuffer(DefaultNetPackage.PkgMaxSize * 4);

        public WebSocketClient(string address, INetPackageDecoder decoder, INetPackageEncoder encoder)
        {
            Set(address, null, decoder, encoder);
        }

        public WebSocketClient(string address, string subProtocol, INetPackageDecoder decoder,
            INetPackageEncoder encoder)
        {
            Set(address, new[] { subProtocol }, decoder, encoder);
        }

        public WebSocketClient(string address, string[] subProtocols, INetPackageDecoder decoder,
            INetPackageEncoder encoder)
        {
            Set(address, subProtocols, decoder, encoder);
        }

        private void Set(string address, string[] subProtocols, INetPackageDecoder decoder,
            INetPackageEncoder encoder)
        {
            Address = address;
            Port = -1;
            SubProtocols = subProtocols;
            Decoder = decoder;
            Encoder = encoder;
        }

        public void Dispose()
        {
        }

        public async void ConnectAsync()
        {
            if (_socket != null)
            {
                return;
            }

            _socket = new ClientWebSocket();
            _cts = new CancellationTokenSource();

            if (SubProtocols != null)
            {
                foreach (var subProtocol in SubProtocols)
                {
                    _socket.Options.AddSubProtocol(subProtocol);
                }
            }

            try
            {
                await _socket.ConnectAsync(new Uri(Address), _cts.Token);
            }
            catch (Exception e)
            {
                return;
            }

            Opened?.Invoke(this, new OpenEventArgs());
        }

        public void CloseAsync()
        {
            throw new NotImplementedException();
        }

        public void Send(INetPackage pkg)
        {
            if (_socket == null || _socket.State != WebSocketState.Open)
            {
                return;
            }

            _sendQueue.Enqueue(pkg);
        }

        public INetPackage PickPkg()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            ProcessSend();
        }

        private async void ProcessSend()
        {
            if (_socket == null || State != SocketClientState.Connected || _sendQueue.Count == 0 ||
                _cts.IsCancellationRequested)
            {
                return;
            }

            while (_sendQueue.Count > 0)
            {
                if (_encodeBuffer.WriteableBytes < WebSocketNetPackage.PkgMaxSize)
                {
                    break;
                }

                if (_sendQueue.TryDequeue(out var pkg))
                {
                    Encoder.Encode(_encodeBuffer, pkg);
                }
            }

            if (_encodeBuffer.ReadableBytes == 0)
            {
                return;
            }

            var sendBytes = _encodeBuffer.ReadBytes(_encodeBuffer.ReadableBytes);
            await _socket.SendAsync(sendBytes, WebSocketMessageType.Binary, true, _cts.Token);
        }

        private void ProcessReceive()
        {
        }
    }
}