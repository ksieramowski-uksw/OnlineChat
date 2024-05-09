using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class CreateTextChannelPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _newTextChannelName;

        [ObservableProperty]
        private string _feedback;

        [ObservableProperty]
        private TextChannelPrivilege _privilege;

        private Category _category;

        public CreateTextChannelPageViewModel(ChatContext context, Category category) {
            Context = context;

            Title = $"Create new text channel in category {category.Name}";
            NewTextChannelName = string.Empty;
            Feedback = string.Empty;
            _category = category;
            Privilege = new(0, 0, category.ID);
        }

        [RelayCommand]
        private void CreateTextChannel() {
            Context.Client.CreateTextChannel(_category.ID, NewTextChannelName, Privilege);
        }

        #region Privilege

        #region ViewChannel
        [RelayCommand]
        private void SetPrivilegeValue_ViewChannel_Positive_() {
            Privilege.ViewChannel = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewChannel_Neutral_() {
            Privilege.ViewChannel = PrivilegeValue.Neutral;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewChannel_Negative_() {
            Privilege.ViewChannel = PrivilegeValue.Negative;
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
        private void SetPrivilegeValue_UpdateChannel_Neutral_() {
            Privilege.UpdateChannel = PrivilegeValue.Neutral;
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
        private void SetPrivilegeValue_DeleteChannel_Neutral_() {
            Privilege.DeleteChannel = PrivilegeValue.Neutral;
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
        private void SetPrivilegeValue_Read_Neutral_() {
            Privilege.Read = PrivilegeValue.Neutral;
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
        private void SetPrivilegeValue_Write_Neutral_() {
            Privilege.Write = PrivilegeValue.Neutral;
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
