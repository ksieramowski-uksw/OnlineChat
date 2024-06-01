using ChatClient.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ChatClient.MVVM.View.Main {
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page {
        public MainPageViewModel ViewModel { get; }


        public MainPage(ChatContext context) {
            InitializeComponent();
            ViewModel = new MainPageViewModel(context);
            DataContext = ViewModel;
        }


        private void MaskGrid_MouseDown(object sender, MouseButtonEventArgs e) {
            ViewModel.HideMask();
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                GuildsScroll.LineUp();
            }
            else {
                GuildsScroll.LineDown();
            }

            e.Handled = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {

        }
    }
}
