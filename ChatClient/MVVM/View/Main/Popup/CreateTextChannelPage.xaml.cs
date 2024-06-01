using ChatClient.MVVM.ViewModel.Main.Popup;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for CreateTextChannelPage.xaml
    /// </summary>
    public partial class CreateTextChannelPage : Page {
        public CreateTextChannelPageViewModel ViewModel { get; }


        public CreateTextChannelPage(ChatContext context, Category category) {
            InitializeComponent();
            ViewModel = new CreateTextChannelPageViewModel(context, category);
            DataContext = ViewModel;
        }


        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
