

using System.Collections.ObjectModel;

namespace ChatShared.Models {
    public class Message {
        public ulong ID { get; set; }
        public ulong ChannelID { get; set; }
        public ulong AuthorID { get; set; }
        public User? Author { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public ObservableCollection<MessageAttachment> Attachments { get; set; }


        public Message(ulong id, ulong channelID, ulong authorID, string content, DateTime time) {
            ID = id;
            ChannelID = channelID;
            AuthorID = authorID;
            Content = content;
            Time = time;

            Attachments = new ObservableCollection<MessageAttachment>();
        }
    }
}
