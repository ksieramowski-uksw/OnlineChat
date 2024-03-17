namespace ChatServer.Config {
    public class ServerConfig {
        public string IPv4 { get; set; }
        public ushort Port { get; set; }
        public string EOM { get; set; }
        public ushort MaxQueueSize { get; set; }
        public ushort MaxMessageLength { get; set; }

        public ServerConfig(string ipv4, ushort port, string eom,
            ushort maxQueueSize, ushort maxMessageLength) {
            IPv4 = ipv4;
            Port = port;
            EOM = eom;
            MaxQueueSize = maxQueueSize;
            MaxMessageLength = maxMessageLength;
        }

        public ServerConfig() {
            IPv4 = "127.0.0.1";
            Port = 21370;
            EOM = "<|EOM|>";
            MaxQueueSize = 100;
            MaxMessageLength = 2048;
        }
    }
}
