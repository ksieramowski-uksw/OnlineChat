using ChatClient.Stores;
using ChatShared.DataModels;
using ChatShared.Models;
using ChatShared.Models.Privileges;
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


        public TextChannelPageViewModel(ChatContext context, TextChannel textChannel) {
            Context = context;

            TextChannel = textChannel;
            MessageContent = string.Empty;

            Context.Client.GetMessageRange(TextChannel.ID, ulong.MaxValue, 20);

            
        }

        //public void UpdateVisibility() {
        //    GuildPrivilege ? g = null;
        //    foreach (var guild in Context.Guilds) {
        //        foreach (var user in guild.Users) {
        //            if (user.User.ID == )
        //        }


        //        foreach (var category in guild.Categories) {
        //            foreach (var channel in category.TextChannels) {
        //                if (channel.ID == TextChannel.ID) {

        //                }
        //            }
        //        }
        //    }
        //    foreach (var user in TextChannel.Users) {
        //        user.User.
        //    }
        //}


        [RelayCommand]
        private void SendMessage() {
            if (Context.CurrentUser == null) { return; }
            bool canWrite = false;
            foreach (var u in TextChannel.Users) {
                if (u.User.ID == Context.CurrentUser.ID) {
                    if (u.FinalPrivilege.Write == ChatShared.PrivilegeValue.Positive) {
                        canWrite = true;
                        break;
                    }
                }
            }
            if (canWrite == false) {
                MessageBox.Show("You don't have privilege to write on this channel");
                return;
            }
            else {
                MessageBox.Show("can write");
            }
            
            if (MessageContent != string.Empty && Context.CurrentUser is User user) {
                MessageData data = new(user.ID, TextChannel.ID, MessageContent);
                string json = JsonSerializer.Serialize(data);
                Context.Client.ServerConnection.Send(OperationCode.SendMessage, json);
                MessageContent = string.Empty;
            }
        }
    }
}
