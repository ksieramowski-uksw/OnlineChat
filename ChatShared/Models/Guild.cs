using System.Collections.ObjectModel;


namespace ChatShared.Models {
    public class Guild {
        public ulong Id { get; set; }
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ulong OwnerId { get; set; }
        public DateTime CreationTime { get; set; }
        public byte[] Icon { get; set; }

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public Guild(ulong id, string publicId, string name, string password, ulong ownerId, DateTime creationTime, byte[] icon) {
            Id = id;
            PublicId = publicId;
            Name = name;
            Password = password;
            OwnerId = ownerId;
            CreationTime = creationTime;
            Icon = icon;
            Categories = new ObservableCollection<Category>();
            Users = new ObservableCollection<User>();
        }
    }
}
