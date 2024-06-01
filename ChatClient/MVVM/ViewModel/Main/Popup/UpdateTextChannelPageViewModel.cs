using ChatShared.Models.Privileges;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatShared;
using System.Collections.ObjectModel;
using ChatClient.MVVM.View.Main.Popup;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class UpdateTextChannelPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private TextChannel _textChannel;

        [ObservableProperty]
        private string _textChannelName;

        [ObservableProperty]
        private ObservableCollection<PrivilegedUser<TextChannelPrivilege>>? _targets;

        [ObservableProperty]
        private PrivilegedUser<TextChannelPrivilege>? _selectedTarget;

        [ObservableProperty]
        private string _feedback;

        public string Title {
            get {
                return $"Properties of text channel\n'{TextChannel.Name}'";
            }
        }


        public UpdateTextChannelPageViewModel(ChatContext context, TextChannel textChannel) {
            Context = context;

            TextChannel = textChannel;
            TextChannelName = new string(textChannel.Name);

            InitializeTargets();

            Feedback = string.Empty;
        }


        private void InitializeTargets() {
            if (TextChannel.DefaultPrivilege == null || TextChannel.Privileges == null || TextChannel.Users == null) { return; }

            Targets = new ObservableCollection<PrivilegedUser<TextChannelPrivilege>>();

            User defaultUser = new(0, string.Empty, "Default", string.Empty, DateTime.MinValue, null, UserStatus.Offline);
            SelectedTarget = new PrivilegedUser<TextChannelPrivilege>(defaultUser, new TextChannelPrivilege(TextChannel.DefaultPrivilege));

            Targets.Add(SelectedTarget);

            foreach (TextChannelPrivilege privilege in TextChannel.Privileges) {
                if (TextChannel.Guild != null && privilege.UserID == TextChannel.Guild.OwnerID) { continue; }
                foreach (User user in TextChannel.Users) {
                    if (privilege.UserID == user.ID) {
                        Targets.Add(new PrivilegedUser<TextChannelPrivilege>(user, new TextChannelPrivilege(privilege)));
                        break;
                    }
                }
            }
        }


        [RelayCommand]
        private void UpdateTextChannel() {
            if (Context.CurrentUser == null) { return; }
            if (SelectedTarget == null) { return; }
            if (TextChannel.DefaultPrivilege == null) { return; }
            if (TextChannel.Privileges == null) { return; }

            bool privilegeChanged = false;
            if (SelectedTarget.Privilege.UserID == 0) {
                if (!TextChannel.DefaultPrivilege.HasEqualValue(SelectedTarget.Privilege)) {
                    privilegeChanged = true;
                }
            }
            else {
                foreach (var p in TextChannel.Privileges) {
                    if (p.UserID == SelectedTarget.Privilege.UserID) {
                        if (!p.HasEqualValue(SelectedTarget.Privilege)) {
                            privilegeChanged = true;
                        }
                        break;
                    }
                }
            }

            if (privilegeChanged) {
                GuildPrivilege? guildPrivilege = TextChannel.Guild?.GetPrivilege(Context.CurrentUser.ID);
                if (guildPrivilege == null) { return; }
                if (guildPrivilege.ManagePrivileges != PrivilegeValue.Positive) {
                    NotificationPage.Show(Context, "You don't have permission to\nmanage privileges.");
                    return;
                }
            }


            if (string.IsNullOrWhiteSpace(TextChannelName)) {
                Feedback = "Pease, fill all fields.";
                return;
            }

            Context.Client.UpdateTextChannel(TextChannel.ID, TextChannelName, SelectedTarget.Privilege);
        }


        #region Privilege

        #region ViewChannel
        [RelayCommand]
        private void SetPrivilegeValue_ViewChannel_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ViewChannel = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewChannel_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ViewChannel = PrivilegeValue.Neutral;
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewChannel_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ViewChannel = PrivilegeValue.Negative;
        }
        #endregion

        #region UpdateChannel
        [RelayCommand]
        private void SetPrivilegeValue_UpdateChannel_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateChannel = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_UpdateChannel_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateChannel = PrivilegeValue.Neutral;
        }

        [RelayCommand]
        private void SetPrivilegeValue_UpdateChannel_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateChannel = PrivilegeValue.Negative;
        }
        #endregion

        #region DeleteChannel
        [RelayCommand]
        private void SetPrivilegeValue_DeleteChannel_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.DeleteChannel = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_DeleteChannel_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.DeleteChannel = PrivilegeValue.Neutral;
        }

        [RelayCommand]
        private void SetPrivilegeValue_DeleteChannel_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.DeleteChannel = PrivilegeValue.Negative;
        }
        #endregion

        #region Read
        [RelayCommand]
        private void SetPrivilegeValue_Read_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.Read = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_Read_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.Read = PrivilegeValue.Neutral;
        }

        [RelayCommand]
        private void SetPrivilegeValue_Read_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.Read = PrivilegeValue.Negative;
        }
        #endregion

        #region Write
        [RelayCommand]
        private void SetPrivilegeValue_Write_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.Write = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_Write_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.Write = PrivilegeValue.Neutral;
        }

        [RelayCommand]
        private void SetPrivilegeValue_Write_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.Write = PrivilegeValue.Negative;
        }
        #endregion

        #endregion

    }
}
