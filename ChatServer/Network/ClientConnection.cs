using ChatClient;
using ChatShared.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ChatServer.Network {
    public partial class ClientConnection {
        private readonly Server _server;
        private readonly Socket _socket;
        private readonly ulong _maxMessageLength;

        public User? User { get; set; }

        public ClientConnection(Server server, Socket socket, ulong maxMessageLength) {
            _server = server;
            _socket = socket;
            _maxMessageLength = maxMessageLength;
        }

        public void HandleConnection() {
            while (true) {
                try {
                    var buffer = new byte[_maxMessageLength];
                    var length = _socket.Receive(buffer, SocketFlags.None);

                    string? content = Encoding.UTF8.GetString(buffer, 0, length);
                    if (string.IsNullOrWhiteSpace(content)) {
                        Logger.Warning("Recieved empty message.");
                        continue;
                    }

                    string[] messages = content.Split("<|EOM|>");

                    ClientOperationHandler handler = new(this);
                    foreach (string msg in messages) {
                        if (string.IsNullOrWhiteSpace(msg)) { continue; }
                        OperationCode opCode = (OperationCode)msg[0];
                        string message = msg[1..];
                        if (opCode == 0 || string.IsNullOrWhiteSpace(message) || message[0] == 0) { continue; }
                        Logger.Message($"{{{opCode}}} | {message}");
                        handler.HandleOperation(opCode, message);
                    }
                }
                catch (Exception) {
                    //Logger.Warning(ex.Message);
                    if (_socket.RemoteEndPoint is IPEndPoint ip) {
                        Logger.Warning($"Client {ip.Address}:{ip.Port} just disconnected.");
                    }
                    if (User != null) {
                        Logger.Info($"User with email \"{User.Email}\" just disconnected.");
                    }
                    break;
                }
            }
        }

        public void Send(OperationCode opCode, string message) {
            MemoryStream content = new();
            content.WriteByte((byte)opCode);
            content.Write(Encoding.ASCII.GetBytes(message));
            content.Write(Encoding.ASCII.GetBytes(_server.Config.EOM));
            _socket.SendAsync(content.ToArray());
        }

        public void UniCast(OperationCode opCode, string message) {
            Send(opCode, message);
        }

        public void MultiCast(OperationCode opCode, string message, Func<ClientConnection, bool> isTarget) {
            foreach (ClientConnection client in _server.Clients) {
                if (isTarget(client)) {
                    Send(opCode, message);
                }
            }
        }
    }
}
