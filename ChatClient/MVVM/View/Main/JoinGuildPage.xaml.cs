using ChatClient.MVVM.ViewModel.Main;
using ChatClient.Stores;
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
    /// Interaction logic for JoinGuildPage.xaml
    /// </summary>
    public partial class JoinGuildPage : Page {
        public JoinGuildPageViewModel ViewModel { get; }

        public JoinGuildPage(NavigationStore navigationStore) {
            InitializeComponent();
            ViewModel = new JoinGuildPageViewModel(navigationStore);
            DataContext = ViewModel;
        }
    }
}
