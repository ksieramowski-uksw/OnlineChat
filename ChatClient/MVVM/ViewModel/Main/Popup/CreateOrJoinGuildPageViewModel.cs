using CommunityToolkit.Mvvm.Input;
using ChatShared.DataModels;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using ChatClient.Stores;
using System.Windows;
using ChatShared.Models.Privileges;
using ChatClient.MVVM.View.Main.Popup;
using ChatClient.MVVM.View.Main;


namespace ChatClient.MVVM.ViewModel {
    public partial class CreateOrJoinGuildPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _newGuildName;

        [ObservableProperty]
        private string _newGuildPassword;

        [ObservableProperty]
        private string _newGuildFeedback;


        [ObservableProperty]
        private string _existingGuildId;

        [ObservableProperty]
        private string _existingGuildPassword;

        [ObservableProperty]
        private string _existingGuildFeedback;


        public CreateOrJoinGuildPageViewModel(ChatContext context) {
            Context = context;

            NewGuildName = string.Empty;
            NewGuildPassword = string.Empty;
            NewGuildFeedback = string.Empty;
            
            ExistingGuildId = string.Empty;
            ExistingGuildPassword = string.Empty;
            ExistingGuildFeedback = string.Empty;
        }


        [RelayCommand]
        void CreateGuild() {
            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                mainPage.ViewModel.PopupContent = new FinalizeGuildCreationPage(Context);
            }
        }

        [RelayCommand]
        private void JoinGuild() {
            if (!string.IsNullOrWhiteSpace(ExistingGuildId)
                && !string.IsNullOrWhiteSpace(ExistingGuildPassword)) {
                Context.Client.JoinGuild(ExistingGuildId, ExistingGuildPassword);
            }
            else {
                ExistingGuildFeedback = "Please, fill all fields marked with '*'.";
            }
        }


    }
}
