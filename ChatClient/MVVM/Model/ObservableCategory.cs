using ChatClient.MVVM.View.Main;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;


namespace ChatClient.MVVM.Model {
    public partial class ObservableCategory : ObservableObject {
        [ObservableProperty]
        private Category _category;

        [ObservableProperty]
        private Visibility _visibility;

        [ObservableProperty]
        private CategoryPrivilege? _privilege;

        [ObservableProperty]
        private bool _expanded;

        [ObservableProperty]
        private ObservableCollection<ObservableTextChannel> _textChannels;

        public User User { get; }


        public ObservableCategory(Category category, User user) {
            Category = category;
            User = user;
            Expanded = true;


            TextChannels = new ObservableCollection<ObservableTextChannel>();
            if (category.TextChannels != null) {
                foreach (var t in category.TextChannels) {
                    TextChannels.Add(new ObservableTextChannel(t, user));
                }
            }

            Update();
        }

        public void Update() {
            if (Category.Privileges == null) {
                MessageBox.Show("Category privileges in ObservableCategory are NULL.");
                return;
            }
            foreach (var p in Category.Privileges) {
                if (p.UserID == User.ID) {
                    Privilege = p;
                    break;
                }
            }
            if (Privilege == null) {
                MessageBox.Show("Category privilege in ObservableCategory is NULL.");
                return;
            }

            if (Category.Guild == null) {
                MessageBox.Show("Guild in ObservableCategory is NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            if (Category.Guild.Privileges == null) {
                MessageBox.Show("Guild privileges in ObservableCategory are NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }
            GuildPrivilege? guildPrivilege = null;
            foreach (var p in Category.Guild.Privileges) {
                if (p.UserID == User.ID) {
                    guildPrivilege = p;
                    break;
                }
            }
            if (guildPrivilege == null) {
                MessageBox.Show("Guild privilege in ObservableCategory is NULL.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }

            Privilege = Privilege.Merge(guildPrivilege);

            if (Privilege.ViewCategory == ChatShared.PrivilegeValue.Positive) {
                Visibility = Visibility.Visible;
            }
            else {
                Visibility = Visibility.Collapsed;
            }
        }

        public static void Remove(ChatContext context, Category category) {
            if (context.App.Navigation.GuildPage is GuildPage guildPage) {
                ObservableCategory? oRemove = null;
                foreach (ObservableCategory c in guildPage.ViewModel.Categories) {
                    if (c.Category.ID == category.ID) {
                        oRemove = c;
                    }
                }
                if (oRemove != null) {
                    guildPage.ViewModel.Categories.Remove(oRemove);
                }
            }
        }


        [RelayCommand]
        private void ToggelExpanded() {
            Expanded = !Expanded;
        }
    }
}
