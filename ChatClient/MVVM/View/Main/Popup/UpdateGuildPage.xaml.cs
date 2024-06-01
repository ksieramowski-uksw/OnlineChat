using ChatClient.MVVM.ViewModel.Main.Popup;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for UpdateGuildPage.xaml
    /// </summary>
    public partial class UpdateGuildPage : Page {

        public UpdateGuildPageViewModel ViewModel { get; }


        public UpdateGuildPage(ChatContext context, Guild guild) {
            InitializeComponent();
            ViewModel = new UpdateGuildPageViewModel(context, guild);
            DataContext = ViewModel;
        }


        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
