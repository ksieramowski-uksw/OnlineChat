using ChatClient.MVVM.Model;
using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatClient.MVVM.ViewModel.Main;
using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Windows;


namespace ChatClient.Network {
    public class ServerOperationHandler {

        private readonly ChatContext Context;
        private readonly ServerConnection _serverConnection;

        public ServerOperationHandler(ServerConnection serverConnection) {
            _serverConnection = serverConnection;
            Context = _serverConnection.Client.Context;
        }

        public void HandleOperation(OperationCode opCode, string message) {
            switch (opCode) {
                case OperationCode.LogInSuccess: {
                    LoginSuccess(message);
                } break;
                case OperationCode.LogInFail: {
                    LoginFail(message);
                } break;
                case OperationCode.RegisterSuccess: {
                    RegisterSuccess(message);
                } break;
                case OperationCode.RegisterFail: {
                    RegisterFail(message);
                } break;
                case OperationCode.CreateGuildSuccess: {
                    CreateGuildSuccess(message);
                } break;
                case OperationCode.CreateGuildFail: {
                    CreateGuildFail(message);
                } break;
                case OperationCode.UpdateGuildSuccess: {
                    UpdateGuildSuccess(message);
                } break;
                case OperationCode.UpdateGuildFail: {
                    UpdateGuildFail(message);
                } break;
                case OperationCode.JoinGuildSuccess: {
                    JoinGuildSuccess(message);
                } break;
                case OperationCode.JoinGuildFail: {
                    JoinGuildFail(message);
                } break;
                case OperationCode.CreateCategorySuccess: {
                    CreateCategorySuccess(message);
                } break;
                case OperationCode.CreateCategoryFail: {
                    CreateCategoryFail(message);
                } break;
                case OperationCode.UpdateCategorySuccess: {
                    UpdateCategorySuccess(message);
                } break;
                case OperationCode.UpdateCategoryFail: {
                    UpdateCategoryFail(message);
                } break;
                case OperationCode.CreateTextChannelSuccess: {
                    CreateTextChannelSuccess(message);
                } break;
                case OperationCode.CreateTextChannelFail: {
                    CreateTextChannelFail(message);
                } break;
                case OperationCode.UpdateTextChannelSuccess: {
                    UpdateTextChannelSuccess(message);
                } break;
                case OperationCode.UpdateTextChannelFail: {
                    UpdateTextChannelFail(message);
                } break;
                case OperationCode.GetGuildsForCurrentUserSuccess: {
                    GetGuildsForCurrentUserSuccess(message);
                } break;
                case OperationCode.GetGuildsForCurrentUserFail: {
                    GetGuildsForCurrentUserFail(message);
                } break;
                case OperationCode.GetGuildDetailsSuccess: {
                    GetGuildDetailsSuccess(message);
                } break;
                case OperationCode.GetGuildDetailsFail: {
                    GetGuildDetailsFail(message);
                } break;
                case OperationCode.SendMessageSuccess: {
                    SendMessageSuccess(message);
                } break;
                case OperationCode.SendMessageFail: {
                    SendMessageFail(message);
                } break;
                case OperationCode.GetMessageRangeSuccess: {
                    GetMessageRangeSuccess(message);
                } break;
                case OperationCode.GetMessageRangeFail: {
                    GetMessageRangeFail(message);
                } break;
                case OperationCode.DeleteGuildSuccess: {
                    DeleteGuildSuccess(message);
                } break;
                case OperationCode.DeleteGuildFail: {
                    DeleteGuildFail(message);
                } break;
                case OperationCode.DeleteCategorySuccess: {
                    DeleteCategorySuccess(message);
                } break;
                case OperationCode.DeleteCategoryFail: {
                    DeleteCategoryFail(message);
                } break;
                case OperationCode.DeleteTextChannelSuccess: {
                    DeleteTextChannelSuccess(message);
                } break;
                case OperationCode.DeleteTextChannelFail: {
                    DeleteTextChannelFail(message);
                } break;
                case OperationCode.UpdateUserSuccess: {
                    UpdateUserSuccess(message);
                } break;
                case OperationCode.UpdateUserFail: {
                    UpdateUserFail(message);
                } break;
                case OperationCode.UserStatusChanged: {
                    UserStatusChanged(message);
                } break;
            }
        }

        private void LoginSuccess(string message) {
            if (Context.App.Navigation.MainWindow != null) {
                return;
            }
            Context.CurrentUser = JsonSerializer.Deserialize<User>(message);
            if (Context.CurrentUser == null) {
                if (Context.App.Navigation.LoginPage is LoginPage loginPage) {
                    loginPage.ViewModel.Feedback = "Something went wrong...";
                }
                return;
            }
            Context.Users.Add(Context.CurrentUser);

            Context.App.Dispatcher.Invoke(() => {
                Window previousWindow = Context.App.MainWindow;
                Context.App.Navigation.MainWindow = new MainWindow(Context);
                MainPage mainPage = new(Context);
                Context.App.Navigation.MainPage = mainPage;
                Context.App.Navigation.MainWindow.ViewModel.MainPage = mainPage;
                Context.App.MainWindow = Context.App.Navigation.MainWindow;

                previousWindow.Close();
                Context.App.Navigation.LoginWindow = null;
                Context.App.Navigation.LoginPage = null;
                Context.App.Navigation.RegisterPage = null;

                Context.App.MainWindow.Show();
            });
        }   
        
        private void LoginFail(string message) {
            if (Context.App.Navigation.LoginPage != null) {
                Context.App.Navigation.LoginPage.ViewModel.Feedback = message;
            }
        }

        private void RegisterSuccess(string message) {
            if (Context.App.Navigation.LoginPage != null) {
                Context.App.Navigation.LoginPage.ViewModel.Feedback = message;
                if (Context.App.Navigation.LoginWindow is LoginWindow loginWindow) {
                    Context.App.Dispatcher.Invoke(() => {
                        loginWindow.ViewModel.Navigate(Context.App.Navigation.LoginPage);
                    });
                }
            }
        }

        private void RegisterFail(string message) {
            if (Context.App.Navigation.RegisterPage != null) {
                Context.App.Navigation.RegisterPage.ViewModel.Feedback = message;
            }
        }



        private void JoinGuildSuccess(string message) {
            JoiningUser? joiningUser = JsonSerializer.Deserialize<JoiningUser>(message);
            if (joiningUser == null || joiningUser.User == null || joiningUser.Guild == null) {
                MessageBox.Show("Response is corrupted.", MethodBase.GetCurrentMethod()?.Name);
                return;
            }

            if (Context.App.Navigation.MainPage is not MainPage mainPage) { return; }

            // current user is joining user
            if (Context.CurrentUser != null && joiningUser.User.ID == Context.CurrentUser.ID) {
                Context.App.Dispatcher.Invoke(() => {

                    mainPage.ViewModel.Guilds.Add(joiningUser.Guild);
                });
            }
            // joining user shares guild with current user
            else {
                foreach (Guild g in mainPage.ViewModel.Guilds) {
                    if (g.ID == joiningUser.Guild.ID) {
                        Context.App.Dispatcher.Invoke(() => {
                            User? user = null;
                            foreach (User u in Context.Users) {
                                if (u.ID == joiningUser.User.ID) {
                                    user = u;
                                    break;
                                }
                            }
                            if (user == null) {
                                user = joiningUser.User;
                                Context.Users.Add(user);
                            }
                            g.Users?.Add(user);

                            // guild privilege
                            GuildPrivilege gp = new(g.DefaultPrivilege) { UserID = joiningUser.User.ID };
                            g.Privileges?.Add(gp);

                            // category privilege
                            if (g.Categories != null) {
                                foreach (var c in g.Categories) {
                                    CategoryPrivilege cp = new(c.DefaultPrivilege) { UserID = user.ID };
                                    c.Privileges?.Add(cp);
                                    
                                    // text channel privilege
                                    if (c.TextChannels == null) { continue; }
                                    foreach (var t in c.TextChannels) {
                                        TextChannelPrivilege tp = new (t.DefaultPrivilege) { UserID = user.ID };
                                        t.Privileges?.Add(tp);
                                    }
                                }
                            }


                            //if (Context.App.Navigation.TextChannelPage is TextChannelPage textChannelPage) {
                            //    ObservableUser vu = new(user, Context);
                            //    vu.Update(textChannelPage.ViewModel.TextChannel);
                            //    textChannelPage.ViewModel.VisualUsers.Add(vu);
                            //}
                            //else
                            if (Context.App.Navigation.GuildPage is GuildPage guildPage) {
                                ObservableUser vu = new(user, Context);
                                vu.Update(guildPage.ViewModel.Guild);
                                guildPage.ViewModel.VisualUsers.Add(vu);
                            }
                        });
                        break;
                    }
                }
            }
            mainPage.ViewModel.HideMask();
        }

        private void JoinGuildFail(string message) {
            if (Context.App.Navigation.CreateOrJoinGuildPage is CreateOrJoinGuildPage createOrJoinPage) {
                createOrJoinPage.ViewModel.ExistingGuildFeedback = message;
            }
            else {
                MessageBox.Show(message);
            }
        }




        private void CreateGuildSuccess(string message) {
            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                if (JsonSerializer.Deserialize<Guild>(message) is Guild guild) {
                    Context.App.Dispatcher.Invoke(() => {
                        bool resourceFound = false;
                        int index = 0;
                        for (; index < Context.App.ResourceStorage.Count; index++) {
                            if (Context.App.ResourceStorage[index] == guild.Icon) {
                                resourceFound = true;
                                break;
                            }
                        }
                        if (resourceFound) {
                            guild.Icon = Context.App.ResourceStorage[index];
                        }
                        else {
                            Context.App.ResourceStorage.Add(guild.Icon);
                        }
                        Context.Guilds.Add(guild);
                        mainPage.ViewModel.HideMask();
                    });
                }
                else {
                    if (Context.App.Navigation.CreateOrJoinGuildPage is CreateOrJoinGuildPage createOrJoinGuildPage) {
                        createOrJoinGuildPage.ViewModel.NewGuildFeedback = "Response is corrupted.";
                    }
                }
            }
        }

        private void CreateGuildFail(string message) {
            if (Context.App.Navigation.CreateOrJoinGuildPage is CreateOrJoinGuildPage createOrJoinGuildPage) {
                createOrJoinGuildPage.ViewModel.NewGuildFeedback = message;
            }
        }



        private void CreateCategorySuccess(string message) {
            if (Context.App.Navigation.GuildPage == null) { return; }
            Category? category = JsonSerializer.Deserialize<Category>(message);
            if (category == null) {
                if (Context.App.Navigation.CreateCategoryPage is CreateCategoryPage createCategoryPage) {
                    createCategoryPage.ViewModel.Feedback = "Response is corrupted.";
                }
                return;
            }

            bool success = false;

            foreach (Guild guild in Context.Guilds) {
                if (guild.ID == category.GuildID) {
                    if (guild.Categories != null) {
                        Context.App.Dispatcher.Invoke(() => {
                            category.Guild = guild;
                            category.Users = guild.Users;
                            guild.Categories.Add(category);

                            if (Context.App.Navigation.GuildPage is GuildPage guildPage && guildPage.ViewModel.Guild.ID == guild.ID && Context.CurrentUser != null) {
                                guildPage.ViewModel.Categories.Add(new ObservableCategory(category, Context.CurrentUser));
                            }
                        });
                        success = true;
                    }
                    break;
                }
            }
            if (success == true && Context.App.Navigation.MainPage is MainPage mainPage) {
                mainPage.ViewModel.HideMask();
                if (Context.App.Navigation.CreateCategoryPage is CreateCategoryPage createCategoryPage) {
                    createCategoryPage.ViewModel.NewCategoryName = string.Empty;
                    createCategoryPage.ViewModel.Feedback = string.Empty;

                    Context.App.Navigation.CreateCategoryPage = null;
                }
            }

            if (success == false) {
                if (Context.App.Navigation.CreateCategoryPage is CreateCategoryPage createCategoryPage) {
                    createCategoryPage.ViewModel.Feedback = "Response is corrupted.";
                }
            }
        }

        private void CreateCategoryFail(string message) {
            if (Context.App.Navigation.CreateCategoryPage is CreateCategoryPage createCategoryPage) {
                createCategoryPage.ViewModel.Feedback = message;
            }
        }



        private void CreateTextChannelSuccess(string message) {
            if (Context.App.Navigation.GuildPage == null) { return; }

            TextChannel? textChannel = JsonSerializer.Deserialize<TextChannel>(message);
            if (textChannel == null) {
                if (Context.App.Navigation.CreateTextChannelPage is CreateTextChannelPage createTextChannelPage) {
                    createTextChannelPage.ViewModel.Feedback = "Response is corrupted.";
                }
                return;
            }

            bool success = false;

            foreach (Guild guild in Context.Guilds) {
                if (guild.Categories == null) {
                    continue;
                }
                foreach (Category category in guild.Categories) {
                    if (category.ID == textChannel.CategoryID && category.TextChannels != null) {

                        Context.App.Dispatcher.Invoke(() => {
                            textChannel.Guild = category.Guild;
                            textChannel.Category = category;
                            textChannel.Users = category.Users;
                            category.TextChannels.Add(textChannel);

                            if (Context.App.Navigation.GuildPage is GuildPage guildPage && guildPage.ViewModel.Guild.ID == guild.ID && Context.CurrentUser != null) {
                                foreach (ObservableCategory c in guildPage.ViewModel.Categories) {
                                    if (c.Category.ID == category.ID) {
                                        c.TextChannels.Add(new ObservableTextChannel(textChannel, Context.CurrentUser));
                                        break;
                                    }
                                }
                            }
                        });
                        success = true;
                        break;
                    }
                }
                if (success == true) { break; }
            }
            if (success == true && Context.App.Navigation.MainPage is MainPage mainPage) {
                mainPage.ViewModel.HideMask();
                if (Context.App.Navigation.CreateTextChannelPage is CreateTextChannelPage createTextChannelPage) {
                    createTextChannelPage.ViewModel.NewTextChannelName = string.Empty;
                    createTextChannelPage.ViewModel.Feedback = string.Empty;
                    Context.App.Navigation.CreateTextChannelPage = null;
                }
            }
            if (success == false) {
                if (Context.App.Navigation.CreateTextChannelPage is CreateTextChannelPage createTextChannelPage) {
                    createTextChannelPage.ViewModel.Feedback = "Response is corrupted.";
                }
            }
        }

        private void CreateTextChannelFail(string message) {
            if (Context.App.Navigation.CreateTextChannelPage is CreateTextChannelPage createTextChannelPage) {
                createTextChannelPage.ViewModel.Feedback = message;
            }
        }


        private void UpdateGuildSuccess(string message) {
            UpdateGuildData? data = JsonSerializer.Deserialize<UpdateGuildData>(message);
            if (data == null) { return; }

            Context.App.Dispatcher.Invoke(() => {

                bool success = false;
                foreach (var g in Context.Guilds) {
                    if (g.ID == data.ID) {
                        g.Name = data.Name;
                        if (data.Icon != null) {
                            g.Icon = data.Icon;
                        }

                        if (data.Privilege != null) {
                            // default
                            if (data.Privilege.UserID == 0) {
                                g.DefaultPrivilege = data.Privilege;
                            }
                            // user
                            else if (g.Privileges != null) {
                                for (int i = 0; i < g.Privileges.Count; i++) {
                                    if (g.Privileges[i].UserID == data.Privilege.UserID) {
                                        g.Privileges[i] = data.Privilege;
                                        break;
                                    }
                                }

                                if (Context.App.Navigation.GuildPage is GuildPage guildPage) {
                                    foreach (var vu in guildPage.ViewModel.VisualUsers) {
                                        vu.Update(guildPage.ViewModel.Guild);
                                    }
                                }

                            }
                        }
                        success = true;
                        break;
                    }
                }

                if (success == true) {
                    if (Context.App.Navigation.MainPage is MainPage mainPage) {
                        mainPage.ViewModel.HideMask();
                    }
                }
                else {
                    if (Context.App.Navigation.UpdateGuildPage is UpdateGuildPage updateGuildPage) {
                        updateGuildPage.ViewModel.Feedback = "Something went wrong...";
                    }
                }
            });
        }

        private void UpdateGuildFail(string message) {
            if (Context.App.Navigation.UpdateGuildPage is UpdateGuildPage updateGuildPage) {
                updateGuildPage.ViewModel.Feedback = message;
            }
        }



        private void UpdateCategorySuccess(string message) {
            UpdateCategoryData? data = JsonSerializer.Deserialize<UpdateCategoryData?>(message);
            if (data == null) {
                MessageBox.Show("Category update data is NULL.");
                return;
            }

            Context.App.Dispatcher.Invoke(() => {

                bool success = false;
                foreach (var g in Context.Guilds) {
                    if (g.Categories == null) { continue; }
                    foreach (var c in g.Categories) {
                        if (c.ID == data.CategoryID) {
                            c.Name = data.Name;
                            if (data.Privilege != null) {
                                // default
                                if (data.Privilege.UserID == 0) {
                                    c.DefaultPrivilege = data.Privilege;
                                }
                                // user
                                else if (c.Privileges != null) {
                                    for (int i = 0; i < c.Privileges.Count; i++) {
                                        if (c.Privileges[i].UserID == data.Privilege.UserID) {
                                            c.Privileges[i] = data.Privilege;
                                            break;
                                        }
                                    }

                                    if (Context.App.Navigation.TextChannelPage is TextChannelPage textChannelPage
                                        && data.Privilege.CategoryID == textChannelPage.ViewModel.TextChannel.CategoryID) {
                                        foreach (var vu in textChannelPage.ViewModel.VisualUsers) {
                                            vu.Update(textChannelPage.ViewModel.TextChannel);
                                        }
                                    }
                                    
                                }
                            }
                            success = true;
                            break;
                        }
                    }
                }

                if (success == true) {
                    if (Context.App.Navigation.MainPage is MainPage mainPage) {
                        mainPage.ViewModel.HideMask();
                    }
                }
                else {
                    if (Context.App.Navigation.UpdateCategoryPage is UpdateCategoryPage updateCategoryPage) {
                        updateCategoryPage.ViewModel.Feedback = "Something went wrong...";
                    }
                }
            });
        }

        private void UpdateCategoryFail(string message) {
            if (Context.App.Navigation.UpdateCategoryPage is UpdateCategoryPage updateCategoryPage) {
                updateCategoryPage.ViewModel.Feedback = $"Something went wrong...{message}";
            }
        }


        private void UpdateTextChannelSuccess(string message) {
            UpdateTextChannelData? data = JsonSerializer.Deserialize<UpdateTextChannelData?>(message);
            if (data == null) {
                MessageBox.Show("Text channel update data is NULL.");
                return;
            }

            Context.App.Dispatcher.Invoke(() => {

                bool success = false;
                foreach (var g in Context.Guilds) {
                    if (g.Categories == null) { continue; }
                    foreach (var c in g.Categories) {
                        if (c.TextChannels == null) { continue; }
                        foreach (var t in c.TextChannels) {
                            if (t.ID == data.ID) {
                                t.Name = data.Name;
                                if (data.Privilege != null) {
                                    // default
                                    if (data.Privilege.UserID == 0) {
                                        t.DefaultPrivilege = data.Privilege;
                                    }
                                    // user
                                    else if (t.Privileges != null) {
                                        for (int i = 0; i < t.Privileges.Count; i++) {
                                            if (t.Privileges[i].UserID == data.Privilege.UserID) {
                                                t.Privileges[i] = data.Privilege;
                                                break;
                                            }
                                        }

                                        if (Context.App.Navigation.TextChannelPage is TextChannelPage textChannelPage
                                            && data.Privilege.ChannelID == textChannelPage.ViewModel.TextChannel.ID) {
                                            foreach (var vu in textChannelPage.ViewModel.VisualUsers) {
                                                vu.Update(textChannelPage.ViewModel.TextChannel);
                                            }
                                        }

                                    }
                                }
                                success = true;
                                break;
                            }
                        }

                    }
                }

                if (success == true) {
                    if (Context.App.Navigation.MainPage is MainPage mainPage) {
                        mainPage.ViewModel.HideMask();
                    }
                }
                else {
                    if (Context.App.Navigation.UpdateTextChannelPage is UpdateTextChannelPage updateTextChannelPage) {
                        updateTextChannelPage.ViewModel.Feedback = "Something went wrong...";
                    }
                }
            });
        }

        private void UpdateTextChannelFail(string message) {
            if (Context.App.Navigation.UpdateTextChannelPage is UpdateTextChannelPage updateTextChannelPage) {
                updateTextChannelPage.ViewModel.Feedback = $"Something went wrong...\n{message}";
            }
        }



        private void DeleteGuildSuccess(string message) {
            if (!ID.TryParse(message, out ID guildID)) { return; }

            Guild? remove = null;
            foreach (var g in Context.Guilds) {
                if (guildID == g.ID) {
                    remove = g;
                    break;
                }
            }

            if (remove != null) {
                Context.App.Dispatcher.Invoke(() => {
                    // close page if contains deleted element
                    if (Context.App.Navigation.MainPage is MainPage mainPage 
                        && Context.App.Navigation.GuildPage is GuildPage guildPage
                        && guildPage.ViewModel.Guild.ID == remove.ID) {
                        mainPage.ViewModel.Navigate(null);
                    }

                    Context.Guilds.Remove(remove);
                });
            }
        }

        private void DeleteGuildFail(string message) {
            MessageBox.Show(message);
        }



        private void DeleteCategorySuccess(string message) {
            if (!ID.TryParse(message, out ID categoryID)) { return; }

            Guild? parent = null;
            Category? remove = null;
            foreach (var g in Context.Guilds) {
                if (g.Categories == null) { continue; }
                foreach (var c in g.Categories) {
                    if (c.ID == categoryID) {
                        parent = g;
                        remove = c;
                        break;
                    };
                }
            }

            if (remove != null && parent != null && parent.Categories != null) {
                Context.App.Dispatcher.Invoke(() => {
                    // close page if contains deleted element
                    if (Context.App.Navigation.GuildPage is GuildPage guildPage
                        && guildPage.ViewModel.TextChannelContent is TextChannelPage textChannelPage
                        && textChannelPage.ViewModel.TextChannel.Category is Category category
                        && category.ID == remove.ID) {
                        guildPage.ViewModel.TextChannelContent = null;
                    }

                    ObservableCategory.Remove(Context, remove);
                    parent.Categories.Remove(remove);
                });

                if (Context.App.Navigation.MainPage is MainPage mainPage) {
                    mainPage.ViewModel.HideMask();
                }
            }

        }

        private void DeleteCategoryFail(string message) {
            MessageBox.Show(message);
        }



        private void DeleteTextChannelSuccess(string message) {
            if (!ID.TryParse(message, out ID textChannelID)) { return; }

            Category? parent = null;
            TextChannel? remove = null;
            foreach (var g in Context.Guilds) {
                if (g.Categories == null) { continue; }
                foreach (var c in g.Categories) {
                    if (c.TextChannels == null) { continue; }
                    foreach (var t in c.TextChannels) {
                        if (t.ID == textChannelID) {
                            parent = c;
                            remove = t;
                            break;
                        }
                    }
                }
            }

            if (remove != null && parent != null && parent.TextChannels != null) {
                Context.App.Dispatcher.Invoke(() => {
                    // close page if contains deleted element
                    if (Context.App.Navigation.GuildPage is GuildPage guildPage
                        && guildPage.ViewModel.TextChannelContent is TextChannelPage textChannelPage
                        && textChannelPage.ViewModel.TextChannel.ID == remove.ID) {
                        guildPage.ViewModel.TextChannelContent = null;
                    }

                    ObservableTextChannel.Remove(Context, remove, parent);
                    parent.TextChannels.Remove(remove);
                });

                if (Context.App.Navigation.MainPage is MainPage mainPage) {
                    mainPage.ViewModel.HideMask();
                }
            }
        }

        public void DeleteTextChannelFail(string message) {
            MessageBox.Show(message, MethodBase.GetCurrentMethod()?.Name);
        }




        private void GetGuildsForCurrentUserSuccess(string message) {
            ObservableCollection<Guild>? guilds = JsonSerializer.Deserialize<ObservableCollection<Guild>>(message);
            if (guilds is null) {
                MessageBox.Show("Failed to get list of servers.");
                return;
            }

            Context.App.Dispatcher.Invoke(() => {
                foreach (var guild in guilds) {
                    Context.Guilds.Add(guild);
                }
            });
        }

        private void GetGuildsForCurrentUserFail(string message) {
            MessageBox.Show($"Failed to get list of servers.\n{message}", "CRITICAL_ERROR");
        }



        private void GetGuildDetailsSuccess(string message) {
            GuildDetails? details = JsonSerializer.Deserialize<GuildDetails>(message);
            if (details == null || details.Users == null || details.Categories == null || details.Privileges == null) {
                MessageBox.Show("Failed to get server details.");
                return;
            }

            Guild? guild = null;
            foreach (var g in Context.Guilds) {
                if (g.ID == details.GuildID) {
                    guild = g;
                    break;
                }
            }

            if (guild == null) {
                MessageBox.Show("Server details corruption.");
                return;
            }

            guild.Users = new();
            guild.Categories = new();
            guild.Privileges = new();
            guild.DefaultPrivilege = details.DefaultPrivilege;

            Context.App.Dispatcher.Invoke(() => {

                foreach (var user in details.Users) {
                    bool found = false;
                    foreach (var u in Context.Users) {
                        if (user.ID == u.ID) {
                            found = true;
                            guild.Users.Add(u);
                            break;
                        }
                    }
                    if (found == false) {
                        Context.Users.Add(user);
                        guild.Users.Add(Context.Users[Context.Users.Count - 1]);
                    }
                }

                foreach (var category in details.Categories) {
                    category.Guild = guild;
                    category.Users = guild.Users;
                    if (category.TextChannels == null) {
                        MessageBox.Show($"Corrupted text channels in category '{category.Name}'.");
                        return;
                    }
                    foreach (var textChannel in category.TextChannels) {
                        textChannel.Guild = guild;
                        textChannel.Category = category;
                        textChannel.Users = guild.Users;
                    }
                    guild.Categories.Add(category);
                }

                foreach (var privilege in details.Privileges) {
                    guild.Privileges.Add(privilege);
                }

                if (Context.App.Navigation.MainPage is MainPage mainPage) {
                    Context.App.Navigation.GuildPage = new GuildPage(Context, guild);
                    mainPage.ViewModel.Navigate(Context.App.Navigation.GuildPage);
                }
            });
        }

        private void GetGuildDetailsFail(string message) {
            throw new NotImplementedException();
        }



        private void SendMessageSuccess(string message) {
            Message? msg = null;
            try { msg = JsonSerializer.Deserialize<Message>(message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            if (msg == null) { return; }

            if (Context.CurrentTextChannelPrivilege != null
                && Context.CurrentTextChannelPrivilege.Read != PrivilegeValue.Positive) {
                if (msg.AuthorID != Context.CurrentTextChannelPrivilege.UserID) {
                    return;
                }
            }

            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                foreach (var guild in mainPage.ViewModel.Guilds) {
                    if (guild.Categories == null) { continue; }
                    foreach (var category in guild.Categories) {
                        if (category.TextChannels == null) { continue; }
                        foreach (TextChannel textChannel in category.TextChannels) {
                            if (msg.ChannelID == textChannel.ID) {

                                foreach (User u in Context.Users) {
                                    if (u.ID == msg.AuthorID) {
                                        msg.Author = u;
                                        break;
                                    }
                                }

                                Context.App.Dispatcher.Invoke(() => {
                                    textChannel.Messages.Add(msg);
                                });

                                return;
                            }
                        }
                    }
                }
            }
        }

        private void SendMessageFail(string message) {
            throw new NotImplementedException();
        }


        private void GetMessageRangeSuccess(string message) {
            if (Context.App.Navigation.TextChannelPage is not TextChannelPage textChannelPage) {
                MessageBox.Show("Channel is null");
                return;
            }

            if (Context.CurrentTextChannelPrivilege != null
                && Context.CurrentTextChannelPrivilege.Read != PrivilegeValue.Positive) {
                Context.App.Dispatcher.Invoke(() => {
                    NotificationPage.Show(Context, "You don't have permission to\nread messages in this channel.");
                });
                return;
            }

            var messages = JsonSerializer.Deserialize<List<Message>>(message);
            if (messages == null || messages.Count == 0) {
                Context.App.Dispatcher.Invoke(() => {
                    textChannelPage.ViewModel.LastMessage = true;
                });
                return;
            }

            foreach (var guild in Context.Guilds) {
                if (guild.Categories == null) { continue; }
                foreach (var category in guild.Categories) {
                    if (category.TextChannels == null) { continue; }
                    foreach (var textChannel in category.TextChannels) {
                        if (textChannel.ID == messages[0].ChannelID) {
                           

                            var scroll = textChannelPage.ViewModel.Scroll;
                            if (scroll != null) {
                                if (textChannel.Messages.Count < 50) {

                                    Context.App.Dispatcher.Invoke(() => {
                                        foreach (var msg in messages) {
                                            if (guild.Users != null) {
                                                foreach (var user in Context.Users) {
                                                    if (user.ID == msg.AuthorID) {
                                                        msg.Author = user;
                                                        break;
                                                    }
                                                }
                                            }
                                            textChannel.Messages.Insert(0, msg);
                                        }

                                        scroll.ScrollToBottom();
                                        Task.Run(() => {
                                            Task.Delay(1000).ContinueWith(t => {
                                                textChannelPage.ViewModel.CanLoadMoreMessages = true;
                                            });
                                        });
                                    });
                                }
                                else {
                                    double currentPosition = scroll.VerticalOffset;
                                    double height = scroll.ExtentHeight - scroll.ViewportHeight;
                                    double percentage = textChannel.Messages.Count / (double)(textChannel.Messages.Count + messages.Count);
                                    double position = height * percentage;
                                    double desiredPosition = height - position;

                                    Task.Delay(1000).ContinueWith(t => {
                                        Context.App.Dispatcher.Invoke(() => {
                                            foreach (var msg in messages) {
                                                if (guild.Users != null) {
                                                    foreach (var user in Context.Users) {
                                                        if (user.ID == msg.AuthorID) {
                                                            msg.Author = user;
                                                            break;
                                                        }
                                                    }
                                                }
                                                textChannel.Messages.Insert(0, msg);
                                            }
                                        });

                                        if (scroll != null) {
                                            Context.App.Dispatcher.Invoke(() => {
                                                if (textChannel.Messages.Count <= 50) {
                                                    scroll.ScrollToBottom();
                                                }
                                                else {
                                                    scroll.ScrollToVerticalOffset(desiredPosition);
                                                }
                                            });
                                        }

                                        textChannelPage.ViewModel.CanLoadMoreMessages = true;
                                    });
                                }
                            }

                            return;
                        }

                        Task.Run(() => {
                            Task.Delay(1000).ContinueWith(t => {
                                textChannelPage.ViewModel.CanLoadMoreMessages = true;
                            });
                        });
                        return;
                    }
                } 
            }

            Task.Run(() => {
                Task.Delay(1000).ContinueWith(t => {
                    textChannelPage.ViewModel.CanLoadMoreMessages = true;
                });
            });
        }

        private void GetMessageRangeFail(string message) {
            MessageBox.Show($"Failed to get messages.\n{message}");
        }



        public void UpdateUserSuccess(string message) {
            UpdateUserData? data = JsonSerializer.Deserialize<UpdateUserData>(message);
            if (data == null) {
                if (Context.App.Navigation.UpdateUserPage is UpdateUserPage page) {
                    page.ViewModel.Feedback = "Something went wrong...";
                }
                return;
            }


            bool success = false;
            Context.App.Dispatcher.Invoke(() => {
                foreach (User user in Context.Users) {
                    if (user.ID == data.UserID) {
                        user.Nickname = data.Nickname;
                        user.Pronoun = data.Pronoun;
                        if (data.ProfilePicture != null) {
                            user.ProfilePicture = data.ProfilePicture;
                            if (Context.App.Navigation.MainPage is MainPage mainPage) {
                                mainPage.ViewModel.UpdateCurrentUserProfilePicture();
                            }
                        }
                        success = true;
                        break;
                    }
                }
            });
            if (success == true) {
                if (Context.App.Navigation.MainPage is MainPage mainPage) {
                    mainPage.ViewModel.HideMask();
                }
            }
            else {
                if (Context.App.Navigation.UpdateUserPage is UpdateUserPage page) {
                    page.ViewModel.Feedback = "Something went wrong...";
                }
            }
        }

        public void UpdateUserFail(string message) {
            if (Context.App.Navigation.UpdateUserPage is UpdateUserPage page) {
                page.ViewModel.Feedback = "Something went wrong...";
            }
        }

        public void UserStatusChanged(string message) {
            UserStatusChangedData? data = JsonSerializer.Deserialize<UserStatusChangedData>(message);
            if (data == null) {
                MessageBox.Show("Invalid user status.");
                return;
            }

            if (Context.CurrentUser is User currentUser && currentUser.ID == data.UserID) {
                currentUser.Status = data.Status;
                return;
            }

            //bool success = false;
            foreach (var user in Context.Users) {
                if (user.ID == data.UserID) {
                    user.Status = data.Status;
                    //success = true;
                    break;
                }
            }
            //if (success == false) {
                //MessageBox.Show("Invalid user in user status handler.");
            //}

        }
    }
}
