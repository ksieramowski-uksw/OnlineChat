using ChatShared.DataModels;
using System.Text.Json;
using System.IO;
using ChatShared.Models.Privileges;
using ChatShared.Models;


namespace ChatClient.Network {
    public class Client {
        public readonly ChatContext Context;

        public readonly Config.Config Config;

        public readonly ServerConnection ServerConnection;

        //public User? User { get; set; }

        public Client(ChatContext context) {
            Context = context;
            Config = new();
            ServerConnection = new ServerConnection(this);
        }

        public void Connect() {
            ServerConnection.Connect();
        }

        public void LogIn(string email, string password) {
            var loginData = new LoginData(email, password);
            string json = JsonSerializer.Serialize(loginData);
            ServerConnection.Send(OperationCode.LogIn, json);
        }

        public void Register(string email, string password, string confirmPassword, string nickname, string pronoun) {
            var profilePicture = ResourceHelper.GetImagePixels($"{Directory.GetCurrentDirectory()}\\Resources\\Images\\942840997837168660.png");
            var registerData = new RegisterData(email, password, confirmPassword, nickname, pronoun, profilePicture);
            string json = JsonSerializer.Serialize(registerData);
            ServerConnection.Send(OperationCode.Register, json);
        }

        public void JoinGuild(string publicId, string password) {
            if (Context.CurrentUser != null) {
                JoinGuildData createData = new(publicId, password, Context.CurrentUser.ID);
                string json = JsonSerializer.Serialize(createData);
                ServerConnection.Send(OperationCode.JoinGuild, json);
            }
        }

        public void CreateGuild(string name, string password, GuildPrivilege defaultPrivilege) {
            if (Context.CurrentUser != null) {
                var icon = ResourceHelper.GetImagePixels($"{Directory.GetCurrentDirectory()}\\Resources\\Images\\942840997837168660.png");
                CreateGuildData createData = new(Context.CurrentUser.ID, name, password, icon, defaultPrivilege);
                string json = JsonSerializer.Serialize(createData);
                ServerConnection.Send(OperationCode.CreateGuild, json);
            }
        }

        public void CreateCategory(ulong guildID, string categoryName, CategoryPrivilege defaultPrivilege) {
            CreateCategoryData data = new(guildID, categoryName, defaultPrivilege);
            string json = JsonSerializer.Serialize(data);
            ServerConnection.Send(OperationCode.CreateCategory, json);
        }

        public void CreateTextChannel(ulong categoryID, string textChannelName, TextChannelPrivilege privilege) {
            CreateTextChannelData data = new(categoryID, textChannelName, privilege);
            string json = JsonSerializer.Serialize(data);
            Context.Client.ServerConnection.Send(OperationCode.CreateTextChannel, json);
        }

        public void GetMessageRange(ulong channelID, ulong first, byte limit) {
            if (Context.CurrentUser != null) {
                MessageRangeData data = new(channelID, first, limit, Context.CurrentUser.ID);
                string json = JsonSerializer.Serialize(data);
                ServerConnection.Send(OperationCode.GetMessageRange, json);
            }
        }




    }
}
