using System.Windows;
using ChatClient.Network;


namespace ChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static Client Client { get; private set; }
        App() {
            InitializeComponent();

            Client = new Client();
            Client.Connect();
        }

    }

}
