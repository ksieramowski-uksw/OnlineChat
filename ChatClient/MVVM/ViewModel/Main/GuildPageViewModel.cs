using ChatClient.MVVM.Model;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatShared;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;


namespace ChatClient.MVVM.ViewModel.Main {
    public partial class GuildPageViewModel : ObservableObject {
        public ChatContext Context { get; }

        [ObservableProperty]
        private Guild _guild;

        [ObservableProperty]
        private ObservableCollection<ObservableCategory> _categories;

        [ObservableProperty]
        private TextChannelPage? _textChannelContent;

        [ObservableProperty]
        private ObservableCollection<ObservableUser> _visualUsers;


        public GuildPageViewModel(ChatContext context, Guild guild) {
            Context = context;
            _guild = guild;

            Categories = new ObservableCollection<ObservableCategory>();
            if (guild.Categories != null && Context.CurrentUser != null) {
                foreach (var c in guild.Categories) {
                    Categories.Add(new ObservableCategory(c, Context.CurrentUser));
                }
            }

            VisualUsers = new();
            if (guild.Users != null) {
                foreach (var user in guild.Users) {
                    var vu = new ObservableUser(user, Context);
                    vu.Update(Guild);
                    VisualUsers.Add(vu);
                }
            }
        }


        public void Dispose() {
            Categories.Clear();
            TextChannelContent = null;
            VisualUsers.Clear();
            Guild.DisposeDetails();
        }


        [RelayCommand]
        private void CopyPublicID(string publicID) {
            Clipboard.SetText(publicID);
        }

        [RelayCommand]
        private void SelectTextChannel(ID id) {
            if (Guild.Categories == null) {
                string error = "Reqired data not found.";
                if (!NotificationPage.Show(Context, error)) {
                    MessageBox.Show(error, MethodBase.GetCurrentMethod()?.Name);
                }
                return;
            }
            foreach (Category category in Guild.Categories) {
                bool success = false;
                if (category.TextChannels == null) {
                    string error = "Reqired data not found.";
                    if (!NotificationPage.Show(Context, error)) {
                        MessageBox.Show(error, MethodBase.GetCurrentMethod()?.Name);
                    }
                    return;
                }
                foreach (TextChannel textChannel in category.TextChannels) {
                    if (textChannel.ID == id) {
                        TextChannelContent = new TextChannelPage(Context, textChannel, VisualUsers);
                        Context.App.Navigation.TextChannelPage = TextChannelContent;
                        success = true;
                        break;
                    }
                }
                if (success) { break; }
            }
        }




        [RelayCommand]
        private void GuildProperties() {
            if (Context.CurrentUser == null) { return; }

            GuildPrivilege? privilege = Guild.GetPrivilege(Context.CurrentUser.ID);
            if (privilege == null) { return; }
            if (privilege.ManageGuild != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\nmanage this server.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.UpdateGuildPage = new UpdateGuildPage(Context, Guild);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.UpdateGuildPage;
                mainPage.ViewModel.ShowMask();
            }
        }

        [RelayCommand]
        private void DeleteGuild() {
            if (Context.CurrentUser == null) { return; }

            if (Context.CurrentUser.ID != Guild.OwnerID) {
                NotificationPage.Show(Context, "Only owner of the server can delete it.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.DeleteConfirmationPage = new DeleteConfirmationPage(Context, Guild);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.DeleteConfirmationPage;
                mainPage.ViewModel.ShowMask();
            }
        }



        [RelayCommand]
        private void CreateCategory() {
            if (Context.CurrentUser == null) { return; }

            GuildPrivilege? privilege = Guild.GetPrivilege(Context.CurrentUser.ID);
            if (privilege == null) { return; }
            if (privilege.CreateCategory != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\ncreate categories in this server.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.CreateCategoryPage = new CreateCategoryPage(Context, Guild);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.CreateCategoryPage;
                mainPage.ViewModel.ShowMask();
            }
        }

        [RelayCommand]
        private void CategoryProperties(Category category) {
            if (Context.CurrentUser == null) {  return; }

            ObservableCategory observableCategory = new(category, Context.CurrentUser);
            if (observableCategory.Privilege == null) { return; }
            if (observableCategory.Privilege.UpdateCategory != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\nmanage this category.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage && category != null) {
                Context.App.Navigation.UpdateCategoryPage = new UpdateCategoryPage(Context, category);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.UpdateCategoryPage;
                mainPage.ViewModel.ShowMask();
            }
        }

        [RelayCommand]
        private void DeleteCategory(Category category) {
            if (Context.CurrentUser == null) { return; }

            ObservableCategory observableCategory = new(category, Context.CurrentUser);
            if (observableCategory.Privilege == null) { return; }
            if (observableCategory.Privilege.DeleteCategory != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\ndelete this category.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.DeleteConfirmationPage = new DeleteConfirmationPage(Context, category);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.DeleteConfirmationPage;
                mainPage.ViewModel.ShowMask();
            }
        }



        [RelayCommand]
        private void CreateTextChannel(ObservableCategory category) {
            if (category.Privilege == null) { return; }

            if (category.Privilege.CreateChannel != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\ncreate channels in this category.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.CreateTextChannelPage = new CreateTextChannelPage(Context, category.Category);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.CreateTextChannelPage;
                mainPage.ViewModel.ShowMask();
            }
        }

        [RelayCommand]
        private void TextChannelProperties(TextChannel textChannel) {
            if (Context.CurrentUser == null) { return; }

            ObservableTextChannel observableTextChannel = new(textChannel, Context.CurrentUser);
            if (observableTextChannel.Privilege == null) { return; }
            if (observableTextChannel.Privilege.UpdateChannel != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\nmanage this channel.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage && textChannel != null) {
                Context.App.Navigation.UpdateTextChannelPage = new UpdateTextChannelPage(Context, textChannel);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.UpdateTextChannelPage;
                mainPage.ViewModel.ShowMask();
            }
        }

        [RelayCommand]
        private void DeleteTextChannel(TextChannel textChannel) {
            if (Context.CurrentUser == null) { return; }

            ObservableTextChannel observableTextChannel = new(textChannel, Context.CurrentUser);
            if (observableTextChannel.Privilege == null) { return; }
            if (observableTextChannel.Privilege.DeleteChannel != PrivilegeValue.Positive) {
                NotificationPage.Show(Context, "You don't have permission to\ndelete this channel.");
                return;
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                Context.App.Navigation.DeleteConfirmationPage = new DeleteConfirmationPage(Context, textChannel);
                mainPage.ViewModel.PopupContent = Context.App.Navigation.DeleteConfirmationPage;
                mainPage.ViewModel.ShowMask();
            }
        }


    }
}
