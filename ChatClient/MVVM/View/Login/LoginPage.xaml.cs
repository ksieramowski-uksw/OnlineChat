using ChatClient.MVVM.ViewModel;
using System.Windows.Controls;


namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page {
        public LoginPageViewModel ViewModel { get; }

        public LoginPage(ChatContext context) {
            InitializeComponent();
            ViewModel = new(context);
            DataContext = ViewModel;
        }
    }
}
