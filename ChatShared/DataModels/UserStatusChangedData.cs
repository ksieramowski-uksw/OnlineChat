

namespace ChatShared.DataModels {
    public class UserStatusChangedData {
        public ID UserID { get; set; }
        public UserStatus Status { get; set; }

        public UserStatusChangedData(ID userID, UserStatus status) {
            UserID = userID;
            Status = status;
        }
    }
}
