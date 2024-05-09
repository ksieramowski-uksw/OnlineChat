using ChatClient.Network;
using ChatShared.Models;
using System.Collections.ObjectModel;


namespace ChatClient {
    public class ChatContext {
        public App App { get; }
        public Client Client { get; }
        public User? CurrentUser { get; set; }
        public ObservableCollection<Guild> Guilds { get; set; }

        public ChatContext(App app) {
            App = app;
            Client = new(this);
            CurrentUser = null;
            Guilds = new ObservableCollection<Guild>();
        }

    }
}
