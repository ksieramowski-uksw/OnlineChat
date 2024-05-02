

namespace ChatShared.DataModels {
    public class MessageData {
        public ulong UserID { get; set; }
        public ulong TextChannelID { get; set; }
        public string Content { get; set; }

        public MessageData(ulong userID, ulong textChannelID, string content) {
            UserID = userID;
            TextChannelID = textChannelID;
            Content = content;
        }
    }
}
