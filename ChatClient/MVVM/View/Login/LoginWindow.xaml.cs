using ChatClient.MVVM.ViewModel.Login;
using System.Windows;
using System.Windows.Input;


namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {
        public LoginWindowViewModel ViewModel { get; }

        public LoginWindow(ChatContext context) {
            InitializeComponent();
            ViewModel = new LoginWindowViewModel(context);
            context.App.Navigation.LoginPage = new LoginPage(context);
            ViewModel.Navigate(context.App.Navigation.LoginPage);
            DataContext = ViewModel;
        }

        private void LoginWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
    }
}
