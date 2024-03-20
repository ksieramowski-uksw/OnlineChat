using ChatClient.MVVM.View;
using ChatClient.MVVM.ViewModel;


namespace ChatClient.Commands {
    public class NavigateToRegisterPageCommand : CommandBase {
        private readonly LoginPageViewModel _viewModel;

        public override void Execute(object? parameter) {
            (_viewModel.LoginWindow as LoginWindow).MainFrame.Navigate(_viewModel.RegisterPage);
        }

        public NavigateToRegisterPageCommand(LoginPageViewModel viewModel) {
            _viewModel = viewModel;
        }
    }
}
