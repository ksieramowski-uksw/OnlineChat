using CommunityToolkit.Mvvm.ComponentModel;


namespace ChatShared.Models {
    public partial class MessageAttachment : ObservableObject {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private ID _messageID;

        [ObservableProperty]
        private byte[] _content;


        public MessageAttachment(ID id, ID messageID, byte[] content) {
            ID = id;
            MessageID = messageID;
            Content = content;
        }
    }
}
