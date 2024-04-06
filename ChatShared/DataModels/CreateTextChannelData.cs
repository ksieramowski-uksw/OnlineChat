

namespace ChatShared.DataModels {
    public class CreateTextChannelData {
        public ulong CategoryID { get; set; }
        public string Name { get; set; }

        public CreateTextChannelData(ulong categoryID, string name) {
            CategoryID = categoryID;
            Name = name;
        }
    }
}
