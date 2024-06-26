﻿using ChatShared;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;


namespace ChatClient.Network {
    public class ServerConnection {
        public readonly Client Client;
        private readonly Socket _socket;
        private readonly IPEndPoint _ip;

        public ServerConnection(Client client) {
            Client = client;
            string address =  Client.Config.Current.IPv4;
            int port = Client.Config.Current.Port;
            _ip = new(IPAddress.Parse(address), port);
            _socket = new(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect() {
            if (_socket.Connected) { return; }
            _socket.Connect(_ip);
            Task.Run(HandleConnection);
        }

        public async void Send(OperationCode opCode, string message) {
            MemoryStream content = new();
            content.WriteByte((byte)opCode);
            content.Write(Encoding.ASCII.GetBytes(message));
            content.Write(Encoding.ASCII.GetBytes(Client.Config.Current.EOM));
            _ = await _socket.SendAsync(content.ToArray());
        }

        public void HandleConnection() {
            while (true) {
                var buffer = new byte[Client.Config.Current.MessageBufferSize];
                var length = _socket.Receive(buffer, SocketFlags.None);
                if (length >= Client.Config.Current.MessageBufferSize) {
                    MessageBox.Show("Message buffer exceeded.");
                }
                string? content = Encoding.UTF8.GetString(buffer, 0, length);
                if (string.IsNullOrWhiteSpace(content)) {
                    continue;
                }

                string[] messages = content.Split("<|EOM|>");

                ServerOperationHandler operationHandler = new(this);
                foreach (string msg in messages) {
                    if (string.IsNullOrWhiteSpace(msg)) { continue; }
                    OperationCode opCode = (OperationCode)msg[0];
                    string message = msg[1..];
                    if (opCode == 0 || string.IsNullOrWhiteSpace(message) || message[0] == 0) { continue; }
                    operationHandler.HandleOperation(opCode, message);
                }
            }
        }
    }
}
