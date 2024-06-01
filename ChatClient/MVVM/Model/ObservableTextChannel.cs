using ChatClient.MVVM.View.Main;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Windows;


namespace ChatClient.MVVM.Model {
    public partial class ObservableTextChannel : ObservableObject {

        [ObservableProperty]
        private TextChannel _textChannel;

        [ObservableProperty]
        private Visibility _visibility;

        [ObservableProperty]
        private TextChannelPrivilege? _privilege;

        public User User { get; }

        public ObservableTextChannel(TextChannel textChannel, User user) {
            TextChannel = textChannel;
            User = user;
            Update();
        }

        public void Update() {

            // text channel
            if (TextChannel.Privileges == null) {
                MessageBox.Show("Text channel privileges in ObservableTextChannel are NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            foreach (var p in TextChannel.Privileges) {
                if (p.UserID == User.ID) {
                    Privilege = p;
                    break;
                }
            }
            if (Privilege == null) {
                MessageBox.Show("Text channel privilege in ObservableTextChannel is NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            // category
            if (TextChannel.Category == null || TextChannel.Category.Privileges == null) {
                MessageBox.Show("Category privileges in ObservableTextChannel are NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            CategoryPrivilege? categoryPrivilege = null;
            foreach (var p in TextChannel.Category.Privileges) {
                if (p.UserID == User.ID) {
                    categoryPrivilege = p;
                    break;
                }
            }
            if (categoryPrivilege == null) {
                MessageBox.Show("Category privilege in ObservableTextChannel is NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            // guild
            if (TextChannel.Guild == null) {
                MessageBox.Show("Guild in ObservableTextChannel is NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            if (TextChannel.Guild.Privileges == null) {
                MessageBox.Show("Guild privileges in ObservableTextChannel are NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            GuildPrivilege? guildPrivilege = null;
            foreach (var p in TextChannel.Guild.Privileges) {
                if (p.UserID == User.ID) {
                    guildPrivilege = p;
                    break;
                }
            }
            if (guildPrivilege == null) {
                MessageBox.Show("Guild privilege in ObservableTextChannel is NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }

            // finish
            categoryPrivilege = categoryPrivilege.Merge(guildPrivilege);
            Privilege = Privilege.Merge(categoryPrivilege);

            if (Privilege.ViewChannel == ChatShared.PrivilegeValue.Positive) {
                Visibility = Visibility.Visible;
            }
            else {
                Visibility = Visibility.Collapsed;
            }
        }

        public static void Remove(ChatContext context, TextChannel textChannel, Category parent) {
            if (context.App.Navigation.GuildPage is GuildPage guildPage) {
                foreach (ObservableCategory c in guildPage.ViewModel.Categories) {
                    if (c.Category.ID == parent.ID) {
                        ObservableTextChannel? oRemove = null;
                        foreach (ObservableTextChannel t in c.TextChannels) {
                            if (t.TextChannel.ID == textChannel.ID) {
                                oRemove = t;
                                break;
                            }
                        }
                        if (oRemove != null) {
                            c.TextChannels.Remove(oRemove);
                        }
                        break;
                    }
                }
            }
        }



    }
}
