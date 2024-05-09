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

        public MainPageViewModel(ChatContext context) {
            Context = context;

            Guilds = new ObservableCollection<Guild>();

            MaskVisibility = Visibility.Hidden;
            

            if (Context.CurrentUser is User user) {
                _currentUserProfilePicture = ResourceHelper.ByteArrayToBitmapImage(user.ProfilePicture);
                _currentUserNickname = user.Nickname;
                _currentUserStatus = user.Status.ToString();

                Context.Client.ServerConnection.Send(OperationCode.GetGuildsForUser, user.ID.ToString());


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
            Context.App.Navigation.CreateOrJoinGuildPage = new CreateOrJoinGuildPage(Context);
            PopupContent = Context.App.Navigation.CreateOrJoinGuildPage;
            MaskVisibility = Visibility.Visible;
        }



        [RelayCommand]
        private void FriendList() {
            
        }

        [RelayCommand]
        private void SelectGuild(ulong id) {
            foreach (Guild guild in Guilds) {
                if (guild.ID == id && Context.CurrentUser is User currentUser) {
                    foreach (var category in guild.Categories) {
                        category.UpdateVisibility(currentUser.ID);
                        foreach (var textChannel in category.TextChannels) {
                            textChannel.UpdateVisibility(currentUser.ID);
                        }
                    }
                    Context.App.Navigation.GuildPage = new GuildPage(Context, guild);
                    GuildContent = Context.App.Navigation.GuildPage;
                    break;
                }
            }
        }
    }
}
