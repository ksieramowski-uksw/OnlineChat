using ChatClient;
using System.Net.Sockets;
using System.Text;


namespace ChatServer.Network {
    public partial class ClientConnection {
        private readonly Server _server;
        private readonly Socket _socket;
        private readonly ushort _maxMessageLength;

        public ClientConnection(Server server, Socket socket, ushort maxMessageLength) {
            _server = server;
            _socket = socket;
            _maxMessageLength = maxMessageLength;
        }

        public async Task HandleConnection() {
            while (true) {
                var buffer = new byte[_maxMessageLength];
                var received = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                OperationCode opCode = (OperationCode)buffer[0];
                string? message = Encoding.UTF8.GetString(buffer, 1, received);
                if (string.IsNullOrWhiteSpace(message)) {
                    Logger.Warning("Recieved empty message.");
                    continue;
                }

                string[] messages = message.Split("<|EOM|>");

                ClientOperationHandler handler = new(this);
                foreach (string msg in messages) {
                    if (string.IsNullOrWhiteSpace(msg) || msg.Length < 3) { continue; }
                    Logger.Message($"Received message: \"{msg}\".");
                    handler.HandleOperation(opCode, msg);
                }
            }
        }

        public async Task Send(OperationCode opCode, string message) {
            MemoryStream content = new();
            content.WriteByte((byte)opCode);
            content.Write(Encoding.ASCII.GetBytes(message));
            content.Write(Encoding.ASCII.GetBytes(_server.Config.EOM));
            _ = await _socket.SendAsync(content.ToArray());
        }

        
    }
}
