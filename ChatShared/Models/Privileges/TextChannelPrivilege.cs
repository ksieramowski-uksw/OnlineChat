

namespace ChatShared.Models.Privileges {
    public class TextChannelPrivilege {
        public ulong ID { get; set; }
        public ulong UserID { get; set; }
        public ulong ChannelID { get; set; }

        public PrivilegeValue UpdateChannel { get; set; }
        public PrivilegeValue DeleteChannel { get; set; }

        public PrivilegeValue Read { get; set; }
        public PrivilegeValue Write { get; set; }
        public PrivilegeValue ViewChannel { get; set; }

        public TextChannelPrivilege(ulong id, ulong userID, ulong channelID) {
            ID = id;
            UserID = userID;
            ChannelID = channelID;

            UpdateChannel = PrivilegeValue.Neutral;
            DeleteChannel = PrivilegeValue.Neutral;

            Read = PrivilegeValue.Neutral;
            Write = PrivilegeValue.Neutral;
            ViewChannel = PrivilegeValue.Positive;
        }

        public TextChannelPrivilege Merge(CategoryPrivilege categoryPrivilege) {
            TextChannelPrivilege privilege = new(ID, UserID, ChannelID);

            privilege.UpdateChannel = (UpdateChannel == PrivilegeValue.Neutral)
                ? categoryPrivilege.UpdateChannel : UpdateChannel;

            privilege.DeleteChannel = (DeleteChannel == PrivilegeValue.Neutral)
                ? categoryPrivilege.DeleteChannel : DeleteChannel;

            privilege.Read = (Read == PrivilegeValue.Neutral)
                ? categoryPrivilege.Read : Read;

            privilege.Write = (Write == PrivilegeValue.Neutral)
                ? categoryPrivilege.Write : Write;

            privilege.ViewChannel = ViewChannel;

            return privilege;
        }
    }
}
