using ChatShared.Models;


namespace ChatShared.DataModels {
    public class JoiningUser {
        public User? User { get; set; }
        public Guild? Guild { get; set; }
        public string Message { get; set; }

        public JoiningUser() {
            User = null;
            Guild = null;
            Message = string.Empty;
        }

        public JoiningUser(User? user, Guild? guild, string message) {
            User = user;
            Guild = guild;
            Message = message;
        }
    }
}
