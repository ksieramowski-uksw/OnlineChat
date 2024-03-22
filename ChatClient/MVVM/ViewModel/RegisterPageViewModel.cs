using ChatClient.Commands;
using ChatClient.MVVM.View;
using ChatClient.Stores;
using System.Windows;
using System.Windows.Input;


namespace ChatClient.MVVM.ViewModel {
    public class RegisterPageViewModel : ViewModelBase {

        public readonly NavigationStore NavigationStore;

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

        private string? _confirmPassword;
        public string? ConfirmPassword {
            get {
                return _confirmPassword;
            }
            set {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        private string? _nickname;
        public string? Nickname {
            get {
                return _nickname;
            }
            set {
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }

        private string? _registerFeedback;
        public string? RegisterFeedback {
            get {
                return _registerFeedback;
            }
            set {
                _registerFeedback = value;
                OnPropertyChanged(nameof(RegisterFeedback));
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginPageCommand { get; }


        public RegisterPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;

            RegisterCommand = new RegisterCommand(this);
            NavigateToLoginPageCommand = new NavigateToLoginPageCommand(this);
            RegisterFeedback = "Enter your account details.";
        }
    }
}
