using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ChatClient.Config
{
    public class Config
    {
        private string ServerConfigFileName { get; set; }
        public ServerConfig ServerConfig { get; private set; }

        public Config()
        {
            LoadConfigFileNames();
            LoadConfig();
        }


        private void LoadConfigFileNames()
        {
            ServerConfigFileName = "Config/ServerConfig.json";
        }

        public void LoadConfig()
        {
            LoadServerConfig();
        }

        private bool LoadServerConfig()
        {
            if (string.IsNullOrWhiteSpace(ServerConfigFileName))
            {
                ServerConfig = ServerConfig.Default();
                return false;
            }
            string serverConfigJson = File.ReadAllText(ServerConfigFileName);
            if (string.IsNullOrWhiteSpace(serverConfigJson))
            {
                ServerConfig = ServerConfig.Default();
                return false;
            }
            var serverConfig = JsonSerializer.Deserialize<ServerConfig>(serverConfigJson);
            if (serverConfig == null)
            {
                ServerConfig = ServerConfig.Default();
                return false;
            }
            ServerConfig = serverConfig;
            return true;
        }
    }
}
