using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Network
{
    public class Server
    {
        public List<NetworkClient> _clients;

        private readonly Socket _listener;

        private readonly IPEndPoint _ip;
        private readonly IPAddress _address;
        private readonly ushort _port = 21370;
        private const ushort _maxQueueSize = 100;
        private const ushort _maxMessageLength = 1024;


        public Server(IPAddress address, ushort port)
        {
            _clients = new List<NetworkClient>();

            _address = address;
            _port = port;

            _ip = new IPEndPoint(_address, _port);
            _listener = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            _listener.Bind(_ip);
            _listener.Listen(_maxQueueSize);

            Logger.Info($"Server listening at [{_ip.AddressFamily}: {_ip.Address}:{_ip.Port}]");

            AcceptNewClients();
        }

        private void AcceptNewClients()
        {
            while (true)
            {
                Socket? client = _listener.Accept();
                if (client == null)
                {
                    Logger.Warning("Newly accepted client is null.");
                    continue;
                }
                Logger.Info($"Accepted new client: {client.RemoteEndPoint?.ToString()}");
                HandleClientConnection(client);
            }
        }

        private void HandleClientConnection(Socket socket)
        {
            NetworkClient client = new(socket, _maxMessageLength);
            _clients.Add(client);
            Task.Run(client.HandleConnection);
        }
    }
}
