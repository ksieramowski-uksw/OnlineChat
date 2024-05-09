using ChatShared.Models.Privileges;


namespace ChatShared.DataModels {
    public class CreateTextChannelData {
        public ulong CategoryID { get; set; }
        public string Name { get; set; }

        public TextChannelPrivilege DefaultPrivilege { get; set; }

        public CreateTextChannelData(ulong categoryID, string name, TextChannelPrivilege defaultPrivilege) {
            CategoryID = categoryID;
            Name = name;
            DefaultPrivilege = defaultPrivilege;
        }
    }
}
