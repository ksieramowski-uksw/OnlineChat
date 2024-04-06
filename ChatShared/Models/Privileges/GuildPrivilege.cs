

namespace ChatShared.Models.Privileges {
    public class GuildPrivilege {
        public ulong ID { get; set; }
        public ulong UserID { get; set; }
        public ulong GuildID { get; set; }

        public PrivilegeValue ManageGuild { get; set; }
        public PrivilegeValue ManagePrivilages { get; set; }

        public PrivilegeValue CreateCategory { get; set; }
        public PrivilegeValue UpdateCategory { get; set; }
        public PrivilegeValue DeleteCategory { get; set; }

        public PrivilegeValue CreateChannel { get; set; }
        public PrivilegeValue UpdateChannel { get; set; }
        public PrivilegeValue DeleteChannel { get; set; }

        public PrivilegeValue Read { get; set; }
        public PrivilegeValue Write { get; set; }


        public GuildPrivilege(ulong id, ulong userID, ulong guildID) {
            ID = id;
            UserID = userID;
            GuildID = guildID;

            ManageGuild = PrivilegeValue.Negative;
            ManagePrivilages = PrivilegeValue.Negative;

            CreateCategory = PrivilegeValue.Negative;
            UpdateCategory = PrivilegeValue.Negative;
            DeleteCategory = PrivilegeValue.Negative;

            CreateChannel = PrivilegeValue.Negative;
            UpdateChannel = PrivilegeValue.Negative;
            DeleteChannel = PrivilegeValue.Negative;

            Read = PrivilegeValue.Negative;
            Write = PrivilegeValue.Negative;
        }

    }
}
