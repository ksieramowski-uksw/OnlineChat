

namespace ChatShared.DataModels {
    public class MessageData {
        public ulong UserId { get; set; }
        public ulong TextChannelId { get; set; }
        public string Text { get; set; }

        public MessageData(ulong userId, ulong textChannelId, string text) {
            UserId = userId;
            TextChannelId = textChannelId;
            Text = text;
        }
    }
}
