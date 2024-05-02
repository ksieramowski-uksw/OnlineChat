using ChatServer.Network;


namespace ChatServer {
    internal class Program {
        static void Main(string[] args) {
            Config.Config config = new();
            Server server = new(config.ServerConfig);
            server.Start();
        }
    }
}
