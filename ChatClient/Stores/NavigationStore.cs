using ChatClient.MVVM.View;
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
        public LoginPageViewModel? LoginPageViewModel { get; set; }

        public RegisterPage? RegisterPage { get; set; }
        public RegisterPageViewModel? RegisterPageViewModel { get; set; }

        public MainWindow? MainWindow { get; set; }


        public NavigationStore() {
            
        }


    }
}
