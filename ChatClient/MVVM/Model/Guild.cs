using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.MVVM.Model {
    public class Guild {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public Guild(ulong id, string name) {
            Id = id;
            Name = name;
            Categories = new ObservableCollection<Category>();
            Users = new ObservableCollection<User>();
        }
    }
}
