using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class Category {
        public ulong ID { get; set; }
        public ulong GuildID { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public ObservableCollection<TextChannel> TextChannels { get; set; }
        public ObservableCollection<CategoryPrivilege> Privileges { get; set; }

        public Category(ulong id, ulong guildID, string name, DateTime creationTime) {
            ID = id;
            GuildID = guildID;
            Name = name;
            CreationTime = creationTime;
            TextChannels = new ObservableCollection<TextChannel>();
            Privileges = new ObservableCollection<CategoryPrivilege>();
        }
    }
}
