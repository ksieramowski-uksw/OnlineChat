using ChatClient.MVVM.View;
using ChatClient.MVVM.View.Main;
using ChatClient.Stores;
using ChatShared.Models;
using System.Collections.ObjectModel;
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
                case OperationCode.CompleteGuildInfoSuccess: {
                    CompleteGuildInfoSuccess(message);
                } break;
                case OperationCode.CompleteGuildInfoFail: {
                    CompleteGuildsInfoFail(message);
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
                        mainPage.ViewModel.Guilds.Add(guild);
                    });
                    mainPage.ViewModel.MaskVisibility = Visibility.Hidden;
                    if (App.Current.NavigationStore.CreateOrJoinGuildPage is CreateOrJoinGuildPage createOrJoinGuildPage) {
                        createOrJoinGuildPage.ViewModel.NewGuildName = string.Empty;
                        createOrJoinGuildPage.ViewModel.NewGuildPassword = string.Empty;
                        createOrJoinGuildPage.ViewModel.NewGuildFeedback = string.Empty;
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

        private void CompleteGuildInfoSuccess(string message) {
            Guild? guild = JsonSerializer.Deserialize<Guild>(message);
            if (guild is null) { return; }
            if (App.Current.NavigationStore.MainPage is MainPage mainPage) {
                bool found = false;
                foreach (Guild g in mainPage.ViewModel.Guilds) {
                    if (guild.PublicId == g.PublicId) {
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

        private void CompleteGuildsInfoFail(string message) {
            MessageBox.Show("Failed to fetch \"CompleteGuildInfo\".\n" + message);
        }
    }
}
