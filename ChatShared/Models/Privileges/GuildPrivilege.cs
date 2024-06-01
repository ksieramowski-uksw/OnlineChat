using CommunityToolkit.Mvvm.ComponentModel;


namespace ChatShared.Models.Privileges {
    public partial class GuildPrivilege : ObservableObject, IPrivilege {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private ID _userID;

        [ObservableProperty]
        private ID _guildID;


        [ObservableProperty]
        private PrivilegeValue _manageGuild;

        [ObservableProperty]
        private PrivilegeValue _managePrivileges;


        [ObservableProperty]
        private PrivilegeValue _createCategory;

        [ObservableProperty]
        private PrivilegeValue _updateCategory;

        [ObservableProperty]
        private PrivilegeValue _deleteCategory;


        [ObservableProperty]
        private PrivilegeValue _createChannel;

        [ObservableProperty]
        private PrivilegeValue _updateChannel;

        [ObservableProperty]
        private PrivilegeValue _deleteChannel;


        [ObservableProperty]
        private PrivilegeValue _read;

        [ObservableProperty]
        private PrivilegeValue _write;


        public GuildPrivilege() {
            ID = 0;
            UserID = 0;
            GuildID = 0;

            ManageGuild = PrivilegeValue.Negative;
            ManagePrivileges = PrivilegeValue.Negative;

            CreateCategory = PrivilegeValue.Negative;
            UpdateCategory = PrivilegeValue.Negative;
            DeleteCategory = PrivilegeValue.Negative;

            CreateChannel = PrivilegeValue.Negative;
            UpdateChannel = PrivilegeValue.Negative;
            DeleteChannel = PrivilegeValue.Negative;

            Read = PrivilegeValue.Positive;
            Write = PrivilegeValue.Positive;
        }

        public GuildPrivilege(ID id, ID userID, ID guildID) {
            ID = id;
            UserID = userID;
            GuildID = guildID;

            ManageGuild = PrivilegeValue.Negative;
            ManagePrivileges = PrivilegeValue.Negative;

            CreateCategory = PrivilegeValue.Negative;
            UpdateCategory = PrivilegeValue.Negative;
            DeleteCategory = PrivilegeValue.Negative;

            CreateChannel = PrivilegeValue.Negative;
            UpdateChannel = PrivilegeValue.Negative;
            DeleteChannel = PrivilegeValue.Negative;

            Read = PrivilegeValue.Positive;
            Write = PrivilegeValue.Positive;
        }

        public GuildPrivilege(GuildPrivilege? privilege) {
            if (privilege == null) { return; }
            ID = privilege.ID;
            UserID = privilege.UserID;
            GuildID = privilege.GuildID;

            ManageGuild = privilege.ManageGuild;
            ManagePrivileges = privilege.ManagePrivileges;

            CreateCategory = privilege.CreateCategory;
            UpdateCategory = privilege.UpdateCategory;
            DeleteCategory = privilege.DeleteCategory;

            CreateChannel = privilege.CreateChannel;
            UpdateChannel = privilege.UpdateChannel;
            DeleteChannel = privilege.DeleteChannel;

            Read = privilege.Read;
            Write = privilege.Write;
        }

        public static GuildPrivilege OwnerPrivilege(ID ownerID, ID guildID) {
            return new GuildPrivilege(0, ownerID, guildID) {
                ManageGuild = PrivilegeValue.Positive,
                ManagePrivileges = PrivilegeValue.Positive,

                CreateCategory = PrivilegeValue.Positive,
                UpdateCategory = PrivilegeValue.Positive,
                DeleteCategory = PrivilegeValue.Positive,

                CreateChannel = PrivilegeValue.Positive,
                UpdateChannel = PrivilegeValue.Positive,
                DeleteChannel = PrivilegeValue.Positive,

                Read = PrivilegeValue.Positive,
                Write = PrivilegeValue.Positive
            };
        }

        public bool HasEqualValue(GuildPrivilege privilege) {
            if (privilege == null) { return false; }

            if (privilege.ManageGuild != ManageGuild
                || privilege.ManagePrivileges != ManagePrivileges
                || privilege.CreateCategory != CreateCategory
                || privilege.UpdateCategory != UpdateCategory
                || privilege.DeleteCategory != DeleteCategory
                || privilege.CreateChannel != CreateChannel
                || privilege.UpdateChannel != UpdateChannel
                || privilege.DeleteChannel != DeleteChannel
                || privilege.Read != Read
                || privilege.Write != Write) {
                return false;
            }
            return true;
        }
    }
}
