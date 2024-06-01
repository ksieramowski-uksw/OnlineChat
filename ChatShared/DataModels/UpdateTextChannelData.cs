using ChatShared.Models.Privileges;


namespace ChatShared.DataModels {
    public class UpdateTextChannelData {
        public ID ID { get; set; }
        public string Name { get; set; }

        public TextChannelPrivilege? Privilege { get; set; }

        public UpdateTextChannelData() {
            ID = 0;
            Name = string.Empty;
            Privilege = null;
        }

        public UpdateTextChannelData(ID channelID, string name, TextChannelPrivilege? privilege) {
            ID = channelID;
            Name = name;
            Privilege = privilege;
        }
    }
}
