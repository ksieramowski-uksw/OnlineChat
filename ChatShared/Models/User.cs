

namespace ChatShared.Models {
    public class User {
        public ulong Id { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Pronoun { get; set; }
        public DateTime CreationTime { get; set; }
        public byte[] ProfilePicture { get; set; }
        public UserStatus Status { get; set; }

        public User(ulong id, string nickname, string email, string pronoun,
            DateTime creationTime, byte[] profilePicture, UserStatus status) {
            Id = id;
            Nickname = nickname;
            Email = email;
            Pronoun = pronoun;
            CreationTime = creationTime;
            ProfilePicture = profilePicture;
            Status = status;
        }
    }
}
