

namespace ChatShared.Models {
    public class MessageAttachment {
        public ulong ID { get; set; }
        public ulong MessageID { get; set; }
        public byte[] Content { get; set; }

        public MessageAttachment(ulong id, ulong messageID, byte[] content) {
            ID = id;
            MessageID = messageID;
            Content = content;
        }
    }
}
