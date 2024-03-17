using ChatClient;
using ChatShared.DataModels;
using Newtonsoft.Json;


namespace ChatServer.Network {
    public partial class ClientConnection {
        private class ClientOperationHandler {
            private readonly ClientConnection _client;
            private readonly Server _server;
            private readonly Database _database;

            public ClientOperationHandler(ClientConnection client) {
                _client = client;
                _server = client._server;
                _database = client._server.Database;
            }

            public void HandleOperation(OperationCode opCode, string message) {
                switch (opCode) {
                    case OperationCode.LogIn: {
                        LogIn(message);
                        break;
                    }
                    case OperationCode.LogOut: {
                        LogOut(message);
                        break;
                    }
                    case OperationCode.Register: {
                        Register(message);
                        break;
                    }
                }
            }

            private async void LogIn(string message) {
                LoginData? data = JsonConvert.DeserializeObject<LoginData>(message);
                if (data == null) {
                    string errorMsg = "We couldn't log you in with what you have entered.";
                    await _client.Send(OperationCode.LogInFail, errorMsg);
                    Logger.Info($"Failed to log in, data was NULL.");
                    return;
                }

                ClientData? clientData = _database.TryLogIn(data);
                if (clientData != null) {
                    string json = JsonConvert.SerializeObject(clientData);
                    await _client.Send(OperationCode.LogInSuccess, json);
                    Logger.Info($"Succesfully logged in with email \"{data.Email}\".");
                }
                else {
                    string errorMsg = "We couldn't log you in with what you have entered.";
                    await _client.Send(OperationCode.LogInFail, errorMsg);
                    Logger.Error($"Failed to log in with email \"{data.Email}\".");
                }
            }

            private void LogOut(string message) {

            }


            private void Register(string message) {
                RegisterData? data = JsonConvert.DeserializeObject<RegisterData>(message);
                if (data == null) {
                    Logger.Info($"Failed to register user, data was NULL.");
                    return;
                }

                if (_database.TryRegisterNewUser(data)) {
                    Logger.Info($"Succesfully registered new user: {data.Nickname}.");
                }
                else {
                    Logger.Error($"Failed to register user: {data.Nickname}.");
                }
            }

        }




    }

}
