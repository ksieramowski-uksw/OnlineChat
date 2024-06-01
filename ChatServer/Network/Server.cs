using System.Net.Sockets;
using System.Net;
using ChatServer.Database;


namespace ChatServer.Network
{
    public class Server {
        public Config Config { get; private set; }

        public List<ClientConnection> Clients { get; set; }

        private readonly Socket _listener;
        private readonly IPEndPoint _ip;
        private readonly IPAddress _address;

        public DatabaseConnection Database { get; private set; }

        public Server(Config config) {
            Config = config;

            Clients = new List<ClientConnection>();

            _address = IPAddress.Parse(config.Current.IPv4);
            _ip = new IPEndPoint(_address, Config.Current.Port);
            _listener = new Socket(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Database = new DatabaseConnection();
            Database.Connect();
        }

        public void Start() {
            _listener.Bind(_ip);
            _listener.Listen(Config.Current.MaxQueueSize);

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
            ClientConnection client = new(this, socket, Config.Current.MessageBufferSizePerClient);
            Clients.Add(client);
            Task.Run(client.HandleConnection);
        }
    }
}
