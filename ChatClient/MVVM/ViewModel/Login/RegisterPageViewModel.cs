using ChatClient.MVVM.View;
using ChatClient.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;


namespace ChatClient.MVVM.ViewModel {
    public partial class RegisterPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private string _nickname;

        [ObservableProperty]
        private string _feedback;

        public RegisterPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;

            Email = string.Empty;
            Password = string.Empty;
            Nickname = string.Empty;
            ConfirmPassword = string.Empty;
            Feedback = "Enter your account details.";
        }

        [RelayCommand]
        private void Register() {
            if (!string.IsNullOrWhiteSpace(Password) && Password != ConfirmPassword) {
                Feedback = "Field 'Confirm password' must have same value as 'Password' field.";
                return;
            }
            if (!string.IsNullOrWhiteSpace(Email) &&
                !string.IsNullOrWhiteSpace(Password) &&
                !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                !string.IsNullOrWhiteSpace(Nickname)) {
                App.Current.Client.Register(Email, Password, ConfirmPassword, Nickname);
            }
            else {
                Feedback = "Please, fill all fields marked with '*'.";
            }
        }

        [RelayCommand]
        private void NavigateToLoginPage() {
            if (NavigationStore.LoginWindow is LoginWindow loginWindow) {
                NavigationStore.LoginPage ??= new LoginPage(NavigationStore);
                loginWindow.MainFrame.Navigate(NavigationStore.LoginPage);
            }
        }



    }
}
