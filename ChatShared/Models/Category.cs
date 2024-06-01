using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public partial class Category : ObservableObject {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private ID _guildID;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private DateTime _creationTime;

        [ObservableProperty]
        private Guild? _guild;

        [ObservableProperty]
        private ObservableCollection<TextChannel>? _textChannels;

        [ObservableProperty]
        private ObservableCollection<User>? _users;

        [ObservableProperty]
        private ObservableCollection<CategoryPrivilege>? _privileges;

        [ObservableProperty]
        private CategoryPrivilege? _defaultPrivilege;


        public Category(ID id, ID guildID, string name, DateTime creationTime) {
            ID = id;
            GuildID = guildID;
            Name = name;
            CreationTime = creationTime;
            TextChannels = new ObservableCollection<TextChannel>();
        }


    }
}
