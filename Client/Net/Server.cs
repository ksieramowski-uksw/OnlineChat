using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ChatClient.Net
{
    public static class Server
    {
        static TcpClient client;

        static public PacketReader PacketReader;

        static public event Action connectedEvent;
        static public event Action msgReceivedEvent;
        static public event Action userDisconnectedEvent;

        static Server()
        {
            client = new();
        }

        public static void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 21370);
                PacketReader = new PacketReader(client.GetStream());

                if (!string.IsNullOrWhiteSpace(username))
                {
                    var connectPacket = new IO.PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    client.Client.Send(connectPacket.GetPacketBytes());


                    Task.Run(() => {
                        while (true)
                        {
                            var packet = new PacketBuilder();
                            packet.WriteOpCode(0);
                            packet.WriteMessage($"{username} says: {new Random().Next()}");
                            client.Client.Send(packet.GetPacketBytes());

                            Thread.Sleep(3000);

                        }
                    });
                    
                }

                ReadPackets();
            }
        }


        private static void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectedEvent?.Invoke();
                            break;
                        default:
                            break;
                    };
                }
            });


        }


        public static void SendMessageToServer(string message)
        {
            if (!client.Client.Connected)
            {
                ConnectToServer(new Random().Next().ToString());
            }

            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            client.Client.Send(messagePacket.GetPacketBytes());
        }

    }
}
