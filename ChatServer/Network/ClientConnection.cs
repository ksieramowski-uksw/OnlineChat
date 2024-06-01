using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ChatServer.Network {
    public partial class ClientConnection {
        private readonly Server _server;
        private readonly Socket _socket;
        private readonly int _maxMessageLength;
        private readonly ClientOperationHandler _operationHandler;

        public User? User { get; set; }

        public ClientConnection(Server server, Socket socket, int maxMessageLength) {
            _server = server;
            _socket = socket;
            _maxMessageLength = maxMessageLength;

            _operationHandler = new ClientOperationHandler(this, _server.Database);
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

                    string[] messages = content.Split(_server.Config.Current.EOM);
                    foreach (string msg in messages) {
                        if (string.IsNullOrWhiteSpace(msg)) { continue; }
                        OperationCode opCode = (OperationCode)msg[0];
                        string message = msg[1..];
                        if (opCode == 0 || string.IsNullOrWhiteSpace(message) || message[0] == 0) { continue; }
                        _operationHandler.HandleOperation(opCode, message);
                    }
                }
                catch (Exception ex) {
                    
                    Logger.Warning(ex.Message);
                    if (_socket.RemoteEndPoint is IPEndPoint ip) {
                        Logger.Warning($"Client {ip?.Address}:{ip?.Port} just disconnected.");
                    }
                    if (User != null) {
                        try {
                            _server.Database.Commands.SetUserStatus(User.ID, UserStatus.Offline);
                            List<ID>? targetUsers = _server.Database.Commands.GetKnownUsers(User.ID);
                            UserStatusChangedData data = new(User.ID, UserStatus.Offline);
                            string json = JsonSerializer.Serialize(data);
                            Multicast(OperationCode.UserStatusChanged, json, targetUsers);
                        }
                        catch (Exception ex2) {
                            Logger.Error(ex2, MethodBase.GetCurrentMethod());
                        }
                        Logger.Info($"User with email \"{User.ID}\" just disconnected.");
                    }
                    break;
                }
            }
        }

        private void Send(OperationCode opCode, string message) {
            MemoryStream content = new();
            content.WriteByte((byte)opCode);
            content.Write(Encoding.ASCII.GetBytes(message));
            content.Write(Encoding.ASCII.GetBytes(_server.Config.Current.EOM));
            _socket.SendAsync(content.ToArray());
        }

        public void Unicast(OperationCode opCode, string message) {
            Send(opCode, message);
        }

        public void Multicast(OperationCode opCode, string message, Func<ClientConnection, bool> isTarget) {
            foreach (ClientConnection client in _server.Clients) {
                if (isTarget(client)) {
                    client.Send(opCode, message);
                }
            }
        }

        public void Multicast(OperationCode opCode, string message, List<ID>? targetUsers) {
            if (targetUsers == null) {
                Logger.Error("Multicast failed, target users are NULL.");
                return;
            }

            if (targetUsers.Count == 0) {
                Logger.Warning("Target group of multicast is empty.");
                return;
            }

            foreach (ClientConnection client in _server.Clients) {
                foreach (ID userID in targetUsers) {
                    if (client.User?.ID == userID) {
                        client.Send(opCode, message);
                    }
                }
            }
        }
    }
}
