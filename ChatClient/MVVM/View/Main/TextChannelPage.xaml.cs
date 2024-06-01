using ChatClient.MVVM.Model;
using ChatShared.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main {
    /// <summary>
    /// Interaction logic for TextChannelPage.xaml
    /// </summary>
    public partial class TextChannelPage : Page {
        public TextChannelPageViewModel ViewModel { get; }


        public TextChannelPage(ChatContext context, TextChannel textChannel, ObservableCollection<ObservableUser> visualUsers) {
            InitializeComponent();

            ViewModel = new TextChannelPageViewModel(context, textChannel, visualUsers);
            DataContext = ViewModel;
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox) {
                textBox.Height = textBox.LineCount * textBox.FontSize * 1.333 + 24;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            ViewModel.Scroll = scrollViewer;

            // check if the user scrolled to the top
            if (scrollViewer.VerticalOffset == 0 && ViewModel.CanLoadMoreMessages == true) {
                ViewModel.CanLoadMoreMessages = false;
                //scrollViewer.ScrollToVerticalOffset(1);
                ViewModel.LoadMoreMessages(50);
            }
        }


    }
}
