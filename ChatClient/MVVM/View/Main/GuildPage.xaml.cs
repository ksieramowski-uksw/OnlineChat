using ChatClient.MVVM.ViewModel.Main;
using ChatClient.Stores;
using ChatShared.Models;
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

namespace ChatClient.MVVM.View.Main {
    /// <summary>
    /// Interaction logic for GuildPage.xaml
    /// </summary>
    public partial class GuildPage : Page {
        public GuildPageViewModel ViewModel { get; }

        public GuildPage(NavigationStore navigationStore, Guild guild) {
            InitializeComponent();

            ViewModel = new GuildPageViewModel(navigationStore, guild);
            DataContext = ViewModel;
        }
    }
}
