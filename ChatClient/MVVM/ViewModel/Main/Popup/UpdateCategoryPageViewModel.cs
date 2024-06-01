using ChatShared.Models.Privileges;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using ChatShared;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ChatClient.MVVM.View.Main.Popup;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class UpdateCategoryPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private Category _category;

        [ObservableProperty]
        private string _categoryName;

        [ObservableProperty]
        private ObservableCollection<PrivilegedUser<CategoryPrivilege>>? _targets;

        [ObservableProperty]
        private PrivilegedUser<CategoryPrivilege>? _selectedTarget;

        [ObservableProperty]
        private string _feedback;

        public string Title {
            get {
                return $"Properties of category\n'{Category.Name}'";
            }
        }


        public UpdateCategoryPageViewModel(ChatContext context, Category category) {
            Context = context;

            Category = category;
            CategoryName = new string(category.Name);
            InitializeTargets();

            Feedback = string.Empty;
        }


        private void InitializeTargets() {
            if (Category.DefaultPrivilege == null || Category.Privileges == null || Category.Users == null) { return; }

            Targets = new ObservableCollection<PrivilegedUser<CategoryPrivilege>>();

            User defaultUser = new(0, string.Empty, "Default", string.Empty, DateTime.MinValue, null, UserStatus.Offline);
            SelectedTarget = new PrivilegedUser<CategoryPrivilege>(defaultUser, new CategoryPrivilege(Category.DefaultPrivilege));
            
            Targets.Add(SelectedTarget);

            foreach (CategoryPrivilege privilege in Category.Privileges) {
                if (Category.Guild != null && privilege.UserID == Category.Guild.OwnerID) { continue; }
                foreach (User user in Category.Users) {
                    if (privilege.UserID == user.ID) {
                        Targets.Add(new PrivilegedUser<CategoryPrivilege>(user, new CategoryPrivilege(privilege)));
                        break;
                    }
                }
            }
        }


        [RelayCommand]
        private void UpdateCategory() {
            if (Context.CurrentUser == null) { return; }
            if (SelectedTarget == null) { return; }
            if (Category.DefaultPrivilege == null) { return; }
            if (Category.Privileges == null) { return; }

            bool privilegeChanged = false;
            if (SelectedTarget.Privilege.UserID == 0) {
                if (!Category.DefaultPrivilege.HasEqualValue(SelectedTarget.Privilege)) {
                    privilegeChanged = true;
                }
            }
            else {
                foreach (var p in Category.Privileges) {
                    if (p.UserID == SelectedTarget.Privilege.UserID) {
                        if (!p.HasEqualValue(SelectedTarget.Privilege)) {
                            privilegeChanged = true;
                        }
                        break;
                    }
                }
            }

            if (privilegeChanged) {
                GuildPrivilege? guildPrivilege = Category.Guild?.GetPrivilege(Context.CurrentUser.ID);
                if (guildPrivilege == null) { return; }
                if (guildPrivilege.ManagePrivileges != PrivilegeValue.Positive) {
                    NotificationPage.Show(Context, "You don't have permission to\nmanage privileges.");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(CategoryName)) {
                Feedback = "Please, fill all fields.";
                return;
            }

            Context.Client.UpdateCategory(Category.ID, CategoryName.ToUpper(), SelectedTarget.Privilege);
        }


        #region Privilege

        #region ViewCategory
        [RelayCommand]
        private void SetPrivilegeValue_ViewCategory_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ViewCategory = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewCategory_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ViewCategory = PrivilegeValue.Neutral;
        }

        [RelayCommand]
        private void SetPrivilegeValue_ViewCategory_Negative_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.ViewCategory = PrivilegeValue.Negative;
        }
        #endregion

        #region UpdateCategory
        [RelayCommand]
        private void SetPrivilegeValue_UpdateCategory_Positive_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateCategory = PrivilegeValue.Positive;
        }

        [RelayCommand]
        private void SetPrivilegeValue_UpdateCategory_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.UpdateCategory = PrivilegeValue.Neutral;
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
        private void SetPrivilegeValue_DeleteCategory_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.DeleteCategory = PrivilegeValue.Neutral;
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
        private void SetPrivilegeValue_CreateChannel_Neutral_() {
            if (SelectedTarget == null) { return; }
            SelectedTarget.Privilege.CreateChannel = PrivilegeValue.Neutral;
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
