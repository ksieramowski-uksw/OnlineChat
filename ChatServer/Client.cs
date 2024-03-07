using System.Net.Sockets;


namespace ChatServer
{
    public class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        private PacketReader PacketReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
           
            PacketReader = new PacketReader(ClientSocket.GetStream());

            var opcode = PacketReader.ReadByte();
            Username = PacketReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: CLient has connected with the username: {Username}");

            Task.Run(Process);
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = PacketReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: {Username} {msg}");
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID}]: Disconnected!");
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
