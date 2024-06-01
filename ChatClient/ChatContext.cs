using ChatClient.Network;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;


namespace ChatClient {
    public class ChatContext {
        public App App { get; }
        public Client Client { get; }
        public User? CurrentUser { get; set; }
        public ObservableCollection<Guild> Guilds { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public GuildPrivilege? CurrentGuildPrivilege { get; set; }
        public CategoryPrivilege? CurrentCategoryPrivilege { get; set; }
        public TextChannelPrivilege? CurrentTextChannelPrivilege { get; set; }


        public ChatContext(App app) {
            App = app;
            Client = new(this);
            CurrentUser = null;
            Guilds = new ObservableCollection<Guild>();
            Users = new ObservableCollection<User>();
        }

    }
}
