using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = Register_EmailTextBox.Text;
            string password = Register_PasswordTextBox.Text;
            string confirmPassword = Register_ConfirmPasswordTextBox.Text;
            string nickname = Register_NicknameTextBox.Text;
            Client.Register(email, password, confirmPassword, nickname);

            //StringBuilder builder = new StringBuilder();
            //builder.Append("{ email: \"");
            //builder.Append(Register_EmailTextBox.Text);
            //builder.Append("\", password: \"");
            //builder.Append(Register_PasswordTextBox.Text);
            //builder.Append("\", password again: \"");
            //builder.Append(Register_ConfirmPasswordTextBox.Text);
            //builder.Append("\", nickname: \"");
            //builder.Append(Register_NicknameTextBox.Text);
            //builder.Append("\" }");
            //Server.SendMessageToServer(builder.ToString());

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow? window = (Application.Current.MainWindow as LoginWindow);
            window?.LoadLoginPage();
        }
    }
}
