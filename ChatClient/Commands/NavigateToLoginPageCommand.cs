using ChatClient.MVVM.View;
using ChatClient.MVVM.ViewModel;


namespace ChatClient.Commands {
    public class NavigateToLoginPageCommand : CommandBase {
        private readonly RegisterPageViewModel _viewModel;

        public override void Execute(object? parameter) {
            (_viewModel.LoginWindow as LoginWindow).MainFrame.Navigate(_viewModel.LoginPage);
        }

        public NavigateToLoginPageCommand(RegisterPageViewModel viewModel) {
            _viewModel = viewModel;
        }
    }
}
