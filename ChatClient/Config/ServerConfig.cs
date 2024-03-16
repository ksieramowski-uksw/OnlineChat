using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.Config
{
    public class ServerConfig
    {


        public string IPv4 { get; set; }
        public int Port { get; set; }
        public string EOM { get; set; }

        public ServerConfig(string ipv4, int port, string eom)
        {
            IPv4 = ipv4;
            Port = port;
            EOM = eom;
        }

        public static ServerConfig Default()
        {
            return new("127.0.0.1", 21370, "<|EOM|>");
        }
    }
}
