using ChatClient.MVVM.ViewModel.Main.Popup;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for UpdateCategoryPage.xaml
    /// </summary>
    public partial class UpdateCategoryPage : Page {
        public UpdateCategoryPageViewModel ViewModel { get; }


        public UpdateCategoryPage(ChatContext context, Category category) {
            InitializeComponent();
            ViewModel = new UpdateCategoryPageViewModel(context, category);
            DataContext = ViewModel;
        }


        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
