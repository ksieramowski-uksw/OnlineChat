using ChatClient.MVVM.ViewModel;
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
    /// Interaction logic for CreateOrJoinGuildPage.xaml
    /// </summary>
    public partial class CreateOrJoinGuildPage : Page {
        public CreateOrJoinGuildPageViewModel ViewModel { get; }

        public CreateOrJoinGuildPage(NavigationStore navigationStore) {
            InitializeComponent();
            ViewModel = new CreateOrJoinGuildPageViewModel(navigationStore);
            DataContext = ViewModel;
        }

        private void HandleMouseEvent(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }
    }
}
