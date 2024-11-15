using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ProxFramework.Module;

namespace ProxFramework.Network
{
    public class NetworkModule : IModule
    {
        private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
        
        public static TcpClient CreateTcpClient()
        {
            var tcpClient = new TcpClient(new DefaultPkgEncoder(), new DefaultPkgDecoder());
            tcpClients.Add(tcpClient);
            return tcpClient;
        }
        
        public void Initialize(object userData = null)
        {
            Initialized = true;
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
            if (Initialized)
            {
                foreach (var client in tcpClients)
                {
                    client.Update(unscaledDeltaTime);
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
            Initialized = false;
        }

        public int Priority { get; set; }
        public bool Initialized { get; set; }
    }
}