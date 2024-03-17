

namespace ChatClient.Config {
    public class ServerConfig {
        public string IPv4 { get; set; }
        public ushort Port { get; set; }
        public string EOM { get; set; }

        public ServerConfig(string ipv4, ushort port, string eom) {
            IPv4 = ipv4;
            Port = port;
            EOM = eom;
        }

        public static ServerConfig Default() {
            return new("127.0.0.1", 21370, "<|EOM|>");
        }
    }
}
