using ChatClient.MVVM.ViewModel;
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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page {
        public MainPageViewModel ViewModel { get; }

        public MainPage(NavigationStore navigationStore) {
            InitializeComponent();
            ViewModel = new MainPageViewModel(navigationStore);
            DataContext = ViewModel;
        }

        private void MaskGrid_MouseDown(object sender, MouseButtonEventArgs e) {
            //if (sender is Grid grid) {
            //    grid.Visibility = Visibility.Hidden;
                ViewModel.MaskVisibility = Visibility.Hidden;
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (App.Current.NavigationStore.MainPage is MainPage mainPage) {
                if (mainPage.ViewModel.SelectedGuild is Guild guild) {
                    MessageBox.Show(guild.Name);
                }
                else {
                    MessageBox.Show("null");
                }
                
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            MessageBox.Show("dupa");
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                GuildsScroll.LineUp();
            }
            else {
                GuildsScroll.LineDown();
            }

            e.Handled = true;
        }

        //private void CreateGuildButton_Click(object sender, RoutedEventArgs e) {
        //    NavigationStore navigationStore = ViewModel.NavigationStore;
        //    if (navigationStore.MainPage is MainPage mainPage) {
        //        CreateOrJoinGuildPage createOrJoinGuildPage = new(navigationStore);
        //        mainPage.MainPagePopupFrame.Content = createOrJoinGuildPage;
        //        mainPage.MainPageMaskGrid.Visibility = Visibility.Visible;
        //    }
        //}


    }
}
