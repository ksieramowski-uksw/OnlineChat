using ChatClient.MVVM.Model;
using ChatClient.MVVM.View;
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
                    break;
                }
                case OperationCode.LogInFail: {
                    LoginFail(message);
                    break;
                }
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
                    //mainPage.GuildListFrame.Content = new GuildListPage();
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


    }
}
