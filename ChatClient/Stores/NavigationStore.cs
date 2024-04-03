using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;


namespace ChatClient.Stores {
    public class NavigationStore {
        public LoginWindow? LoginWindow { get; set; }
        public LoginPage? LoginPage { get; set; }
        public RegisterPage? RegisterPage { get; set; }


        public MainWindow? MainWindow { get; set; }
        public MainPage? MainPage { get; set; }
        public CreateOrJoinGuildPage? CreateOrJoinGuildPage { get; set; }





        public NavigationStore() {
            
        }


    }
}
