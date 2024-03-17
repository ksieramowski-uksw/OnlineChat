using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page {
        private LoginWindow? _window;

        public LoginPage() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            _window ??= (Application.Current.MainWindow as LoginWindow);
            _window?.LoadRegisterPage();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            string email = Login_EmailTextBox.Text;
            string password = Login_PasswordTextBox.Text;
            App.Client.LogIn(email, password);

            //_window ??= (Application.Current.MainWindow as LoginWindow);
            //if (_window != null)
            //{
            //    Application.Current.MainWindow = new MainWindow();
            //    Application.Current.MainWindow.Show();
            //    _window.Close();
            //}
        }

    }
}
