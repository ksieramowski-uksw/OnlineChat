using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ChatClient.Network {
    public class ServerConnection {
        private readonly Socket _socket;
        private readonly IPEndPoint _ip;

        public ServerConnection(Config.Config config) {
            string address = config.ServerConfig.IPv4;
            int port = config.ServerConfig.Port;
            _ip = new(IPAddress.Parse(address), port);
            _socket = new(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect() {
            if (_socket.Connected) { return; }
            _socket.Connect(_ip);
        }

        public async void Send(OperationCode opCode, string message) {
            MemoryStream content = new();
            content.WriteByte((byte)opCode);
            content.Write(Encoding.ASCII.GetBytes(message));
            content.Write(Encoding.ASCII.GetBytes(App.Client.Config.ServerConfig.EOM));
            _ = await _socket.SendAsync(content.ToArray());
        }
    }
}
