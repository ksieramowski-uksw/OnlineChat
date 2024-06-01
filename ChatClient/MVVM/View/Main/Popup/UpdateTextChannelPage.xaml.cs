using ChatClient.MVVM.ViewModel.Main.Popup;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for UpdateTextChannelPage.xaml
    /// </summary>
    public partial class UpdateTextChannelPage : Page {
        public UpdateTextChannelPageViewModel ViewModel { get; }


        public UpdateTextChannelPage(ChatContext context, TextChannel textChannel) {
            InitializeComponent();
            ViewModel = new UpdateTextChannelPageViewModel(context, textChannel);
            DataContext = ViewModel;
        }


        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
