using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class Category {
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public ObservableCollection<TextChannel> Channels { get; set; }

        public Category(ulong id, ulong guildId, string name, DateTime creationTime) {
            Id = id;
            GuildId = guildId;
            Name = name;
            CreationTime = creationTime;
            Channels = new ObservableCollection<TextChannel>();
        }
    }
}
