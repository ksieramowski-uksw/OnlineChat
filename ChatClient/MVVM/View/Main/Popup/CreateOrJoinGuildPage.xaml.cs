using ChatClient.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;


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
