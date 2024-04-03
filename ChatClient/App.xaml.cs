using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using ChatClient.MVVM.View;
using ChatClient.MVVM.ViewModel;
using ChatClient.Network;
using ChatClient.Stores;


namespace ChatClient {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public readonly Client Client;
        public NavigationStore NavigationStore { get; }
        public List<byte[]> ResourceStorage { get; }

        public static new App Current {
            get {
                if (Application.Current is App app) {
                    return app;
                }
                else {
                    throw new Exception("Current App in null.");
                }
            }
        }

        App() {
            Client = new Client();
            ResourceStorage = new List<byte[]>();

            NavigationStore = new NavigationStore();
            NavigationStore.LoginWindow = new LoginWindow(NavigationStore);
            if (NavigationStore.LoginWindow != null) {
                MainWindow = NavigationStore.LoginWindow;
            }
        }

        protected override void OnStartup(StartupEventArgs e) {
            Client.Connect();
            MainWindow.Show();

            base.OnStartup(e);
        }

    }

}
