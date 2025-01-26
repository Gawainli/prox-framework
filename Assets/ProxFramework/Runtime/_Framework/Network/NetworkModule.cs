using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ProxFramework.Network
{
    public class NetworkModule
    {
        private bool _initialized;
        private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
        
        public static TcpClient CreateTcpClient()
        {
            var tcpClient = new TcpClient(new DefaultPkgEncoder(), new DefaultPkgDecoder());
            tcpClients.Add(tcpClient);
            return tcpClient;
        }
        
        public void Initialize()
        {
            _initialized = true;
        }

        public void Tick(float deltaTime)
        {
            if (_initialized)
            {
                foreach (var client in tcpClients)
                {
                    client.Update(deltaTime);
                }
            }
        }

        public void Shutdown()
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