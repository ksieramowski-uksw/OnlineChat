using ChatShared.DataModels;
using System.Text.Json;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ChatShared.Models;


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

        public void Register(string email, string password, string confirmPassword, string nickname) {
            var profilePicture = ResourceHelper.GetImagePixels($"{Directory.GetCurrentDirectory()}\\Resources\\Images\\942840997837168660.png");
            var registerData = new RegisterData(email, password, confirmPassword, nickname, profilePicture);
            string json = JsonSerializer.Serialize(registerData);
            ServerConnection.Send(OperationCode.Register, json);
        }

        public void CreateGuild(string name, string password) {
            if (User != null) {
                var icon = ResourceHelper.GetImagePixels($"{Directory.GetCurrentDirectory()}\\Resources\\Images\\942840997837168660.png");
                CreateGuildData createData = new(User.Id, name, password, icon);
                string json = JsonSerializer.Serialize(createData);
                ServerConnection.Send(OperationCode.CreateGuild, json);
            }
        }

        public void JoinGuild(string publicId, string password) {
            if (User != null) {
                JoinGuildData createData = new(publicId, password);
                string json = JsonSerializer.Serialize(createData);
                ServerConnection.Send(OperationCode.JoinGuild, json);
            }
        }






    }
}
