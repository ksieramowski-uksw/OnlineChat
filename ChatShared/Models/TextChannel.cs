using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class TextChannel {
        public ulong ID { get; set; }
        public ulong CategoryID { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public ObservableCollection<TextChannelPrivilege> Privileges { get; set; } 

        public TextChannel(ulong id, ulong categoryID, string name, DateTime creationTime) {
            ID = id;
            CategoryID = categoryID;
            Name = name;
            CreationTime = creationTime;
            Messages = new ObservableCollection<Message>();
            Privileges = new ObservableCollection<TextChannelPrivilege>();
        }
    }
}
