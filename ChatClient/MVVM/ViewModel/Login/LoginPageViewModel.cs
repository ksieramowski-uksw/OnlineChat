using ChatClient.MVVM.View;
using ChatClient.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace ChatClient.MVVM.ViewModel {
    public partial class LoginPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _feedback;


        public LoginPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;

            Email = string.Empty;
            Password = string.Empty;
            Feedback = "Enter your login details.";
        }


        [RelayCommand]
        private void LogIn() {
            if (string.IsNullOrWhiteSpace(Email) ||string.IsNullOrWhiteSpace(Password)) {
                Feedback = "Please, fill all fields marked with '*'.\"";
                return;
            }
            App.Current.Client.LogIn(Email, Password);
        }

        [RelayCommand]
        private void NavigateToRegisterPage() {
            if (NavigationStore.LoginWindow is LoginWindow loginWindow) {
                NavigationStore.RegisterPage ??= new RegisterPage(NavigationStore);
                loginWindow.MainFrame.Navigate(NavigationStore.RegisterPage);
            }
        }


    }
}
