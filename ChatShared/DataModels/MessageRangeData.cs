

namespace ChatShared.DataModels {
    public class MessageRangeData {
        public ID ChannelID { get; set; }
        public ID First { get; set; }
        public byte Limit { get; set; }
        public ID UserID { get; set; }

        public MessageRangeData(ID channelID, ID first, byte limit, ID userID) {
            ChannelID = channelID;
            First = first;
            Limit = limit;
            UserID = userID;
        }
    }
}
