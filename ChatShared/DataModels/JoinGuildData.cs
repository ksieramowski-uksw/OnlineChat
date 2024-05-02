

namespace ChatShared.DataModels {
    public class JoinGuildData {
        public string PublicID { get; set; }
        public string Password { get; set; }
        public ulong UserID { get; set; }

        public JoinGuildData(string publicID, string password, ulong userID) {
            PublicID = publicID;
            Password = password;
            UserID = userID;
        }
    }
}
