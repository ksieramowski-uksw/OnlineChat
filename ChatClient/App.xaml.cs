using ChatClient.Config;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>


    public partial class App : Application
    {
        App()
        {
            InitializeComponent();

            Client.Connect();
        }

    }

}
