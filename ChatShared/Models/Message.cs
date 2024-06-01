using CommunityToolkit.Mvvm.ComponentModel;


namespace ChatShared.Models {
    public partial class Message : ObservableObject {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private ID _channelID;

        [ObservableProperty]
        private ID _authorID;

        [ObservableProperty]
        private User? _author;

        [ObservableProperty]
        private string _content;

        [ObservableProperty]
        private DateTime _time;


        public Message(ID id, ID channelID, ID authorID, string content, DateTime time) {
            ID = id;
            ChannelID = channelID;
            AuthorID = authorID;
            Content = content;
            Time = time;
        }
    }
}
