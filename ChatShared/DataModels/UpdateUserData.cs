

namespace ChatShared.DataModels {
    public class UpdateUserData {
        public ID UserID { get; set; }
        public string Nickname { get; set; }
        public string Pronoun { get; set; }
        public byte[]? ProfilePicture { get; set; }

        public UpdateUserData() {
            UserID = 0;
            Nickname = string.Empty;
            Pronoun = string.Empty;
            ProfilePicture = null;
        }

        public UpdateUserData(ID userID, string nickname, string pronoun, byte[]? profilePicture) {
            UserID = userID;
            Nickname = nickname;
            Pronoun = pronoun;
            ProfilePicture = profilePicture;
        }
    }
}
