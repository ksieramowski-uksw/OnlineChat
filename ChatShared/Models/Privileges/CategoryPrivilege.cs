using CommunityToolkit.Mvvm.ComponentModel;


namespace ChatShared.Models.Privileges {
    public partial class CategoryPrivilege : ObservableObject, IPrivilege {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private ID _userID;

        [ObservableProperty]
        private ID _categoryID;

        [ObservableProperty]
        private PrivilegeValue _managePrivileges;

        [ObservableProperty]
        private PrivilegeValue _viewCategory;

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


        public CategoryPrivilege() {
            ID = 0;
            UserID = 0;
            CategoryID = 0;

            ManagePrivileges = PrivilegeValue.Neutral;

            ViewCategory = PrivilegeValue.Positive;
            UpdateCategory = PrivilegeValue.Neutral;
            DeleteCategory = PrivilegeValue.Neutral;

            CreateChannel = PrivilegeValue.Neutral;
            UpdateChannel = PrivilegeValue.Neutral;
            DeleteChannel = PrivilegeValue.Neutral;

            Read = PrivilegeValue.Neutral;
            Write = PrivilegeValue.Neutral;
        }

        public CategoryPrivilege(ID id, ID userID, ID categoryID) {
            ID = id;
            UserID = userID;
            CategoryID = categoryID;

            ManagePrivileges = PrivilegeValue.Neutral;

            ViewCategory = PrivilegeValue.Positive;
            UpdateCategory = PrivilegeValue.Neutral;
            DeleteCategory = PrivilegeValue.Neutral;

            CreateChannel = PrivilegeValue.Neutral;
            UpdateChannel = PrivilegeValue.Neutral;
            DeleteChannel = PrivilegeValue.Neutral;

            Read = PrivilegeValue.Neutral;
            Write = PrivilegeValue.Neutral;
        }

        public CategoryPrivilege(CategoryPrivilege? privilege) {
            if (privilege == null) { return; }

            ID = privilege.ID;
            UserID = privilege.UserID;
            CategoryID = privilege.CategoryID;

            ManagePrivileges = privilege.ManagePrivileges;

            ViewCategory = privilege.ViewCategory;
            UpdateCategory = privilege.UpdateCategory;
            DeleteCategory = privilege.DeleteCategory;

            CreateChannel = privilege.CreateChannel;
            UpdateChannel = privilege.UpdateChannel;
            DeleteChannel = privilege.DeleteChannel;

            Read = privilege.Read;
            Write = privilege.Write;
        }

        public static CategoryPrivilege OwnerPrivilege(ID ownerID, ID categoryID) {
            return new CategoryPrivilege(0, ownerID, categoryID) {
                ManagePrivileges = PrivilegeValue.Positive,

                ViewCategory = PrivilegeValue.Positive,
                UpdateCategory = PrivilegeValue.Positive,
                DeleteCategory = PrivilegeValue.Positive,

                CreateChannel = PrivilegeValue.Positive,
                UpdateChannel = PrivilegeValue.Positive,
                DeleteChannel = PrivilegeValue.Positive,

                Read = PrivilegeValue.Positive,
                Write = PrivilegeValue.Positive,
            };
        }

        public CategoryPrivilege Merge(GuildPrivilege guildPrivilege) {
            CategoryPrivilege privilege = new(ID, UserID, CategoryID);

            privilege.ManagePrivileges = (ManagePrivileges == PrivilegeValue.Neutral)
                ? guildPrivilege.ManagePrivileges : ManagePrivileges;

            privilege.ViewCategory = ViewCategory;

            privilege.UpdateCategory = (UpdateCategory == PrivilegeValue.Neutral)
                ? guildPrivilege.UpdateCategory : UpdateCategory;

            privilege.DeleteCategory = (DeleteCategory == PrivilegeValue.Neutral)
                ? guildPrivilege.DeleteCategory : DeleteCategory;

            privilege.CreateChannel = (CreateChannel == PrivilegeValue.Neutral)
                ? guildPrivilege.CreateChannel : CreateChannel;

            privilege.UpdateChannel = (UpdateChannel == PrivilegeValue.Neutral)
                ? guildPrivilege.UpdateChannel : UpdateChannel;

            privilege.DeleteChannel = (DeleteChannel == PrivilegeValue.Neutral)
                ? guildPrivilege.DeleteChannel : DeleteChannel;

            privilege.Read = (Read == PrivilegeValue.Neutral)
                ? guildPrivilege.Read : Read;

            privilege.Write = (Write == PrivilegeValue.Neutral)
                ? guildPrivilege.Write : Write;

            return privilege;
        }

        public bool HasEqualValue(CategoryPrivilege? privilege) {
            if (privilege == null) { return false; }

            if (   privilege.ManagePrivileges != ManagePrivileges
                || privilege.ViewCategory     != ViewCategory
                || privilege.UpdateCategory   != UpdateCategory
                || privilege.DeleteCategory   != DeleteCategory
                || privilege.CreateChannel    != CreateChannel
                || privilege.UpdateChannel    != UpdateChannel
                || privilege.DeleteChannel    != DeleteChannel
                || privilege.Read             != Read
                || privilege.Write            != Write) {
                return false;
            }
            return true;
        }
    }
}
