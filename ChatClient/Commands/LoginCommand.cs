using ChatClient.MVVM.ViewModel;
using System.ComponentModel;


namespace ChatClient.Commands {
    public class LoginCommand : CommandBase {

        private readonly LoginPageViewModel _loginPageViewModel;

        public override void Execute(object? parameter) {
            string? email = _loginPageViewModel.Email;
            string? password = _loginPageViewModel.Password;
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password)) {
                App.Client.LogIn(email, password);
            }
            else {
                _loginPageViewModel.LoginFeedback = "Please, fill all fields marked with '*'.\"";
            }
        }

        public LoginCommand(LoginPageViewModel loginPageViewModel) {
            _loginPageViewModel = loginPageViewModel;

            _loginPageViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(LoginPageViewModel.Email) ||
                e.PropertyName == nameof(LoginPageViewModel.Password)) {
                OnCanExecuteChanged();
            }
        }
    }
}
