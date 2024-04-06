

namespace ChatShared.DataModels {
    public class CreateCategoryData {
        public ulong GuildID { get; set; }
        public string Name { get; set; }

        public CreateCategoryData(ulong guildID, string name) {
            GuildID = guildID;
            Name = name;
        }
    }
}
