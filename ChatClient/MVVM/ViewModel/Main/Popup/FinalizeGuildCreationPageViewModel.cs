using ChatShared;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class FinalizeGuildCreationPageViewModel : ObservableObject {
        public ChatContext Context { get; set; }

        [ObservableProperty]
        private string _guildName;

        [ObservableProperty]
        private string _guildPassword;

        [ObservableProperty]
        private string _feedback;

        [ObservableProperty]
        private GuildPrivilege _privilege;

        [ObservableProperty]
        private string _iconFilePath;

        [ObservableProperty]
        private string _iconFilePathText;

        public FinalizeGuildCreationPageViewModel(ChatContext context) {
            Context = context;

            GuildName = string.Empty;
            GuildPassword = string.Empty;
            Feedback = string.Empty;
            Privilege = new GuildPrivilege();

            IconFilePath = ResourceHelper.DefaultImage;
            IconFilePathText = "Default";
        }


        [RelayCommand]
        private void CreateGuild() {
            if (!string.IsNullOrWhiteSpace(GuildName)
                && !string.IsNullOrWhiteSpace(GuildPassword)) {
                Context.Client.CreateGuild(GuildName, GuildPassword, Privilege, IconFilePath);
            }
            else {
                Feedback = "Please, fill all fields marked with '*'..";
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
            Privilege.ManageGuild = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_ManageGuild_Negative_() {
            Privilege.ManageGuild = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region ManagePrivileges
        [RelayCommand]
        private void SetPrivilegeValue_ManagePrivileges_Positive_() {
            Privilege.ManagePrivileges = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_ManagePrivileges_Negative_() {
            Privilege.ManagePrivileges = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region CreateCategory
        [RelayCommand]
        private void SetPrivilegeValue_CreateCategory_Positive_() {
            Privilege.CreateCategory = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_CreateCategory_Negative_() {
            Privilege.CreateCategory = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region UpdateCategory
        [RelayCommand]
        private void SetPrivilegeValue_UpdateCategory_Positive_() {
            Privilege.UpdateCategory = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_UpdateCategory_Negative_() {
            Privilege.UpdateCategory = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region DeleteCategory
        [RelayCommand]
        private void SetPrivilegeValue_DeleteCategory_Positive_() {
            Privilege.DeleteCategory = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_DeleteCategory_Negative_() {
            Privilege.DeleteCategory = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region CreateChannel
        [RelayCommand]
        private void SetPrivilegeValue_CreateChannel_Positive_() {
            Privilege.CreateChannel = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_CreateChannel_Negative_() {
            Privilege.CreateChannel = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region UpdateChannel
        [RelayCommand]
        private void SetPrivilegeValue_UpdateChannel_Positive_() {
            Privilege.UpdateChannel = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_UpdateChannel_Negative_() {
            Privilege.UpdateChannel = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region DeleteChannel
        [RelayCommand]
        private void SetPrivilegeValue_DeleteChannel_Positive_() {
            Privilege.DeleteChannel = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_DeleteChannel_Negative_() {
            Privilege.DeleteChannel = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region Read
        [RelayCommand]
        private void SetPrivilegeValue_Read_Positive_() {
            Privilege.Read = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_Read_Negative_() {
            Privilege.Read = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #region Write
        [RelayCommand]
        private void SetPrivilegeValue_Write_Positive_() {
            Privilege.Write = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_Write_Negative_() {
            Privilege.Write = PrivilegeValue.Negative;
            OnPropertyChanged(nameof(Privilege));
        }
        #endregion

        #endregion

    }
}
