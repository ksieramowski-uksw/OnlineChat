global using ID = ulong;

using System.Windows;
using ChatClient.MVVM.View;
using ChatClient.Stores;


namespace ChatClient {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {


        public readonly ChatContext Context;
        //public readonly Client Client;
        public NavigationStore Navigation { get; }
        public List<byte[]> ResourceStorage { get; }

        //public static new App Current {
        //    get {
        //        if (Application.Current is App app) {
        //            return app;
        //        }
        //        else {
        //            throw new Exception("Current App in null.");
        //        }
        //    }
        //}

        App() {
            Context = new(this);
            ResourceStorage = new List<byte[]>();

            Navigation = new NavigationStore();
            Navigation.LoginWindow = new LoginWindow(Context);
            if (Navigation.LoginWindow != null) {
                MainWindow = Navigation.LoginWindow;
            }
        }

        protected override void OnStartup(StartupEventArgs e) {
            Context.Client.Connect();
            MainWindow.Show();

            base.OnStartup(e);
        }

    }

}
