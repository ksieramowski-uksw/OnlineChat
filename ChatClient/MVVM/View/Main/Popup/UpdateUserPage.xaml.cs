using ChatClient.MVVM.ViewModel.Main.Popup;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for UpdateUserPage.xaml
    /// </summary>
    public partial class UpdateUserPage : Page {
        public UpdateUserPageViewModel ViewModel { get; }


        public UpdateUserPage(ChatContext context, User user) {
            InitializeComponent();

            ViewModel = new UpdateUserPageViewModel(context, user);
            DataContext = ViewModel;
        }


        public void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }
    }
}
