using ChatClient.MVVM.ViewModel.Main.Popup;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for DeleteConfirmationPage.xaml
    /// </summary>
    public partial class DeleteConfirmationPage : Page {
        public DeleteConfirmationPageViewModel ViewModel { get; }


        public DeleteConfirmationPage(ChatContext context, object? target) {
            InitializeComponent();
            ViewModel = new DeleteConfirmationPageViewModel(context, target);
            DataContext = ViewModel;
        }


        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
