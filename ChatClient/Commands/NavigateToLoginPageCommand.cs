using ChatClient.MVVM.View;
using ChatClient.MVVM.ViewModel;


namespace ChatClient.Commands {
    public class NavigateToLoginPageCommand : CommandBase {
        private readonly RegisterPageViewModel _viewModel;

        public override void Execute(object? parameter) {
            if (_viewModel.NavigationStore.LoginWindow is LoginWindow loginWindow) {
                loginWindow.MainFrame.Navigate(_viewModel.NavigationStore.LoginPage);
            }
        }

        public NavigateToLoginPageCommand(RegisterPageViewModel viewModel) {
            _viewModel = viewModel;
        }
    }
}
