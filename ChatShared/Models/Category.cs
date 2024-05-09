using ChatShared.Models.Privileges;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class Category {
        public ulong ID { get; set; }
        public ulong GuildID { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public ObservableCollection<TextChannel> TextChannels { get; set; }
        public ObservableCollection<PrivilegedUser<CategoryPrivilege>> Users { get; set; }
        public CategoryPrivilege? DefaultPrivilege { get; set; }
        public bool Expanded { get; set; }
        public bool Show { get; set; }

        public Category(ulong id, ulong guildID, string name, DateTime creationTime) {
            ID = id;
            GuildID = guildID;
            Name = name;
            CreationTime = creationTime;
            TextChannels = new ObservableCollection<TextChannel>();
            Users = new ObservableCollection<PrivilegedUser<CategoryPrivilege>>();
            Expanded = true;
            Show = true;
        }

        public void UpdateVisibility(ulong userID) {
            foreach (var u in Users) {
                if (u.User.ID == userID) {
                    if (u.FinalPrivilege.ViewCategory == PrivilegeValue.Positive) {
                        Show = true;
                        return;
                    }
                }
            }
            Show = false;
        }
    }
}
