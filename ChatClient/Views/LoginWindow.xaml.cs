using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {
        public LoginPage? LoginPage;
        public RegisterPage? RegisterPage;

        public LoginWindow() {
            InitializeComponent();

            LoadLoginPage();
        }

        public void LoadLoginPage() {
            LoginPage ??= new LoginPage();
            MainFrame.Navigate(LoginPage);
        }

        public void LoadRegisterPage() {
            RegisterPage ??= new RegisterPage();
            MainFrame.Navigate(RegisterPage);
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

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e) {
            if (sender is Button button) {
                button.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0, 0));
            }
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e) {
            if (sender is Button button) {
                button.Background = new SolidColorBrush(Color.FromRgb(34, 34, 34));
            }
        }
    }
}
