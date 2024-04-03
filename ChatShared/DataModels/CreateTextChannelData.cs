

namespace ChatShared.DataModels {
    public class CreateTextChannelData {
        public ulong CategoryId { get; set; }
        public string Name { get; set; }

        public CreateTextChannelData(ulong categoryId, string name) {
            CategoryId = categoryId;
            Name = name;
        }
    }
}
