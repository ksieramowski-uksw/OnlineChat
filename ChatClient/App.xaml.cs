using System.Diagnostics;
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
        public readonly Client Client;
        public readonly NavigationStore NavigationStore;

        App() {
            Client = new Client();

            LoginWindow loginWindow = new();
            LoginPage loginPage = new();
            RegisterPage registerPage = new();
            MainWindow mainWindow = new();

            NavigationStore = new() {
                LoginWindow = loginWindow,
                LoginPage = loginPage,
                RegisterPage = registerPage,
                MainWindow = mainWindow
            };

            LoginPageViewModel loginPageViewModel = new(NavigationStore);
            loginPage.DataContext = loginPageViewModel;
            RegisterPageViewModel registerPageViewModel = new(NavigationStore);
            registerPage.DataContext = registerPageViewModel;
            loginWindow.MainFrame.Content = loginPage;
            MainWindow = loginWindow;

            NavigationStore.LoginPageViewModel = loginPageViewModel;
            NavigationStore.RegisterPageViewModel = registerPageViewModel;
        }

        protected override void OnStartup(StartupEventArgs e) {
            Client.Connect();
            MainWindow.Show();
            base.OnStartup(e);
        }

    }

}
