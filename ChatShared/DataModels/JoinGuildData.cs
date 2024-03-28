using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DataModels {
    public class JoinGuildData {
        public string Name { get; set; }
        public string Password { get; set; }

        public JoinGuildData(string name, string password) {
            Name = name;
            Password = password;
        }
    }
}
