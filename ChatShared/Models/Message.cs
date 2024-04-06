

namespace ChatShared.Models {
    public class Message {
        public ulong ID { get; set; }
        public ulong ChannelID { get; set; }
        public User Author { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }

        public Message(ulong id, ulong channelID, User author, string content, DateTime dateTime) {
            ID = id;
            ChannelID = channelID;
            Author = author;
            Content = content;
            TimeStamp = dateTime;
        }
    }
}
