using ChatClient.MVVM.Model;
using ChatShared.DataModels;
using System.Text.Json;


namespace ChatClient.Network {
    public class Client {
        public readonly Config.Config Config;

        public readonly ServerConnection ServerConnection;

        public User? User { get; set; }

        public Client() {
            Config = new();
            ServerConnection = new(Config);
        }

        public void Connect() {
            ServerConnection.Connect();
        }

        public void LogIn(string email, string password) {
            var loginData = new LoginData(email, password);
            string json = JsonSerializer.Serialize(loginData);
            ServerConnection.Send(OperationCode.LogIn, json);
        }

        public void Register(string email, string password,
            string confirmPassword, string nickname) {
            var registerData = new RegisterData(email, password, confirmPassword, nickname);
            string json = JsonSerializer.Serialize(registerData);
            ServerConnection.Send(OperationCode.Register, json);
        }
    }
}
