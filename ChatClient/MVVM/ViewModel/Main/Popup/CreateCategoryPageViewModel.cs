using ChatClient.MVVM.View.Main;
using ChatShared;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Windows;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class CreateCategoryPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _newCategoryName;

        [ObservableProperty]
        private string _feedback;

        [ObservableProperty]
        private CategoryPrivilege _privilege;

        private Guild _guild;

        public CreateCategoryPageViewModel(ChatContext context, Guild guild) {
            Context = context;

            NewCategoryName = string.Empty;
            Feedback = string.Empty;
            _guild = guild;

            Privilege = new CategoryPrivilege();
        }


        [RelayCommand]
        private void CreateCategory() {
            Context.Client.CreateCategory(_guild.ID, NewCategoryName.ToUpper(), Privilege);
        }


        #region Privilege

        #region ViewCategory
        [RelayCommand]
        private void SetPrivilegeValue_ViewCategory_Positive_() {
            Privilege.ViewCategory = PrivilegeValue.Positive;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewCategory_Neutral_() {
            Privilege.ViewCategory = PrivilegeValue.Neutral;
            OnPropertyChanged(nameof(Privilege));
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewCategory_Negative_() {
            Privilege.ViewCategory = PrivilegeValue.Negative;
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
        private void SetPrivilegeValue_UpdateCategory_Neutral_() {
            Privilege.UpdateCategory = PrivilegeValue.Neutral;
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
        private void SetPrivilegeValue_DeleteCategory_Neutral_() {
            Privilege.DeleteCategory = PrivilegeValue.Neutral;
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
        private void SetPrivilegeValue_CreateChannel_Neutral_() {
            Privilege.CreateChannel = PrivilegeValue.Neutral;
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
