using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public partial class Guild : ObservableObject {

        [ObservableProperty]
        private ID _ID;

        [ObservableProperty]
        private string _publicID;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private ID _ownerID;

        [ObservableProperty]
        private byte[] _icon;

        [ObservableProperty]
        private DateTime _creationTime;

        [ObservableProperty]
        private ObservableCollection<Category>? _categories;

        [ObservableProperty]
        private ObservableCollection<User>? _users;

        [ObservableProperty]
        private ObservableCollection<GuildPrivilege>? _privileges;

        [ObservableProperty]
        private GuildPrivilege? _defaultPrivilege;


        public Guild(ID id, string publicID, string name, ID ownerID, byte[] icon, DateTime creationTime) {
            ID = id;
            PublicID = publicID;
            Name = name;
            OwnerID = ownerID;
            CreationTime = creationTime;
            Icon = icon;
        }


        public GuildPrivilege? GetPrivilege(ID userID) {
            if (Privileges == null) { return null; }

            GuildPrivilege? privilege = null;
            foreach (var p in Privileges) {
                if (p.UserID == userID) {
                    privilege = p;
                    break;
                }
            }

            return privilege;
        }

        public void DisposeDetails() {
            if (Categories != null) {
                foreach (Category category in Categories) {
                    if (category.TextChannels == null) { continue; }
                    foreach (TextChannel textChannel in category.TextChannels) {
                        textChannel.Messages.Clear();
                        textChannel.Users?.Clear();
                        textChannel.Users = null;
                    }
                    category.TextChannels.Clear();
                    category.TextChannels = null;
                    category.Users?.Clear();
                    category.Users = null;
                }
            }

            Categories?.Clear();
            Categories = null;

            DefaultPrivilege = null;

            Privileges?.Clear();
            Privileges = null;

            Users?.Clear();
            Users = null;
        }
    }
}
