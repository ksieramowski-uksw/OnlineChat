

namespace ChatShared.DataModels {
    public class JoinGuildData {
        public ID UserID { get; set; }
        public string PublicID { get; set; }
        public string Password { get; set; }

        public JoinGuildData(ID userID, string publicID, string password) {
            UserID = userID;
            PublicID = publicID;
            Password = password;
        }
    }
}
