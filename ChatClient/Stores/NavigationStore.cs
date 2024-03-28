using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Stores {
    public class NavigationStore {
        public LoginWindow? LoginWindow { get; set; }
        public LoginPage? LoginPage { get; set; }
        public RegisterPage? RegisterPage { get; set; }


        public MainWindow? MainWindow { get; set; }
        public MainPage? MainPage { get; set; }


        public CreateGuildPage? CreateGuildPage { get; set; }
        public JoinGuildPage? JoinGuildPage { get; set; }
        public CreateOrJoinGuildPage? CreateOrJoinGuildPage { get; set; }





        public NavigationStore() {
            
        }


    }
}
