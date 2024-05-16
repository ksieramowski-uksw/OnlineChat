using ChatClient.MVVM.ViewModel.Main.Popup;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for FinalizeGuildCreationPage.xaml
    /// </summary>
    public partial class FinalizeGuildCreationPage : Page {
        FinalizeGuildCreationPageViewModel ViewModel { get; }

        public FinalizeGuildCreationPage(ChatContext context) {
            InitializeComponent();
            ViewModel = new FinalizeGuildCreationPageViewModel(context);
            DataContext = ViewModel;
        }

        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
