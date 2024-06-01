using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ChatClient.MVVM.ViewModel {
    public partial class MainPageViewModel : ObservableObject {
        public ChatContext Context{ get; }

        [ObservableProperty]
        private Visibility _maskVisibility;

        [ObservableProperty]
        private BitmapImage? _currentUserProfilePicture;

        [ObservableProperty]
        private User? _user;

        [ObservableProperty]
        private object? _popupContent;

        [ObservableProperty]
        private object? _guildContent;

        [ObservableProperty]
        public ObservableCollection<Guild> _guilds;

        public MainPageViewModel(ChatContext context) {
            Context = context;

            Guilds = Context.Guilds;
            MaskVisibility = Visibility.Hidden;

            if (Context.CurrentUser != null) {
                User = Context.CurrentUser;
                UpdateCurrentUserProfilePicture();
                Context.Client.GetGuildsForCurrentUser();
            }
        }

        public void UpdateCurrentUserProfilePicture() {
            if (User != null && User.ProfilePicture != null) {
                CurrentUserProfilePicture = ResourceHelper.ByteArrayToBitmapImage(User.ProfilePicture);
            }
            else {
                CurrentUserProfilePicture = ResourceHelper.ByteArrayToBitmapImage(
                    ResourceHelper.GetScaledImagePixels(ResourceHelper.DefaultImage));
            }
        }

        [RelayCommand]
        public void Navigate(object? content) {
            GuildContent = content;
            HideMask();
        }

        [RelayCommand]
        public void Popup(object? content) {
            PopupContent = content;
            ShowMask();
        }

        [RelayCommand]
        public void ShowMask() {
            MaskVisibility = Visibility.Visible;
        }

        [RelayCommand]
        public void HideMask() {
            MaskVisibility = Visibility.Hidden;
        }

        [RelayCommand]
        private void AddGuild() {
            Context.App.Navigation.CreateOrJoinGuildPage = new CreateOrJoinGuildPage(Context);
            Popup(Context.App.Navigation.CreateOrJoinGuildPage);
        }

        [RelayCommand]
        private void SelectGuild(ID id) {
            foreach (Guild guild in Guilds) {
                if (guild.ID == id) {
                    if (Context.Client.Config.Current.DisposeMessagesOnGuildChanged == true
                        && Context.App.Navigation.GuildPage is GuildPage guildPage) {
                        guildPage.ViewModel.Dispose();
                        GC.Collect();
                    }

                    if (guild.Categories == null || guild.Users == null || guild.Privileges == null || guild.DefaultPrivilege == null) {
                        Context.Client.GetGuildDetails(guild.ID);
                    }
                    else {
                        Context.App.Navigation.GuildPage = new GuildPage(Context, guild);
                        Navigate(Context.App.Navigation.GuildPage);
                    }
                    break;
                }
            }
        }

        [RelayCommand]
        private void UserSettings() {
            if (Context.CurrentUser is User currentUser) {
                Context.App.Navigation.UpdateUserPage = new UpdateUserPage(Context, currentUser);
                Popup(Context.App.Navigation.UpdateUserPage);
            }
        }
    }
}
