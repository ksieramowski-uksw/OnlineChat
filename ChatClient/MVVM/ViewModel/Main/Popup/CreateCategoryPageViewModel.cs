using ChatClient.Stores;
using ChatShared.DataModels;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class CreateCategoryPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private string _newCategoryName;

        [ObservableProperty]
        private string _feedback;

        private Guild _guild;

        public CreateCategoryPageViewModel(ChatContext context, Guild guild) {
            Context = context;

            NewCategoryName = string.Empty;
            Feedback = string.Empty;
            _guild = guild;
        }


        [RelayCommand]
        private void CreateCategory() {
            Context.Client.CreateCategory(_guild.ID, NewCategoryName);
        }
    }
}
