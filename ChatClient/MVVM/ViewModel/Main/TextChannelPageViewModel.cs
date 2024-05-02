using ChatClient.Stores;
using ChatShared.DataModels;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class TextChannelPageViewModel : ObservableObject {
        public NavigationStore NavigationStore { get; }

        [ObservableProperty]
        private TextChannel _textChannel;

        [ObservableProperty]
        private string _messageContent;



        public TextChannelPageViewModel(NavigationStore navigationStore, TextChannel textChannel) {
            NavigationStore = navigationStore;

            TextChannel = textChannel;
            MessageContent = string.Empty;

            
            App.Current.Client.GetMessageRange(TextChannel.ID, ulong.MaxValue, 20);
        }

        [RelayCommand]
        void SendMessage() {
            if (MessageContent != string.Empty && App.Current.Client.User is User user) {
                MessageData data = new(user.ID, TextChannel.ID, MessageContent);
                string json = JsonSerializer.Serialize(data);
                App.Current.Client.ServerConnection.Send(OperationCode.SendMessage, json);
                MessageContent = string.Empty;
            }
        }
    }
}
