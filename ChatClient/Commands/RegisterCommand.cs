using ChatClient.MVVM.ViewModel;
using System.ComponentModel;


namespace ChatClient.Commands {
    public class RegisterCommand : CommandBase {

        private readonly RegisterPageViewModel _registerPageViewModel;

        public override void Execute(object? parameter) {
            string? email = _registerPageViewModel.Email;
            string? password = _registerPageViewModel.Password;
            string? confirmPassword = _registerPageViewModel.ConfirmPassword;
            string? nickname = _registerPageViewModel.Nickname;

            if (!string.IsNullOrWhiteSpace(password) && password != confirmPassword) {
                string feedback = "Field 'Confirm password' must have same value as 'Password' field.";
                _registerPageViewModel.RegisterFeedback = feedback;
            }
            else {
                if (!string.IsNullOrWhiteSpace(email) &&
                    !string.IsNullOrWhiteSpace(password) &&
                    !string.IsNullOrWhiteSpace(confirmPassword) &&
                    !string.IsNullOrWhiteSpace(nickname)) {
                    App.Client.Register(email, password, confirmPassword, nickname);
                }
                else {
                    string feedback = "Please, fill all fields marked with '*'.";
                    _registerPageViewModel.RegisterFeedback = feedback;
                }
            }

        }

        public RegisterCommand(RegisterPageViewModel registerPageViewModel) {
            _registerPageViewModel = registerPageViewModel;
            _registerPageViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(LoginPageViewModel.Email) ||
                e.PropertyName == nameof(LoginPageViewModel.Password)) {
                OnCanExecuteChanged();
            }
        }

    }
}
