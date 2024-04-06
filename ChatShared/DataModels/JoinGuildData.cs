

namespace ChatShared.DataModels {
    public class JoinGuildData {
        public string PublicID { get; set; }
        public string Password { get; set; }

        public JoinGuildData(string publicID, string password) {
            PublicID = publicID;
            Password = password;
        }
    }
}
