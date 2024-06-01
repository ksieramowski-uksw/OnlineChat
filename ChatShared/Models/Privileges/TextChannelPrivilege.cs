using CommunityToolkit.Mvvm.ComponentModel;


namespace ChatShared.Models.Privileges {
    public partial class TextChannelPrivilege : ObservableObject, IPrivilege {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private ID _userID;

        [ObservableProperty]
        private ID _channelID;

        [ObservableProperty]
        private PrivilegeValue _managePrivileges;

        [ObservableProperty]
        private PrivilegeValue _viewChannel;

        [ObservableProperty]
        private PrivilegeValue _updateChannel;

        [ObservableProperty]
        private PrivilegeValue _deleteChannel;


        [ObservableProperty]
        private PrivilegeValue _read;

        [ObservableProperty]
        private PrivilegeValue _write;


        public TextChannelPrivilege() {
            ID = 0;
            UserID = 0;
            ChannelID = 0;

            ManagePrivileges = PrivilegeValue.Neutral;

            ViewChannel = PrivilegeValue.Positive;
            UpdateChannel = PrivilegeValue.Neutral;
            DeleteChannel = PrivilegeValue.Neutral;

            Read = PrivilegeValue.Neutral;
            Write = PrivilegeValue.Neutral;
        }

        public TextChannelPrivilege(ID id, ID userID, ID channelID) {
            ID = id;
            UserID = userID;
            ChannelID = channelID;

            ManagePrivileges = PrivilegeValue.Neutral;

            ViewChannel = PrivilegeValue.Positive;
            UpdateChannel = PrivilegeValue.Neutral;
            DeleteChannel = PrivilegeValue.Neutral;

            Read = PrivilegeValue.Neutral;
            Write = PrivilegeValue.Neutral;
        }

        public TextChannelPrivilege(TextChannelPrivilege? privilege) {
            if (privilege == null) { return; }

            ID = privilege.ID; 
            UserID = privilege.UserID;
            ChannelID = privilege.ChannelID;

            ManagePrivileges = privilege.ManagePrivileges;

            ViewChannel = privilege.ViewChannel;
            UpdateChannel = privilege.UpdateChannel;
            DeleteChannel = privilege.DeleteChannel;

            Read = privilege.Read;
            Write = privilege.Write;
        }

        public static TextChannelPrivilege OwnerPrivilege(ID ownerID, ID textChannelID) {
            return new TextChannelPrivilege(0, ownerID, textChannelID) {
                ManagePrivileges = PrivilegeValue.Positive,

                UpdateChannel = PrivilegeValue.Positive,
                DeleteChannel = PrivilegeValue.Positive,

                Read = PrivilegeValue.Positive,
                Write = PrivilegeValue.Positive,
                ViewChannel = PrivilegeValue.Positive
            };
        }

        public TextChannelPrivilege Merge(CategoryPrivilege categoryPrivilege) {
            TextChannelPrivilege privilege = new(ID, UserID, ChannelID);

            privilege.ManagePrivileges = (ManagePrivileges == PrivilegeValue.Neutral)
                ? categoryPrivilege.ManagePrivileges : ManagePrivileges;

            privilege.ViewChannel = (categoryPrivilege.ViewCategory == PrivilegeValue.Negative)
                ? PrivilegeValue.Negative : ViewChannel;

            privilege.UpdateChannel = (UpdateChannel == PrivilegeValue.Neutral)
                ? categoryPrivilege.UpdateChannel : UpdateChannel;

            privilege.DeleteChannel = (DeleteChannel == PrivilegeValue.Neutral)
                ? categoryPrivilege.DeleteChannel : DeleteChannel;

            privilege.Read = (Read == PrivilegeValue.Neutral)
                ? categoryPrivilege.Read : Read;

            privilege.Write = (Write == PrivilegeValue.Neutral)
                ? categoryPrivilege.Write : Write;

            return privilege;
        }


        public bool HasEqualValue(TextChannelPrivilege? privilege) {
            if (privilege == null) { return false; }

            if (   privilege.ManagePrivileges != ManagePrivileges
                || privilege.ViewChannel      != ViewChannel
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
