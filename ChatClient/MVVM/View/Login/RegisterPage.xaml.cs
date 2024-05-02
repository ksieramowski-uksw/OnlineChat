using ChatClient.MVVM.ViewModel;
using ChatClient.Stores;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View {
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page {
        public RegisterPageViewModel ViewModel { get; }

        public RegisterPage(ChatContext context) {
            InitializeComponent();
            ViewModel = new(context);
            DataContext = ViewModel;
        }
    }
}
