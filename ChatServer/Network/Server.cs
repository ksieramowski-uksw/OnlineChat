using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Config;


namespace ChatServer.Network {
    public class Server {
        public ServerConfig Config { get; private set; }

        private readonly List<ClientConnection> _clients;

        private readonly Socket _listener;
        private readonly IPEndPoint _ip;
        private readonly IPAddress _address;

        public Database Database { get; private set; }

        public Server(ServerConfig config) {
            Config = config;

            _clients = new List<ClientConnection>();

            _address = IPAddress.Parse(config.IPv4);
            _ip = new IPEndPoint(_address, Config.Port);
            _listener = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Database = new Database();
            Database.Connect();
        }

        public void Start() {
            _listener.Bind(_ip);
            _listener.Listen(Config.MaxQueueSize);

            Logger.Info($"Server listening at [{_ip.AddressFamily}: {_ip.Address}:{_ip.Port}]");

            AcceptNewClients();
        }

        private void AcceptNewClients() {
            while (true) {
                Socket? client = _listener.Accept();
                if (client == null) {
                    Logger.Warning("Newly accepted client is null.");
                    continue;
                }
                Logger.Info($"Accepted new client: {client.RemoteEndPoint?.ToString()}");
                HandleClientConnection(client);
            }
        }

        private void HandleClientConnection(Socket socket) {
            ClientConnection client = new(this, socket, Config.MaxMessageLength);
            _clients.Add(client);
            Task.Run(client.HandleConnection);
        }
    }
}
