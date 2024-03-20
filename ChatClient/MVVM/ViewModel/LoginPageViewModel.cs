using ChatClient.Commands;
using ChatClient.MVVM.View;
using System.Windows;
using System.Windows.Input;


namespace ChatClient.MVVM.ViewModel {
    public class LoginPageViewModel : ViewModelBase {
        public readonly Window LoginWindow;
        public readonly RegisterPage RegisterPage;

        private string? _email;
        public string? Email {
            get {
                return _email;
            }
            set {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string? _password;
        public string? Password {
            get {
                return _password;
            }
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string? _loginFeedback;
        public string? LoginFeedback {
            get {
                return _loginFeedback;
            }
            set {
                _loginFeedback = value;
                OnPropertyChanged(nameof(LoginFeedback));
            }
        }

        public ICommand LogInCommand { get; }
        public ICommand NavigateToRegistrationCommand { get; }


        public LoginPageViewModel(Window loginWindow, RegisterPage registerPage) {
            LoginWindow = loginWindow;
            RegisterPage = registerPage;

            LogInCommand = new LoginCommand(this);
            NavigateToRegistrationCommand = new NavigateToRegisterPageCommand(this);
            LoginFeedback = "Enter your login details.";
        }
    }
}
