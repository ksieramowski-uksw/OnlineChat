using ChatServer.Network;


namespace ChatServer {
    internal class Program {
        static void Main(string[] args) {
            Server server = new(new Config());
            server.Start();
        }
    }
}
