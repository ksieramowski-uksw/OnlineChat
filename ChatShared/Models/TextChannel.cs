using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class TextChannel {
        public ulong Id { get; set; }
        public ulong CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public ObservableCollection<Message> Messages { get; set; }

        public TextChannel(ulong id, ulong categoryId, string name, DateTime creationTime) {
            Id = id;
            CategoryId = categoryId;
            Name = name;
            Messages = new ObservableCollection<Message>();
        }
    }
}
