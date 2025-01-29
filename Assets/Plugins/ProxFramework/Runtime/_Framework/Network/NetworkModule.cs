using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ProxFramework.Network
{
    public static class NetworkModule
    {
        private static bool _initialized;
        private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
        
        public static TcpClient CreateTcpClient()
        {
            var tcpClient = new TcpClient(new DefaultPkgEncoder(), new DefaultPkgDecoder());
            tcpClients.Add(tcpClient);
            return tcpClient;
        }
        
        public static void Initialize()
        {
            _initialized = true;
        }

        public static void Tick(float deltaTime)
        {
            if (_initialized)
            {
                foreach (var client in tcpClients)
                {
                    client.Update(deltaTime);
                }
            }
        }

        public static void Shutdown()
        {
            foreach (var client in tcpClients)
            {
                client.Dispose();
            }
            tcpClients.Clear();
            _initialized = false;
        }
    }
}