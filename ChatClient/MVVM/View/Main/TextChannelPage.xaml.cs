using ChatClient.Stores;
using ChatShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient.MVVM.ViewModel.Main {
    /// <summary>
    /// Interaction logic for TextChannelPage.xaml
    /// </summary>
    public partial class TextChannelPage : Page {
        public TextChannelPageViewModel ViewModel { get; }

        public TextChannelPage(NavigationStore navigationStore, TextChannel textChannel) {
            InitializeComponent();

            ViewModel = new TextChannelPageViewModel(navigationStore, textChannel);
            DataContext = ViewModel;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox) {
                textBox.Height = textBox.LineCount * textBox.FontSize * 1.333 + 24;
            }
        }
    }
}
