using ChatShared.Models.Privileges;


namespace ChatShared.DataModels {
    public class MessageRangeData {
        public ulong ChannelID { get; set; }
        public ulong First { get; set; }
        public byte Limit { get; set; }
        public ulong UserID { get; set; }

        public MessageRangeData(ulong channelID, ulong first, byte limit, ulong userID) {
            ChannelID = channelID;
            First = first;
            Limit = limit;
            UserID = userID;
        }
    }
}
