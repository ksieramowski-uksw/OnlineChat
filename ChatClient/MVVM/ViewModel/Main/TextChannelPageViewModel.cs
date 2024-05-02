using ChatClient.Stores;
using ChatShared.DataModels;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class TextChannelPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private TextChannel _textChannel;

        [ObservableProperty]
        private string _messageContent;

        [ObservableProperty]
        private ObservableCollection<User> _users;


        public TextChannelPageViewModel(ChatContext context, TextChannel textChannel) {
            Context = context;

            TextChannel = textChannel;
            MessageContent = string.Empty;
            Users = new ObservableCollection<User>();

            
            Context.Client.GetMessageRange(TextChannel.ID, ulong.MaxValue, 20);
        }

        [RelayCommand]
        void SendMessage() {
            if (MessageContent != string.Empty && Context.CurrentUser is User user) {
                MessageData data = new(user.ID, TextChannel.ID, MessageContent);
                string json = JsonSerializer.Serialize(data);
                Context.Client.ServerConnection.Send(OperationCode.SendMessage, json);
                MessageContent = string.Empty;
            }
        }
    }
}
