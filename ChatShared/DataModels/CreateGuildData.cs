using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DataModels {
    public class CreateGuildData {
        public ulong OwnerId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public byte[] Icon { get; set; }

        public CreateGuildData(ulong ownerId, string name, string password, byte[] icon) {
            OwnerId = ownerId;
            Name = name;
            Password = password;
            Icon = icon;
        }
    }
}
