using ChatClient.Stores;
using ChatShared.DataModels;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class CreateCategoryPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private string _newCategoryName;

        [ObservableProperty]
        private string _feedback;

        private Guild _guild;

        public CreateCategoryPageViewModel(NavigationStore navigationStore, Guild guild) {
            NavigationStore = navigationStore;

            NewCategoryName = string.Empty;
            Feedback = string.Empty;
            _guild = guild;
        }


        [RelayCommand]
        private void CreateCategory() {
            CreateCategoryData data = new(_guild.ID, NewCategoryName);
            string json = JsonSerializer.Serialize(data);
            App.Current.Client.ServerConnection.Send(OperationCode.CreateCategory, json);
        }
    }
}
