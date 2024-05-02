using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;


namespace ChatClient.Stores {
    public class NavigationStore {
        public LoginWindow? LoginWindow { get; set; }
        public LoginPage? LoginPage { get; set; }
        public RegisterPage? RegisterPage { get; set; }


        public MainWindow? MainWindow { get; set; }
        public MainPage? MainPage { get; set; }
        public GuildPage? GuildPage { get; set; }
        



        public CreateOrJoinGuildPage? CreateOrJoinGuildPage { get; set; }

        public CreateCategoryPage? CreateCategoryPage { get; set; }

        public CreateTextChannelPage? CreateTextChannelPage { get; set; }



        public NavigationStore() {
            
        }


    }
}
