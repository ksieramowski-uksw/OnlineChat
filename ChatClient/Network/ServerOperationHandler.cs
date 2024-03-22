using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
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

                    break;
                }
                case OperationCode.LogInFail: {
                    if (App.Current is App app && app.NavigationStore.LoginPageViewModel != null) {
                        app.NavigationStore.LoginPageViewModel.LoginFeedback = message;
                    }
                    else {
                        MessageBox.Show("null");
                    }
                    break;
                }
            }
        }
    }
}
