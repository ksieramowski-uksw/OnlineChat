using ChatClient.MVVM.Model;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatShared;
using ChatShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class TextChannelPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private TextChannel _textChannel;

        [ObservableProperty]
        private string _messageContent;

        [ObservableProperty]
        private ObservableCollection<ObservableUser> _visualUsers;

        public bool CanLoadMoreMessages { get; set; }
        public bool LastMessage { get; set; }

        public ScrollViewer? Scroll { get; set; }

        public TextChannelPageViewModel(ChatContext context, TextChannel textChannel, ObservableCollection<ObservableUser> visualUsers) {
            Context = context;

            TextChannel = textChannel;
            MessageContent = string.Empty;

            CanLoadMoreMessages = true;
            LastMessage = false;

            VisualUsers = visualUsers;

            foreach (var vu in VisualUsers) {
                vu.Update(textChannel);
            }
        }

        public void LoadMoreMessages(byte limit) {
            if (LastMessage == true) { return; }
            ID first = int.MaxValue;
            if (TextChannel.Messages.Count > 0) {
                first = TextChannel.Messages[0].ID;
            }
            Context.Client.GetMessageRange(TextChannel.ID, first, limit);
        }


        [RelayCommand]
        public void SendMessage(string? message = null) {
            if (Context.CurrentTextChannelPrivilege == null) { return; }
            if (Context.CurrentTextChannelPrivilege.Write != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\nwrite on this channel.");
                return;
            }
            if (Context.CurrentUser is not User user) { return; }
            if (MessageContent != string.Empty) {
                Context.Client.SendMessage(user.ID, TextChannel.ID, MessageContent);
                MessageContent = string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(message)) {
                Context.Client.SendMessage(user.ID, TextChannel.ID, message);
                MessageContent = string.Empty;
            }
        }

        [RelayCommand]
        private void TestButtonClick() {
            if (Context.CurrentUser != null && uint.TryParse(MessageContent, out uint n)) {
                for (uint i = 1; i <= n; i++) {
                    Context.Client.SendMessage(Context.CurrentUser.ID, TextChannel.ID, i.ToString());
                }
            }
        }
    }
}
