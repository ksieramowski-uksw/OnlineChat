using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class Guild {
        public ulong ID { get; set; }
        public string PublicID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ulong OwnerID { get; set; }
        public DateTime CreationTime { get; set; }
        public byte[] Icon { get; set; }

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<PrivilegedUser<GuildPrivilege>> Users { get; set; }
        //public ObservableCollection<GuildPrivilege> Privileges { get; set; }
        public GuildPrivilege? DefaultPrivilege { get; set; }

        public Guild(ulong id, string publicID, string name, string password, ulong ownerID,
                     DateTime creationTime, byte[] icon) {
            ID = id;
            PublicID = publicID;
            Name = name;
            Password = password;
            OwnerID = ownerID;
            CreationTime = creationTime;
            Icon = icon;
            Categories = new ObservableCollection<Category>();
            Users = new ObservableCollection<PrivilegedUser<GuildPrivilege>>();
            //Privileges = new ObservableCollection<GuildPrivilege>();
        }
    }
}
