using ChatClient.Stores;
using ChatShared.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class CreateGuildPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private string _guildName;

        [ObservableProperty]
        private string _guildPassword;

        [ObservableProperty]
        private string _feedback;

        public CreateGuildPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;

            _guildName = string.Empty;
            _guildPassword = string.Empty;
            _feedback = string.Empty;
        }

        [RelayCommand]
        void CreateGuild() {
            if (!string.IsNullOrWhiteSpace(GuildName) && !string.IsNullOrWhiteSpace(GuildPassword)) {
                string json = JsonSerializer.Serialize(new CreateGuildData(GuildName, GuildPassword));
                App.Current.Client.ServerConnection.Send(OperationCode.CreateGuild, json);
            }
            else {
                Feedback = "Please, fill required fields.";
            }

        }
    }
}
