using System.Text.Json;

namespace ChatServer.Config {
    public class Config {
        private string? _serverConfigFileName;

        public ServerConfig ServerConfig { get; private set; }

        public Config() {
            LoadConfigFileNames();
            LoadConfig();
        }

        private void LoadConfigFileNames() {
            _serverConfigFileName = "Config/ServerConfig.json";
        }

        public void LoadConfig() {
            LoadServerConfig();
        }

        private bool LoadServerConfig() {
            if (string.IsNullOrWhiteSpace(_serverConfigFileName) || !File.Exists(_serverConfigFileName)) {
                ServerConfig = new ServerConfig();
                return false;
            }
            string serverConfigJson = File.ReadAllText(_serverConfigFileName);
            if (string.IsNullOrWhiteSpace(serverConfigJson)) {
                ServerConfig = new ServerConfig();
                return false;
            }
            var serverConfig = JsonSerializer.Deserialize<ServerConfig>(serverConfigJson);
            if (serverConfig == null) {
                ServerConfig = new ServerConfig();
                return false;
            }
            ServerConfig = serverConfig;
            return true;
        }
    }
}
