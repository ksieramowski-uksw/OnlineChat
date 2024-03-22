using ChatServer;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;


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
            Task.Run(HandleConnection);
        }

        public async void Send(OperationCode opCode, string message) {
            if (App.Current is App app) {
                MemoryStream content = new();
                content.WriteByte((byte)opCode);
                content.Write(Encoding.ASCII.GetBytes(message));
                content.Write(Encoding.ASCII.GetBytes(app.Client.Config.ServerConfig.EOM));
                _ = await _socket.SendAsync(content.ToArray());
            }
            
        }

        public async Task HandleConnection() {
            while (true) {
                const ushort _maxMessageLength = 2048;
                var buffer = new byte[_maxMessageLength];
                var received = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                OperationCode opCode = (OperationCode)buffer[0];
                string? message = Encoding.UTF8.GetString(buffer, 1, received);
                if (string.IsNullOrWhiteSpace(message)) {
                    Logger.Warning("Recieved empty message.");
                    continue;
                }

                string[] messages = message.Split("<|EOM|>");

                ServerOperationHandler operationHandler = new(this);
                foreach (string msg in messages) {
                    if (string.IsNullOrWhiteSpace(msg) || msg.Length < 3) { continue; }
                    Logger.Info($"Received message: \"{msg}\".");
                    operationHandler.HandleOperation(opCode, msg);
                }

            }
        }
    }
}
