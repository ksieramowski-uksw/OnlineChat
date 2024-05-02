using ChatClient.MVVM.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace ChatClient.MVVM.ViewModel {
    public partial class LoginPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _feedback;


        public LoginPageViewModel(ChatContext context) {
            Context = context;

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
            Context.Client.LogIn(Email, Password);
        }

        [RelayCommand]
        private void NavigateToRegisterPage() {
            if (Context.App.Navigation.LoginWindow is LoginWindow loginWindow) {
                Context.App.Navigation.RegisterPage ??= new RegisterPage(Context);
                loginWindow.MainFrame.Navigate(Context.App.Navigation.RegisterPage);
            }
        }


    }
}
