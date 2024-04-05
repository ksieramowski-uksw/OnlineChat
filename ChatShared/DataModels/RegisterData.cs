

namespace ChatShared.DataModels {
    public class RegisterData {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Nickname { get; set; }
        public string Pronoun { get; set; }
        public byte[] ProfilePicture { get; set; }

        public RegisterData(string email, string password, string confirmPassword, string nickname,
            string pronoun, byte[] profilePicture) {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
            Nickname = nickname;
            Pronoun = pronoun;
            ProfilePicture = profilePicture;
        }
    }
}
