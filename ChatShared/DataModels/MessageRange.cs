using ChatShared.Models;


namespace ChatShared.DataModels {
    public class MessageRange {
        public List<Message> Messages { get; set; }
        public int First { get; set; }
        public int Limit { get; set; }
        public bool Last { get; set; }

        public MessageRange() {
            Messages = new List<Message>();
            First = int.MaxValue;
            Limit = 20;
            Last = false;
        }

        public MessageRange(List<Message> messages, int first, int limit, bool last) {
            Messages = messages;
            First = first;
            Limit = limit;
            Last = last;
        }
    }
}
