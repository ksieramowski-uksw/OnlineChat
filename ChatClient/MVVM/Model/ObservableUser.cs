using ChatShared;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Windows;


namespace ChatClient.MVVM.Model {
    public partial class ObservableUser : ObservableObject {

        [ObservableProperty]
        private ChatContext _context;

        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private Visibility _visibility;

        [ObservableProperty]
        private GuildPrivilege? _guildPrivilege;

        [ObservableProperty]
        private CategoryPrivilege? _categoryPrivilege;

        [ObservableProperty]
        private TextChannelPrivilege? _textChannelPrivilege;


        public ObservableUser(User user, ChatContext context) {
            User = user;
            Context = context;
        }

        public void Update<T>(T obj) {
            if (obj is TextChannel textChannel) {
                // text channel
                if (textChannel.Privileges == null) {
                    MessageBox.Show("Text channel privileges in ObservableUser are NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                foreach (var p in textChannel.Privileges) {
                    if (p.UserID == User.ID) {
                        TextChannelPrivilege = p;
                        break;
                    }
                }
                if (TextChannelPrivilege == null) {
                    MessageBox.Show("Text channel privilege in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                // category
                if (textChannel.Category == null || textChannel.Category.Privileges == null) {
                    MessageBox.Show("Category privileges in ObservableUser are NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                foreach (var p in textChannel.Category.Privileges) {
                    if (p.UserID == User.ID) {
                        CategoryPrivilege = p;
                        break;
                    }
                }
                if (CategoryPrivilege == null) {
                    MessageBox.Show("Category privilege in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                // guild
                if (textChannel.Guild == null) {
                    MessageBox.Show("Guild in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                if (textChannel.Guild.Privileges == null) {
                    MessageBox.Show("Guild privileges in ObservableUser are NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                foreach (var p in textChannel.Guild.Privileges) {
                    if (p.UserID == User.ID) {
                        GuildPrivilege = p;
                        break;
                    }
                }
                if (GuildPrivilege == null) {
                    MessageBox.Show("Guild privilege in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }

                // finish
                CategoryPrivilege = CategoryPrivilege.Merge(GuildPrivilege);
                TextChannelPrivilege = TextChannelPrivilege.Merge(CategoryPrivilege);
                if (TextChannelPrivilege.ViewChannel == PrivilegeValue.Positive) {
                    Visibility = Visibility.Visible;
                }
                else {
                    Visibility = Visibility.Collapsed;
                }

                if (Context.CurrentUser != null && Context.CurrentUser.ID == User.ID) {
                    Context.CurrentTextChannelPrivilege = TextChannelPrivilege;
                    Context.CurrentCategoryPrivilege = CategoryPrivilege;
                    Context.CurrentGuildPrivilege = GuildPrivilege;
                }
            }
            else if (obj is Category category) {
                // category
                if (category.Privileges == null) {
                    MessageBox.Show("Category privileges in ObservableUser are NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                foreach (var p in category.Privileges) {
                    if (p.UserID == User.ID) {
                        CategoryPrivilege = p;
                        break;
                    }
                }
                if (CategoryPrivilege == null) {
                    MessageBox.Show("Category privilege in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                // guild
                if (category.Guild == null) {
                    MessageBox.Show("Guild in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                if (category.Guild.Privileges == null) {
                    MessageBox.Show("Guild privileges in ObservableUser are NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                MessageBox.Show(category.Guild.Privileges.Count.ToString());
                foreach (var p in category.Guild.Privileges) {
                    if (p.UserID == User.ID) {
                        GuildPrivilege = p;
                        break;
                    }
                }
                if (GuildPrivilege == null) {
                    MessageBox.Show("Guild privilege in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }

                // finish
                CategoryPrivilege = CategoryPrivilege.Merge(GuildPrivilege);
                if (CategoryPrivilege.ViewCategory == PrivilegeValue.Positive) {
                    Visibility = Visibility.Visible;
                }
                else {
                    Visibility = Visibility.Collapsed;
                }

                if (Context.CurrentUser != null && Context.CurrentUser.ID == User.ID) {
                    Context.CurrentCategoryPrivilege = CategoryPrivilege;
                    Context.CurrentGuildPrivilege = GuildPrivilege;
                }
            }
            else if (obj is Guild guild) {
                // guild
                if (guild == null) {
                    MessageBox.Show("Guild in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                if (guild.Privileges == null) {
                    MessageBox.Show("Guild privileges in ObservableUser are NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }
                foreach (var p in guild.Privileges) {
                    if (p.UserID == User.ID) {
                        GuildPrivilege = p;
                        break;
                    }
                }
                if (GuildPrivilege == null) {
                    MessageBox.Show("Guild privilege in ObservableUser is NULL.", MethodBase.GetCurrentMethod()?.Name);
                    return;
                }

                if (Context.CurrentUser != null && Context.CurrentUser.ID == User.ID) {
                    Context.CurrentGuildPrivilege = GuildPrivilege;
                }
            }
            else {
                MessageBox.Show($"Invalid input type in ObservableUser.Update: {obj?.GetType()}");
            }


        }

    }
}
