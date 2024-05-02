using ChatClient.Stores;
using ChatShared.DataModels;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows;


namespace ChatClient.MVVM.ViewModel.Main.Popup {
    public partial class CreateTextChannelPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _newTextChannelName;

        [ObservableProperty]
        private string _feedback;

        private Category _category;

        public CreateTextChannelPageViewModel(NavigationStore navigationStore, Category category) {
            NavigationStore = navigationStore;

            Title = $"Create new text channel in category {category.Name}";
            NewTextChannelName = string.Empty;
            Feedback = string.Empty;
            _category = category;
        }

        [RelayCommand]
        private void CreateTextChannel() {
            CreateTextChannelData data = new(_category.ID, NewTextChannelName);
            string json = JsonSerializer.Serialize(data);
            App.Current.Client.ServerConnection.Send(OperationCode.CreateTextChannel, json);
        }
    }
}
