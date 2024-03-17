

namespace ChatShared.DataModels {
    public class ClientData {
        public ulong Id { get; set; }
        public string Nickname { get; set; }

        public ClientData(ulong id, string nickname) {
            Id = id;
            Nickname = nickname;
        }
    }
}
