using ChatClient.MVVM.ViewModel;
using ChatClient.Stores;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for CreateOrJoinGuildPage.xaml
    /// </summary>
    public partial class CreateOrJoinGuildPage : Page {
        public CreateOrJoinGuildPageViewModel ViewModel { get; }

        public CreateOrJoinGuildPage(ChatContext context) {
            InitializeComponent();
            ViewModel = new CreateOrJoinGuildPageViewModel(context);
            DataContext = ViewModel;
        }

        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
