using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public partial class TextChannel : ObservableObject {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private ID _categoryID;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private DateTime _creationTime;

        [ObservableProperty]
        private Guild? _guild;

        [ObservableProperty]
        private Category? _category;

        [ObservableProperty]
        private ObservableCollection<Message> _messages;

        [ObservableProperty]
        private TextChannelPrivilege? _defaultPrivilege;

        [ObservableProperty]
        private ObservableCollection<User>? _users;

        [ObservableProperty]
        private ObservableCollection<TextChannelPrivilege>? _privileges;


        public TextChannel(ID id, ID categoryID, string name, DateTime creationTime) {
            ID = id;
            CategoryID = categoryID;
            Name = name;
            CreationTime = creationTime;
            Messages = new ObservableCollection<Message>();
        }
    }
}
