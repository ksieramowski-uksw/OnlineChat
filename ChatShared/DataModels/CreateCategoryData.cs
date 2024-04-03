

namespace ChatShared.DataModels {
    public class CreateCategoryData {
        public ulong GuildId { get; set; }
        public string Name { get; set; }

        public CreateCategoryData(ulong guildId, string name) {
            GuildId = guildId;
            Name = name;
        }
    }
}
