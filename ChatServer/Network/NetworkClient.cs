using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Network
{
    public class NetworkClient
    {
        private Socket _socket;

        private readonly ushort _maxMessageLength;
        

        public NetworkClient(Socket socket, ushort maxMessageSize)
        {
            _socket = socket;
            _maxMessageLength = maxMessageSize;
        }

        public async Task HandleConnection()
        {
            while (true)
            {
                var buffer = new byte[_maxMessageLength];
                var received = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                string? response = Encoding.UTF8.GetString(buffer, 5, received);

                if (string.IsNullOrWhiteSpace(response))
                {
                    Logger.Warning("Recieved empty message.");
                    continue;
                }


                Logger.Info($"Received message: \"{response}\".");
            }
        }
    }
}
