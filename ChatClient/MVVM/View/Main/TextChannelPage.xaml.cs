using ChatShared.Models;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main {
    /// <summary>
    /// Interaction logic for TextChannelPage.xaml
    /// </summary>
    public partial class TextChannelPage : Page {
        public TextChannelPageViewModel ViewModel { get; }

        public TextChannelPage(ChatContext context, TextChannel textChannel) {
            InitializeComponent();

            ViewModel = new TextChannelPageViewModel(context, textChannel);
            DataContext = ViewModel;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox) {
                textBox.Height = textBox.LineCount * textBox.FontSize * 1.333 + 24;
            }
        }
    }
}
