

namespace ChatShared.DataModels {
    public class MessageData {
        public ulong UserID { get; set; }
        public ulong TextChannelID { get; set; }
        public string Text { get; set; }

        public MessageData(ulong userID, ulong textChannelID, string text) {
            UserID = userID;
            TextChannelID = textChannelID;
            Text = text;
        }
    }
}
