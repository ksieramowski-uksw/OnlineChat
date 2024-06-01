using ChatShared.Models.Privileges;


namespace ChatShared.DataModels {
    public class UpdateGuildData {
        public ID ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public byte[]? Icon { get; set; }
        public GuildPrivilege? Privilege { get; set; }

        public UpdateGuildData() {
            ID = 0;
            Name = string.Empty;
            Password = string.Empty;
            Icon = null;
            Privilege = new GuildPrivilege();
        }

        public UpdateGuildData(ID guildID, string name, string password, byte[]? icon, GuildPrivilege? privilege) {
            ID = guildID;
            Name = name;
            Password = password;
            Icon = icon;
            Privilege = privilege;
        }
    }
}
