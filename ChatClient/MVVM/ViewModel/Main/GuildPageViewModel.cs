using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatClient.Stores;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class GuildPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private Guild _guild;

        [ObservableProperty]
        private TextChannelPage? _textChannelContent;

        public GuildPageViewModel(NavigationStore navigationStore, Guild guild) {
            NavigationStore = navigationStore;
            _guild = guild;
        }

        [RelayCommand]
        private void SelectTextChannel(ulong id) {
            foreach (Category category in Guild.Categories) {
                bool success = false;
                foreach (TextChannel textChannel in category.TextChannels) {
                    if (textChannel.ID == id) {
                        TextChannelContent = new TextChannelPage(NavigationStore, textChannel);
                        success = true;
                        break;
                    }
                }
                if (success) { break; }
            }
        }

        [RelayCommand]
        private void CreateCategory(Guild guild) {
            if (NavigationStore.MainPage is MainPage mainPage) {
                NavigationStore.CreateCategoryPage = new CreateCategoryPage(NavigationStore, guild);
                mainPage.ViewModel.PopupContent = NavigationStore.CreateCategoryPage;
                mainPage.ViewModel.MaskVisibility = Visibility.Visible;
            }
        }

        [RelayCommand]
        private void CreateTextChannel(Category category) {
            if (NavigationStore.MainPage is MainPage mainPage) {
                NavigationStore.CreateTextChannelPage = new CreateTextChannelPage(NavigationStore, category);
                mainPage.ViewModel.PopupContent = NavigationStore.CreateTextChannelPage;
                mainPage.ViewModel.MaskVisibility = Visibility.Visible;
            }
        }
    }
}
