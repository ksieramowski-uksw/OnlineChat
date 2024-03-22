using ChatClient.MVVM.View;
using ChatClient.MVVM.ViewModel;


namespace ChatClient.Commands {
    public class NavigateToRegisterPageCommand : CommandBase {
        private readonly LoginPageViewModel _viewModel;

        public override void Execute(object? parameter) {
            if (_viewModel.NavigationStore.LoginWindow is LoginWindow loginWindow) {
                loginWindow.MainFrame.Navigate(_viewModel.NavigationStore.RegisterPage);
            }
        }

        public NavigateToRegisterPageCommand(LoginPageViewModel viewModel) {
            _viewModel = viewModel;
        }
    }
}
