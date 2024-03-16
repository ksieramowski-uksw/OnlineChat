using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.Network
{
    public class ServerConnection
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _ip;

        public ServerConnection()
        {
            string address = Client.Config.ServerConfig.IPv4;
            int port = Client.Config.ServerConfig.Port;
            _ip = new(IPAddress.Parse(address), port);
            _socket = new(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            if (_socket.Connected) { return; }
            _socket.Connect(_ip);
        }

        public async void Send(OperationCode opCode, string message)
        {
            MemoryStream content = new();
            content.WriteByte((byte)opCode);
            content.Write(Encoding.ASCII.GetBytes(message));
            content.Write(Encoding.ASCII.GetBytes(Client.Config.ServerConfig.EOM));
            _ = await _socket.SendAsync(content.ToArray());
        }
    }
}
