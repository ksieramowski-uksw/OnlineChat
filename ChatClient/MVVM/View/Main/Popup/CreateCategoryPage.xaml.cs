using ChatClient.MVVM.ViewModel.Main.Popup;
using ChatClient.Stores;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for CreateCategoryPage.xaml
    /// </summary>
    public partial class CreateCategoryPage : Page {
        public CreateCategoryPageViewModel ViewModel { get; set; }

        public CreateCategoryPage(NavigationStore navigationStore, Guild guild) {
            InitializeComponent();
            ViewModel = new CreateCategoryPageViewModel(navigationStore, guild);
            DataContext = ViewModel;
        }

        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
