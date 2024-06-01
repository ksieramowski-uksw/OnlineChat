using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatClient.MVVM.ViewModel.Main;


namespace ChatClient.Stores {
    public class NavigationStore {
        public LoginWindow? LoginWindow { get; set; }
        public LoginPage? LoginPage { get; set; }
        public RegisterPage? RegisterPage { get; set; }


        public MainWindow? MainWindow { get; set; }
        public MainPage? MainPage { get; set; }
        public GuildPage? GuildPage { get; set; }
        public TextChannelPage? TextChannelPage { get; set; }


        public CreateOrJoinGuildPage? CreateOrJoinGuildPage { get; set; }
        public FinalizeGuildCreationPage? FinalizeGuildCreationPage { get; set; }
        public UpdateGuildPage? UpdateGuildPage { get; set; }

        public CreateCategoryPage? CreateCategoryPage { get; set; }
        public UpdateCategoryPage? UpdateCategoryPage { get; set; }

        public CreateTextChannelPage? CreateTextChannelPage { get; set; }
        public UpdateTextChannelPage? UpdateTextChannelPage { get; set; }

        public UpdateUserPage? UpdateUserPage { get; set; }


        public DeleteConfirmationPage? DeleteConfirmationPage { get; set; }

        public NavigationStore() {
            
        }

    }
}
