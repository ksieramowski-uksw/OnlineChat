using ChatClient.MVVM.ViewModel.Main.Popup;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main.Popup {
    /// <summary>
    /// Interaction logic for NotificationPage.xaml
    /// </summary>
    public partial class NotificationPage : Page {
        public NotificationPageViewModel ViewModel { get; }


        public NotificationPage(ChatContext context, string message) {
            InitializeComponent();
            ViewModel = new NotificationPageViewModel(context, message);
            DataContext = ViewModel;
        }


        private void HandleClickEvent(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }

        public static bool Show(ChatContext context, string message) {
            if (context.App.Navigation.MainPage is MainPage mainPage) {
                NotificationPage notificationPage = new(context, message);
                mainPage.ViewModel.PopupContent = notificationPage;
                mainPage.ViewModel.ShowMask();
                return true;
            }
            return false;
        }
    }
}
