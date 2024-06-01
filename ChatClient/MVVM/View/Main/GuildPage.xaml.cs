using ChatClient.MVVM.ViewModel.Main;
using ChatShared.Models;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.View.Main {
    /// <summary>
    /// Interaction logic for GuildPage.xaml
    /// </summary>
    public partial class GuildPage : Page {
        public GuildPageViewModel ViewModel { get; }


        public GuildPage(ChatContext context, Guild guild) {
            InitializeComponent();

            ViewModel = new GuildPageViewModel(context, guild);
            DataContext = ViewModel;
        }


        private void CategoryProperties_Click(object sender, RoutedEventArgs e) {
            ViewModel.CategoryPropertiesCommand.Execute(((MenuItem)sender).Tag);
        }

        private void CategoryDelete_Click(object sender, RoutedEventArgs e) {
            ViewModel.DeleteCategoryCommand.Execute(((MenuItem)sender).Tag);
        }

        private void TextChannelProperties_Click(object sender, RoutedEventArgs e) {
            ViewModel.TextChannelPropertiesCommand.Execute(((MenuItem)sender).Tag);
        }

        private void TextChannelDelete_Click(object sender, RoutedEventArgs e) {
            ViewModel.DeleteTextChannelCommand.Execute(((MenuItem)sender).Tag);
        }
    }
}
