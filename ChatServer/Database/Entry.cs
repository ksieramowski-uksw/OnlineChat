

namespace ChatServer.Database {
    public class Entry {
        public string Key { get; set; }
        public object Value { get; set; }

        public Entry(string key, object value) {
            Key = key;
            Value = value;
        }
    }
}
