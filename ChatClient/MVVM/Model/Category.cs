using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.MVVM.Model {
    public class Category {
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public ObservableCollection<TextChannel> Channels { get; set; }

        public Category(ulong id, ulong guildId, string name) {
            Id = id;
            GuildId = guildId;
            Name = name;
            
        }
    }
}
