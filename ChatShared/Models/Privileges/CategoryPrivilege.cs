

namespace ChatShared.Models.Privileges {
    public class CategoryPrivilege {
        public ulong ID { get; set; }
        public ulong UserID { get; set; }
        public ulong CategoryID { get; set; }

        public PrivilegeValue UpdateCategory { get; set; }
        public PrivilegeValue DeleteCategory { get; set; }

        public PrivilegeValue CreateChannel { get; set; }
        public PrivilegeValue UpdateChannel { get; set; }
        public PrivilegeValue DeleteChannel { get; set; }
        public PrivilegeValue ViewCategory { get; set; }

        public PrivilegeValue Read { get; set; }
        public PrivilegeValue Write { get; set; }

        public CategoryPrivilege(ulong id, ulong userID, ulong categoryID) {
            ID = id;
            UserID = userID;
            CategoryID = categoryID;

            UpdateCategory = PrivilegeValue.Neutral;
            DeleteChannel = PrivilegeValue.Neutral;

            CreateChannel = PrivilegeValue.Neutral;
            UpdateChannel = PrivilegeValue.Neutral;
            DeleteChannel = PrivilegeValue.Neutral;
            ViewCategory = PrivilegeValue.Positive;

            Read = PrivilegeValue.Neutral;
            Write = PrivilegeValue.Neutral;
        }
    }
}
