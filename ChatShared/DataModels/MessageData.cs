

namespace ChatShared.DataModels {
    public class MessageData {
        public ID UserID { get; set; }
        public ID TextChannelID { get; set; }
        public string Content { get; set; }

        public MessageData(ID userID, ID textChannelID, string content) {
            UserID = userID;
            TextChannelID = textChannelID;
            Content = content;
        }
    }
}
