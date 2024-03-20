using System.Windows;
using System.Windows.Controls;
using ChatClient.MVVM.View;
using ChatClient.MVVM.ViewModel;
using ChatClient.Network;
using ChatClient.Stores;


namespace ChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static Client Client { get; private set; }
        private readonly NavigationStore _navigationStore;

        App() {
            Client = new Client();


            

            //LoginPage loginPage = new();
            //RegisterPage registerPage = new();
            //
            //Frame frame = new();
            //LoginWindow loginWindow = new();
            //loginWindow.DataContext = new LoginPageViewModel(loginWindow, loginPage, registerPage);
            //
            //MainWindow = loginWindow;
            //loginWindow.MainFrame = frame;
            //frame.Navigate(loginPage);
            //loginWindow.Show();
        }

        protected override void OnStartup(StartupEventArgs e) {
            Client.Connect();


            LoginWindow loginWindow = new();
            LoginPage loginPage = new();
            RegisterPage registerPage = new();
            loginPage.DataContext = new LoginPageViewModel(loginWindow, registerPage);
            registerPage.DataContext = new RegisterPageViewModel(loginWindow, loginPage);
            //registerPage.DataContext = new RegisterPageViewM

            loginWindow.MainFrame.Content = loginPage;
            MainWindow = loginWindow;
            MainWindow.Show();



            base.OnStartup(e);
        }

    }

}
