using ChatShared.Models;
using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;


namespace ChatShared.DataModels {
    public class GuildDetails {
        public ID GuildID { get; set; }

        public ObservableCollection<User>? Users { get; set; }
        public ObservableCollection<Category>? Categories { get; set; }
        
        public ObservableCollection<GuildPrivilege>? Privileges { get; set; }
        public GuildPrivilege? DefaultPrivilege { get; set; }

        public GuildDetails() {}

    }
}
