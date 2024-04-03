

namespace ChatShared.DataModels {
    public class JoinGuildData {
        public string PublicId { get; set; }
        public string Password { get; set; }

        public JoinGuildData(string publicId, string password) {
            PublicId = publicId;
            Password = password;
        }
    }
}
