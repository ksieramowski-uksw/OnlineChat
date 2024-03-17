using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page {
        public RegisterPage() {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            string email = Register_EmailTextBox.Text;
            string password = Register_PasswordTextBox.Text;
            string confirmPassword = Register_ConfirmPasswordTextBox.Text;
            string nickname = Register_NicknameTextBox.Text;
            App.Client.Register(email, password, confirmPassword, nickname);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            if (Application.Current.MainWindow is LoginWindow loginWindow) {
                loginWindow.LoadLoginPage();
            }
        }
    }
}
