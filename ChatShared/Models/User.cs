using CommunityToolkit.Mvvm.ComponentModel;


namespace ChatShared.Models {
    public partial class User : ObservableObject {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private string _publicID;

        [ObservableProperty]
        private string _nickname;

        [ObservableProperty]
        private string _pronoun;

        [ObservableProperty]
        private DateTime _creationTime;

        [ObservableProperty]
        private byte[]? _profilePicture;

        [ObservableProperty]
        private UserStatus _status;


        public User(ID id, string publicID, string nickname, string pronoun,
                    DateTime creationTime, byte[]? profilePicture, UserStatus status) {
            ID = id;
            PublicID = publicID;
            Nickname = nickname;
            Pronoun = pronoun;
            CreationTime = creationTime;
            ProfilePicture = profilePicture;
            Status = status;
        }
    }
}
