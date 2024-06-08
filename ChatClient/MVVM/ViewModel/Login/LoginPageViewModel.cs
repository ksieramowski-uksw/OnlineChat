using ChatClient.MVVM.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel {
    public partial class LoginPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _feedback;


        public LoginPageViewModel(ChatContext context) {
            Context = context;

            Email = string.Empty;
            Feedback = "Enter your login details.";
        }


        [RelayCommand]
        private void LogIn(PasswordBox passwordBox) {
            string password = passwordBox.Password;
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password)) {
                Feedback = "Please, fill all fields marked with '*'.\"";
                return;
            }
            Context.Client.LogIn(Email, password);
        }

        [RelayCommand]
        private void NavigateToRegisterPage() {
            if (Context.App.Navigation.LoginWindow is LoginWindow loginWindow) {
                Context.App.Navigation.RegisterPage ??= new RegisterPage(Context);
                loginWindow.ViewModel.Navigate(Context.App.Navigation.RegisterPage);
            }
        }


    }
}
