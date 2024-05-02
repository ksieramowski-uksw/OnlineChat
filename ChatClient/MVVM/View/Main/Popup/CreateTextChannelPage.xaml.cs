using ChatClient.MVVM.ViewModel.Main.Popup;
using ChatClient.Stores;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup
{
    /// <summary>
    /// Interaction logic for CreateTextChannelPage.xaml
    /// </summary>
    public partial class CreateTextChannelPage : Page {
        public CreateTextChannelPageViewModel ViewModel { get; }


        public CreateTextChannelPage(NavigationStore navigationStore, Category category) {
            InitializeComponent();
            ViewModel = new CreateTextChannelPageViewModel(navigationStore, category);
            DataContext = ViewModel;
        }

        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
