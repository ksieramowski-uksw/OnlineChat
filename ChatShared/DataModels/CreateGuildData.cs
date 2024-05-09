

using ChatShared.Models.Privileges;

namespace ChatShared.DataModels {
    public class CreateGuildData {
        public ulong OwnerID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public byte[] Icon { get; set; }
        public GuildPrivilege DefaultPrivilege { get; set; }

        public CreateGuildData(ulong ownerID, string name, string password, byte[] icon,
            GuildPrivilege defaultPrivilege) {
            OwnerID = ownerID;
            Name = name;
            Password = password;
            Icon = icon;
            DefaultPrivilege = defaultPrivilege;
        }
    }
}
