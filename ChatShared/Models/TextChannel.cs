using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class TextChannel {
        public ulong ID { get; set; }
        public ulong CategoryID { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public TextChannel? DefaultPrivilege { get; set; }
        public ObservableCollection<PrivilegedUser<TextChannelPrivilege>> Users { get; set; }
        public bool Show { get; set; }

        public TextChannel(ulong id, ulong categoryID, string name, DateTime creationTime) {
            ID = id;
            CategoryID = categoryID;
            Name = name;
            CreationTime = creationTime;
            Messages = new ObservableCollection<Message>();
            Users = new ObservableCollection<PrivilegedUser<TextChannelPrivilege>>();
        }

        public void UpdateVisibility(ulong userID) {
            foreach (var u in Users) {
                if (u.User.ID == userID) {
                    if (u.FinalPrivilege.ViewChannel == PrivilegeValue.Positive) {
                        Show = true;
                        return;
                    }
                }
            }
            Show = false;
        }
    }
}
