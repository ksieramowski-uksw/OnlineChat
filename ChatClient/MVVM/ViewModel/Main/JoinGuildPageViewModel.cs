using ChatClient.Stores;
using ChatShared.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class JoinGuildPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private string _guildName;

        [ObservableProperty]
        private string _guildPassword;

        [ObservableProperty]
        private string _feedback;

        public JoinGuildPageViewModel(NavigationStore navigationStore) {
            NavigationStore = navigationStore;

            GuildName = string.Empty;
            GuildPassword = string.Empty;
            Feedback = string.Empty;
        }

        [RelayCommand]
        private void JoinGuild() {
            if (!string.IsNullOrWhiteSpace(GuildName) && !string.IsNullOrWhiteSpace(GuildPassword)) {
                string json = JsonSerializer.Serialize(new JoinGuildData(GuildName, GuildPassword));
                App.Current.Client.ServerConnection.Send(OperationCode.JoinGuild, json);
            }
            else {
                Feedback = "Please, fill required fields.";
            }
        }
    }
}
