using ChatServer.Network;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using ChatServer.Config;
using System.Security.Cryptography.X509Certificates;


namespace ChatServer {
    internal class Program {
        //static List<Client> users;
        //static Socket listener;
        //
        static void Main(string[] args) {

            Config.Config config = new();

            Server server = new(config.ServerConfig);
            server.Start();



            //users = new List<Client>();
            //var ipv4 = IPAddress.Parse("127.0.0.1");
            //const int port = 21370;
            //const int maxQueueSize = 100;
            //IPEndPoint ipEndPoint = new(ipv4, port);
            //listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //Console.WriteLine(ipEndPoint.AddressFamily);
            //listener.Bind(ipEndPoint);
            //listener.Listen(maxQueueSize);
            //
            //
            //
            //
            //
            //
            //while (true)
            //{
            //    var client = listener.AcceptTcpClient();
            //
            //    users.Add(new Client(client));
            //    Console.WriteLine("Client has connected!");
            //
            //    BroadcastConnection();
            //}
        }

        static void BroadcastConnection()
        {
            //foreach (var user in users)
            //{
            //    foreach (var usr in users)
            //    {
            //        var broadcastPacket = new PacketBuilder();
            //        broadcastPacket.WriteOpCode(1);
            //        broadcastPacket.WriteMessage(usr.Username);
            //        broadcastPacket.WriteMessage(usr.UID.ToString());
            //        user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            //    }
            //}
        }

        public static void BroadcastMessage(string message)
        {
            //foreach(var user in users)
            //{
            //    var broadcastPacket = new PacketBuilder();
            //    broadcastPacket.WriteOpCode(5);
            //    broadcastPacket.WriteMessage(message);
            //    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            //}
        }


        public static void BroadcastDisconnection(string uid)
        {
            //var disconnectedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            //if (disconnectedUser == null) { return; }
            //
            //users.Remove(disconnectedUser);
            //foreach (var user in users)
            //{
            //    var broadcastPacket = new PacketBuilder();
            //    broadcastPacket.WriteOpCode(10);
            //    broadcastPacket.WriteMessage(uid);
            //    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            //}
            //
            //BroadcastMessage($"[{disconnectedUser.Username}] Disconnected");
        }
    }
}
