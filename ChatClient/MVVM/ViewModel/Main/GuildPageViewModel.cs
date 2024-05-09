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
        public ChatContext Context { get; }

        [ObservableProperty]
        private Guild _guild;

        [ObservableProperty]
        private TextChannelPage? _textChannelContent;

        public GuildPageViewModel(ChatContext context, Guild guild) {
            Context = context;
            _guild = guild;

            if (Context.CurrentUser != null) {
                foreach (var category in guild.Categories) {
                    category.UpdateVisibility(Context.CurrentUser.ID);
                    foreach (var textChannel in category.TextChannels) {
                        textChannel.UpdateVisibility(Context.CurrentUser.ID);
                    }
                }
            }

        }


        [RelayCommand]
        private void SelectTextChannel(ulong id) {
            foreach (Category category in Guild.Categories) {
                bool success = false;
                foreach (TextChannel textChannel in category.TextChannels) {
                    if (textChannel.ID == id) {
                        TextChannelContent = new TextChannelPage(Context, textChannel);
                        success = true;
                        break;
                    }
                }
                if (success) { break; }
            }
        }

        [RelayCommand]
        private void CreateCategory(Guild guild) {
            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.CreateCategoryPage = new CreateCategoryPage(Context, guild);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.CreateCategoryPage;
                mainPage.ViewModel.MaskVisibility = Visibility.Visible;
            }
        }

        [RelayCommand]
        private void CreateTextChannel(Category category) {
            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.CreateTextChannelPage = new CreateTextChannelPage(Context, category);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.CreateTextChannelPage;
                mainPage.ViewModel.MaskVisibility = Visibility.Visible;
            }
        }
    }
}
