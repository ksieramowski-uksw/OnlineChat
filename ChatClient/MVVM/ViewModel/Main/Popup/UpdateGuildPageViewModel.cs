using ChatClient.MVVM.View.Main.Popup;
using ChatShared;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class UpdateGuildPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private Guild _guild;

        [ObservableProperty]
        private string _guildName;

        [ObservableProperty]
        private string _iconFilePath;

        [ObservableProperty]
        private string _iconFilePathText;

        [ObservableProperty]
        private ObservableCollection<PrivilegedUser<GuildPrivilege>>? _targets;

        [ObservableProperty]
        private PrivilegedUser<GuildPrivilege>? _selectedTarget;

        [ObservableProperty]
        private string _feedback;

        public string Title {
            get {
                return $"Properties of server\n'{Guild.Name}'";
            }
        }


        public UpdateGuildPageViewModel(ChatContext context, Guild guild) {
            Context = context;

            Guild = guild;

            GuildName = new string(guild.Name);

            IconFilePath = ResourceHelper.DefaultImage;
            IconFilePathText = "Default";

            InitializeTargets();

            Feedback = string.Empty;
        }


        private void InitializeTargets() {
            if (Guild.DefaultPrivilege == null || Guild.Privileges == null || Guild.Users == null) { return; }

            Targets = new ObservableCollection<PrivilegedUser<GuildPrivilege>>();

            User defaultUser = new(0, string.Empty, "Default", string.Empty, DateTime.MinValue, null, UserStatus.Offline);
            SelectedTarget = new PrivilegedUser<GuildPrivilege>(defaultUser, new GuildPrivilege(Guild.DefaultPrivilege));

            Targets.Add(SelectedTarget);

            foreach (GuildPrivilege privilege in Guild.Privileges) {
                if (privilege.UserID == Guild.OwnerID) { continue; }
                foreach (User user in Guild.Users) {
                    if (privilege.UserID == user.ID) {
                        Targets.Add(new PrivilegedUser<GuildPrivilege>(user, new GuildPrivilege(privilege)));
                        break;
                    }
                }
            }
        }


        [RelayCommand]
        private void UpdateGuild(PasswordBox passwordBox) {
            if (Context.CurrentUser == null) { return; }
            if (SelectedTarget == null) { return; }
            if (Guild.DefaultPrivilege == null) { return; }
            if (Guild.Privileges == null) { return; }

            bool privilegeChanged = false;
            if (SelectedTarget.Privilege.UserID == 0) {
                if (!Guild.DefaultPrivilege.HasEqualValue(SelectedTarget.Privilege)) {
                    privilegeChanged = true;
                }
            }
            else {
                foreach (var p in Guild.Privileges) {
                    if (p.UserID == SelectedTarget.Privilege.UserID) {
                        if (!p.HasEqualValue(SelectedTarget.Privilege)) {
                            privilegeChanged = true;
                        }
                        break;
                    }
                }
            }

            if (privilegeChanged) {
                GuildPrivilege? guildPrivilege = Guild.GetPrivilege(Context.CurrentUser.ID);
                if (guildPrivilege == null) { return; }
                if (guildPrivilege.ManagePrivileges != PrivilegeValue.Positive) {
                    NotificationPage.Show(Context, "You don't have permission to\nmanage privileges.");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(GuildName)) {
                Feedback = "Please, fill required fields.";
                return;
            }

            string password = passwordBox.Password;
            if (IconFilePathText == "Default") {
                Context.Client.UpdateGuild(Guild.ID, GuildName, password, SelectedTarget.Privilege, IconFilePathText);
            }
            else {
                Context.Client.UpdateGuild(Guild.ID, GuildName, password, SelectedTarget.Privilege, IconFilePath);
            }
        }

        [RelayCommand]
        private void SelectIconFile() {
            OpenFileDialog openFileDialog = new() {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff)|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff|All files (*.*)|*.*",
                FilterIndex = 1,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (openFileDialog.ShowDialog() == true) {
                IconFilePath = openFileDialog.FileName;
                IconFilePathText = openFileDialog.FileName;
            }
        }


        #region Privilege

        #region ManageGuild
        [RelayCommand]
        private void SetPrivilegeValue_ManageGuild_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ManageGuild = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_ManageGuild_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ManageGuild = PrivilegeValue.Negative;
        }
        #endregion

        #region ManagePrivileges
        [RelayCommand]
        private void SetPrivilegeValue_ManagePrivileges_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ManagePrivileges = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_ManagePrivileges_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ManagePrivileges = PrivilegeValue.Negative;
        }
        #endregion

        #region CreateCategory
        [RelayCommand]
        private void SetPrivilegeValue_CreateCategory_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.CreateCategory = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_CreateCategory_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.CreateCategory = PrivilegeValue.Negative;
        }
        #endregion

        #region UpdateCategory
        [RelayCommand]
        private void SetPrivilegeValue_UpdateCategory_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateCategory = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_UpdateCategory_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateCategory = PrivilegeValue.Negative;
        }
        #endregion

        #region DeleteCategory
        [RelayCommand]
        private void SetPrivilegeValue_DeleteCategory_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.DeleteCategory = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_DeleteCategory_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.DeleteCategory = PrivilegeValue.Negative;
        }
        #endregion

        #region CreateChannel
        [RelayCommand]
        private void SetPrivilegeValue_CreateChannel_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.CreateChannel = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_CreateChannel_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.CreateChannel = PrivilegeValue.Negative;
        }
        #endregion

        #region UpdateChannel
        [RelayCommand]
        private void SetPrivilegeValue_UpdateChannel_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateChannel = PrivilegeValue.Positive;
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
        private void SetPrivilegeValue_Write_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.Write = PrivilegeValue.Negative;
        }
        #endregion

        #endregion
    }
}
