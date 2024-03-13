using ChatClient.Net;
using Client;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private LoginWindow? _window;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _window ??= (Application.Current.MainWindow as LoginWindow);
            _window?.LoadRegisterPage();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{ email: \"");
            builder.Append(Login_EmailTextBox.Text);
            builder.Append("\", password: \"");
            builder.Append(Login_PasswordTextBox.Text);
            builder.Append("\" }");
            Server.SendMessageToServer(builder.ToString());

            _window ??= (Application.Current.MainWindow as LoginWindow);
            if (_window != null)
            {
                Application.Current.MainWindow = new MainWindow();
                Application.Current.MainWindow.Show();
                _window.Close();
            }
        }

    }
}
