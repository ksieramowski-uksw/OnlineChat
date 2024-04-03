

namespace ChatShared.Models {
    public class Message {
        public ulong Id { get; set; }
        public ulong ChannelId { get; set; }
        public User Author { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }

        public Message(ulong id, ulong channelId, User author, string content, DateTime dateTime) {
            Id = id;
            ChannelId = channelId;
            Author = author;
            Content = content;
            TimeStamp = dateTime;
        }
    }
}
