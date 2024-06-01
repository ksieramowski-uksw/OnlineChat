using ChatClient.MVVM.ViewModel.Main;
using System.Windows;
using System.Windows.Input;


namespace ChatClient.MVVM.View.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindowViewModel ViewModel { get; set; }


        public MainWindow(ChatContext context) {
            InitializeComponent();

            ViewModel = new MainWindowViewModel(context);
            DataContext = ViewModel;
        }


        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
    }
}
