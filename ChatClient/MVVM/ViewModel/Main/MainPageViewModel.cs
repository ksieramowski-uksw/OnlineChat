using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatClient.Stores;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ChatClient.MVVM.ViewModel {
    public partial class MainPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private Visibility _maskVisibility;

        [ObservableProperty]
        private BitmapImage? _currentUserProfilePicture;

        [ObservableProperty]
        private string _currentUserNickname;

        [ObservableProperty]
        private string _currentUserStatus;

        [ObservableProperty]
        private object? _popupContent;

        [ObservableProperty]
        private object? _guildContent;

        [ObservableProperty]
        private Guild? _selectedGuild;
        public ObservableCollection<Guild> Guilds { get; set; }

        private Dictionary<string, GuildPage> _guildPageCache { get; }

        public MainPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;

            Guilds = new ObservableCollection<Guild>();
            _guildPageCache = new Dictionary<string, GuildPage>();

            MaskVisibility = Visibility.Hidden;
            

            if (App.Current.Client.User is User user) {
                _currentUserProfilePicture = ResourceHelper.ByteArrayToBitmapImage(user.ProfilePicture);
                _currentUserNickname = user.Nickname;
                _currentUserStatus = user.Status.ToString();

                App.Current.Client.ServerConnection.Send(OperationCode.GetGuildsForUser, user.ID.ToString());


                //Guilds.Add(new Guild(ulong.MaxValue, "PUBLIC_ID_OF_DEFAUL_GUILD", "DUPA", "dupa123", 1, DateTime.Now, user.ProfilePicture));
            }
            else {
                _currentUserProfilePicture = null;
                _currentUserNickname = string.Empty;
                _currentUserStatus = string.Empty;
            }
        }

        [RelayCommand]
        private void HideMask() {
            MaskVisibility = Visibility.Hidden;
        }

        [RelayCommand]
        private void AddGuild() {
            NavigationStore.CreateOrJoinGuildPage = new CreateOrJoinGuildPage(NavigationStore);
            PopupContent = NavigationStore.CreateOrJoinGuildPage;
            MaskVisibility = Visibility.Visible;
        }



        [RelayCommand]
        private void FriendList() {
            
        }

        [RelayCommand]
        private void SelectGuild(ulong id) {
            foreach (Guild guild in Guilds) {
                if (guild.ID == id) {
                    NavigationStore.GuildPage = new GuildPage(NavigationStore, guild);
                    GuildContent = NavigationStore.GuildPage;
                    break;
                }
            }
        }
    }
}
