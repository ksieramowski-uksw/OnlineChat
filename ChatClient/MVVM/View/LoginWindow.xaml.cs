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
using System.Windows.Shapes;

namespace ChatClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginPage? LoginPage;
        public RegisterPage? RegisterPage;

        public LoginWindow()
        {
            InitializeComponent();

            LoadLoginPage();
        }

        public void LoadLoginPage()
        {
            LoginPage ??= new LoginPage();
            MainFrame.Navigate(LoginPage);
        }

        public void LoadRegisterPage()
        {
            RegisterPage ??= new();
            MainFrame.Navigate(RegisterPage);
        }

        private void LoginWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("");
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("");
            (sender as Button).Background = new SolidColorBrush(Color.FromRgb(0xFF, 0, 0));
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Button).Background = new SolidColorBrush(Color.FromRgb(34, 34, 34));
        }
    }
}
