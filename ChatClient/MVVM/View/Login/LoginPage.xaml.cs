using ChatClient.MVVM.ViewModel;
using ChatClient.Stores;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page {
        public LoginPageViewModel ViewModel { get; }

        public LoginPage(NavigationStore navigationStore) {
            InitializeComponent();
            ViewModel = new(navigationStore);
            DataContext = ViewModel;
        }
    }
}
