using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;
using ChatClient.MVVM.View.Main.Popup;
using ChatClient.Stores;
using ChatShared.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows;


namespace ChatClient.Network {
    public class ServerOperationHandler {

        private readonly ServerConnection _serverConnection;

        public ServerOperationHandler(ServerConnection serverConnection) {
            _serverConnection = serverConnection;
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
                case OperationCode.CreateTextChannelSuccess: {
                    CreateTextChannelSuccess(message);
                } break;
                case OperationCode.CreateTextChannelFail: {
                    CreateTextChannelFail(message);
                } break;
                case OperationCode.GetGuildsForUserSuccess: {
                    GetGuildsForUserSuccess(message);
                } break;
                case OperationCode.GetGuildsForUserFail: {
                    GetGuildsForUserFail(message);
                } break;
                case OperationCode.SendMessage: {
                    SendMessage(message);
                } break;
                case OperationCode.GetMessageRangeSuccess: {
                    GetMessageRangeSuccess(message);
                } break;
                case OperationCode.GetMessageRangeFail: {
                    GetMessageRangeFail(message);
                } break;
            }
        }

        private void LoginSuccess(string message) {
            var app = App.Current;
            var client = app.Client;
            var navigationStore = app.NavigationStore;

            if (navigationStore.MainWindow == null) {
                client.User = JsonSerializer.Deserialize<User>(message);
                if (client.User == null) {
                    if (navigationStore.LoginPage != null) {
                        string errorMsg = "Something went wrong...";
                        navigationStore.LoginPage.ViewModel.Feedback = errorMsg;
                    }
                    return;
                }

                app.Dispatcher.Invoke(() => {
                    Window previousWindow = app.MainWindow;
                    navigationStore.MainWindow = new MainWindow();
                    MainPage mainPage = new(navigationStore);
                    navigationStore.MainPage = mainPage;
                    navigationStore.MainWindow.MainFrame.Content = mainPage;
                    app.MainWindow = navigationStore.MainWindow;

                    previousWindow.Close();
                    navigationStore.LoginWindow = null;
                    navigationStore.LoginPage = null;
                    navigationStore.RegisterPage = null;

                    app.MainWindow.Show();
                });

            }
        }   
        
        private void LoginFail(string message) {
            if (App.Current.NavigationStore.LoginPage != null) {
                App.Current.NavigationStore.LoginPage.ViewModel.Feedback = message;
            }
        }

        private void RegisterSuccess(string message) {
            if (App.Current.NavigationStore.LoginPage != null) {
                App.Current.NavigationStore.LoginPage.ViewModel.Feedback = message;
                if (App.Current.NavigationStore.LoginWindow is LoginWindow loginWindow) {
                    App.Current.Dispatcher.Invoke(() => {
                        loginWindow.MainFrame.Navigate(App.Current.NavigationStore.LoginPage);
                    });
                }
            }
        }

        private void RegisterFail(string message) {
            if (App.Current.NavigationStore.RegisterPage != null) {
                App.Current.NavigationStore.RegisterPage.ViewModel.Feedback = message;
            }
        }

        private void CreateGuildSuccess(string message) {
            if (App.Current.NavigationStore.MainPage is MainPage mainPage) {
                if (JsonSerializer.Deserialize<Guild>(message) is Guild guild) {
                    App.Current.Dispatcher.Invoke(() => {
                        bool resourceFound = false;
                        int index = 0;
                        for (; index < App.Current.ResourceStorage.Count; index++) {
                            if (App.Current.ResourceStorage[index] == guild.Icon) {
                                resourceFound = true;
                                break;
                            }
                        }
                        if (resourceFound) {
                            guild.Icon = App.Current.ResourceStorage[index];
                        }
                        else {
                            App.Current.ResourceStorage.Add(guild.Icon);
                        }
                        mainPage.ViewModel.Guilds.Add(guild);
                    });
                    mainPage.ViewModel.MaskVisibility = Visibility.Hidden;
                    if (App.Current.NavigationStore.CreateOrJoinGuildPage is CreateOrJoinGuildPage createOrJoinGuildPage) {
                        createOrJoinGuildPage.ViewModel.NewGuildName = string.Empty;
                        createOrJoinGuildPage.ViewModel.NewGuildPassword = string.Empty;
                        createOrJoinGuildPage.ViewModel.NewGuildFeedback = string.Empty;

                        App.Current.NavigationStore.CreateOrJoinGuildPage = null;
                    }
                }
                else {
                    if (App.Current.NavigationStore.CreateOrJoinGuildPage is CreateOrJoinGuildPage createOrJoinGuildPage) {
                        createOrJoinGuildPage.ViewModel.NewGuildFeedback = "Response is corrupted.";
                    }
                }
            }
        }

        private void CreateGuildFail(string message) {
            if (App.Current.NavigationStore.CreateOrJoinGuildPage is CreateOrJoinGuildPage createOrJoinGuildPage) {
                createOrJoinGuildPage.ViewModel.NewGuildFeedback = message;
            }
        }


        private void JoinGuildSuccess(string message) {
            if (JsonSerializer.Deserialize<Guild>(message) is Guild guild) {
                if (App.Current.NavigationStore.MainPage is MainPage mainPage) {
                    bool found = false;
                    foreach (Guild g in mainPage.ViewModel.Guilds) {
                        if (guild.PublicID == g.PublicID) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        App.Current.Dispatcher.Invoke(() => {
                            bool resourceFound = false;
                            int index = 0;
                            for (; index < App.Current.ResourceStorage.Count; index++) {
                                if (App.Current.ResourceStorage[index] == guild.Icon) {
                                    resourceFound = true;
                                    break;
                                }
                            }
                            if (resourceFound) {
                                guild.Icon = App.Current.ResourceStorage[index];
                            }
                            else {
                                App.Current.ResourceStorage.Add(guild.Icon);
                            }
                            mainPage.ViewModel.Guilds.Add(guild);
                        });
                    }
                }
            }
        }

        private void JoinGuildFail(string message) {

        }

        private void CreateCategorySuccess(string message) {
            if (App.Current.NavigationStore.MainPage is MainPage mainPage &&
                App.Current.NavigationStore.GuildPage is GuildPage guildPage) {
                if (JsonSerializer.Deserialize<Category>(message) is Category category) {
                    App.Current.Dispatcher.Invoke(() => {
                        guildPage.ViewModel.Guild.Categories.Add(category);
                    });
                    mainPage.ViewModel.MaskVisibility = Visibility.Hidden;
                    if (App.Current.NavigationStore.CreateCategoryPage is CreateCategoryPage createCategoryPage) {
                        createCategoryPage.ViewModel.NewCategoryName = string.Empty;
                        createCategoryPage.ViewModel.Feedback = string.Empty;

                        App.Current.NavigationStore.CreateCategoryPage = null;
                    }
                }
                else {
                    if (App.Current.NavigationStore.CreateCategoryPage is CreateCategoryPage createCategoryPage) {
                        createCategoryPage.ViewModel.Feedback = "Response is corrupted.";
                    }
                }
            }
        }

        private void CreateCategoryFail(string message) {
            if (App.Current.NavigationStore.CreateCategoryPage is CreateCategoryPage createCategoryPage) {
                createCategoryPage.ViewModel.Feedback = message;
            }
        }
        private void CreateTextChannelSuccess(string message) {
            if (App.Current.NavigationStore.MainPage is MainPage mainPage &&
                App.Current.NavigationStore.GuildPage is GuildPage guildPage) {
                if (JsonSerializer.Deserialize<TextChannel>(message) is TextChannel textChannel) {
                    App.Current.Dispatcher.Invoke(() => {
                        foreach (Category category in guildPage.ViewModel.Guild.Categories) {
                            if (textChannel.CategoryID == category.ID) {
                                category.TextChannels.Add(textChannel);
                                break;
                            }
                        }
                    });
                    mainPage.ViewModel.MaskVisibility = Visibility.Hidden;
                    if (App.Current.NavigationStore.CreateTextChannelPage is CreateTextChannelPage createTextChannelPage) {
                        createTextChannelPage.ViewModel.NewTextChannelName = string.Empty;
                        createTextChannelPage.ViewModel.Feedback = string.Empty;

                        App.Current.NavigationStore.CreateTextChannelPage = null;
                    }
                }
                else {
                    if (App.Current.NavigationStore.CreateTextChannelPage is CreateTextChannelPage createTextChannelPage) {
                        createTextChannelPage.ViewModel.Feedback = "Response is corrupted.";
                    }
                }
            }
        }

        private void CreateTextChannelFail(string message) {
            if (App.Current.NavigationStore.CreateTextChannelPage is CreateTextChannelPage createTextChannelPage) {
                createTextChannelPage.ViewModel.Feedback = message;
            }
        }

        private void GetGuildsForUserSuccess(string message) {
            Guild? guild = JsonSerializer.Deserialize<Guild>(message);
            if (guild is null) { return; }
            if (App.Current.NavigationStore.MainPage is MainPage mainPage) {
                bool found = false;
                foreach (Guild g in mainPage.ViewModel.Guilds) {
                    if (guild.PublicID == g.PublicID) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    App.Current.Dispatcher.Invoke(() => {
                        bool resourceFound = false;
                        int index = 0;
                        for (;index < App.Current.ResourceStorage.Count; index++) {
                            if (App.Current.ResourceStorage[index] == guild.Icon) {
                                resourceFound = true;
                                break;
                            }
                        }
                        if (resourceFound) {
                            guild.Icon = App.Current.ResourceStorage[index];
                        }
                        else {
                            App.Current.ResourceStorage.Add(guild.Icon);
                        }
                        mainPage.ViewModel.Guilds.Add(guild);
                    });
                }
            }
        }

        private void GetGuildsForUserFail(string message) {
            MessageBox.Show($"Failed to fetch \"CompleteGuildInfo\".\n[{message}]");
        }

        private void SendMessage(string message) {
            Message? msg = JsonSerializer.Deserialize<Message>(message);
            if (msg == null) { return; }
            if (App.Current.NavigationStore.MainPage is MainPage mainPage) {
                foreach (var guild in mainPage.ViewModel.Guilds) {
                    foreach (var category in guild.Categories) {
                        foreach (TextChannel textChannel in category.TextChannels) {
                            if (msg.ChannelID == textChannel.ID) {
                                App.Current.Dispatcher.Invoke(() => {
                                    textChannel.Messages.Add(msg);
                                });
                                return;
                            }
                        }
                    }
                }
            }
            
        }

        private void GetMessageRangeSuccess(string message) {
            if (JsonSerializer.Deserialize<ObservableCollection<Message>>(message)
                is ObservableCollection<Message> messages) {
                if (messages.Count == 0) { return; }

                if (App.Current.NavigationStore.MainPage is MainPage mainPage) {
                    foreach (var guild in mainPage.ViewModel.Guilds) {
                        foreach (var category in guild.Categories) {
                            foreach (var textChannel in category.TextChannels) {
                                if (textChannel.ID == messages[0].ChannelID) {
                                    App.Current.Dispatcher.Invoke(() => {
                                        ObservableCollection<Message> combined = new(messages
                                        .Concat(textChannel.Messages)
                                        .GroupBy(x => x.ID)
                                        .Select(g => g.First())
                                        .OrderBy(x => x.ID));
                                        // for some reason reasigning collection doesn't trigger binding's OnPropertyChanged
                                        // but only happens on first use of fetch 
                                        textChannel.Messages.Clear();
                                        foreach (var message in combined) {
                                            textChannel.Messages.Add(message);
                                        }
                                    });
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GetMessageRangeFail(string message) {
            MessageBox.Show($"Failed to get messages.\n{message}");

        }
    }
}
