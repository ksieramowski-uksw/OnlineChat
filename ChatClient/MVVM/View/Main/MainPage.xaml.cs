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

namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page {
        public MainPageViewModel ViewModel { get; }

        public MainPage(NavigationStore navigationStore) {
            InitializeComponent();
            ViewModel = new MainPageViewModel(navigationStore);
            DataContext = ViewModel;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
            if (sender is Grid grid) {
                grid.Visibility = Visibility.Hidden;
            }
        }

        private void CreateGuildButton_Click(object sender, RoutedEventArgs e) {
            NavigationStore navigationStore = ViewModel.NavigationStore;
            if (navigationStore.MainPage is MainPage mainPage) {
                CreateOrJoinGuildPage createOrJoinGuildPage = new(navigationStore);
                mainPage.MainPagePopupFrame.Content = createOrJoinGuildPage;
                mainPage.MainPageMaskGrid.Visibility = Visibility.Visible;
            }
        }


    }
}
