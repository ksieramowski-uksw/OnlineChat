using ChatClient.MVVM.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace ChatClient.MVVM.ViewModel {
    public partial class RegisterPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private string _nickname;

        [ObservableProperty]
        private string _pronoun;

        [ObservableProperty]
        private string _feedback;

        public RegisterPageViewModel(ChatContext context) {
            Context = context;

            Email = string.Empty;
            Password = string.Empty;
            Nickname = string.Empty;
            Pronoun = string.Empty;
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
                Context.Client.Register(Email, Password, ConfirmPassword, Nickname, Pronoun);
            }
            else {
                Feedback = "Please, fill all fields marked with '*'.";
            }
        }

        [RelayCommand]
        private void NavigateToLoginPage() {
            if (Context.App.Navigation.LoginWindow is LoginWindow loginWindow) {
                Context.App.Navigation.LoginPage ??= new LoginPage(Context);
                loginWindow.ViewModel.Navigate(Context.App.Navigation.LoginPage);
            }
        }



    }
}
