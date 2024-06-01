using ChatShared.DataModels;
using System.Text.Json;
using ChatShared.Models.Privileges;
using ChatShared;


namespace ChatClient.Network {
    public class Client {
        public readonly ChatContext Context;

        public readonly Config Config;

        public readonly ServerConnection ServerConnection;

        public Client(ChatContext context) {
            Context = context;
            Config = new();
            ServerConnection = new ServerConnection(this);
        }

        public void Connect() {
            ServerConnection.Connect();
        }

        public void Register(string email, string password, string confirmPassword, string nickname, string pronoun) {
            var profilePicture = ResourceHelper.GetScaledImagePixels(ResourceHelper.DefaultImage);
            var registerData = new RegisterData(email, password, confirmPassword, nickname, pronoun, profilePicture);
            string json = JsonSerializer.Serialize(registerData);
            ServerConnection.Send(OperationCode.Register, json);
        }

        public void LogIn(string email, string password) {
            var loginData = new LoginData(email, password);
            string json = JsonSerializer.Serialize(loginData);
            ServerConnection.Send(OperationCode.LogIn, json);
        }



        public void GetGuildsForCurrentUser() {
            if (Context.CurrentUser == null) { return; }
            ServerConnection.Send(OperationCode.GetGuildsForCurrentUser, Context.CurrentUser.ID.ToString());
        }

        public void GetGuildDetails(ID guildID) {
            ServerConnection.Send(OperationCode.GetGuildDetails, guildID.ToString());
        }

        public void JoinGuild(string publicId, string password) {
            if (Context.CurrentUser != null) {
                JoinGuildData createData = new(Context.CurrentUser.ID, publicId, password);
                string json = JsonSerializer.Serialize(createData);
                ServerConnection.Send(OperationCode.JoinGuild, json);
            }
        }



        public void CreateGuild(string name, string password, GuildPrivilege defaultPrivilege, string iconPath) {
            if (Context.CurrentUser != null) {
                var icon = ResourceHelper.GetScaledImagePixels(iconPath);
                CreateGuildData createData = new(Context.CurrentUser.ID, name, password, icon, defaultPrivilege);
                string json = JsonSerializer.Serialize(createData);
                ServerConnection.Send(OperationCode.CreateGuild, json);
            }
        }

        public void CreateCategory(ID guildID, string categoryName, CategoryPrivilege defaultCategoryPrivilege) {
            CreateCategoryData data = new(guildID, categoryName, defaultCategoryPrivilege);
            string json = JsonSerializer.Serialize(data);
            ServerConnection.Send(OperationCode.CreateCategory, json);
        }

        public void CreateTextChannel(ID categoryID, string textChannelName, TextChannelPrivilege defaultTextChannelPrivilege) {
            CreateTextChannelData data = new(categoryID, textChannelName, defaultTextChannelPrivilege);
            string json = JsonSerializer.Serialize(data);
            ServerConnection.Send(OperationCode.CreateTextChannel, json);
        }



        public void UpdateGuild(ID id, string name, string password, GuildPrivilege defaultGuildPrivilege, string iconPath) {
            UpdateGuildData data;
            if (iconPath == "Default") {
                data = new UpdateGuildData(id, name, password, null, defaultGuildPrivilege);
            }
            else {
                var icon = ResourceHelper.GetScaledImagePixels(iconPath);
                data = new UpdateGuildData(id, name, password, icon, defaultGuildPrivilege);
            }
            string json = JsonSerializer.Serialize(data);
            ServerConnection.Send(OperationCode.UpdateGuild, json);
        }

        public void UpdateCategory(ID categoryID, string categoryName, CategoryPrivilege defaultCategoryPrivilege) {
            UpdateCategoryData data = new(categoryID, categoryName, defaultCategoryPrivilege);
            string json = JsonSerializer.Serialize(data);
            ServerConnection.Send(OperationCode.UpdateCategory, json);
        }

        public void UpdateTextChannel(ID textChannelID, string textChannelName, TextChannelPrivilege defaultTextChannelPrivilege) {
            UpdateTextChannelData data = new(textChannelID, textChannelName, defaultTextChannelPrivilege);
            string json = JsonSerializer.Serialize(data);
            ServerConnection.Send(OperationCode.UpdateTextChannel, json);
        }



        public void DeleteGuild(ID guildID) {
            ServerConnection.Send(OperationCode.DeleteGuild, guildID.ToString());
        }

        public void DeleteCategory(ID categoryID) {
            ServerConnection.Send(OperationCode.DeleteCategory, categoryID.ToString());
        }

        public void DeleteTextChannel(ID textChannelID) {
            ServerConnection.Send(OperationCode.DeleteTextChannel, textChannelID.ToString());
        }



        public void SendMessage(ID userID, ID textChannelID, string message) {
            MessageData data = new(userID, textChannelID, message.Trim());
            string json = JsonSerializer.Serialize(data);
            Context.Client.ServerConnection.Send(OperationCode.SendMessage, json);
        }

        public void GetMessageRange(ID channelID, ID first, byte limit) {
            if (Context.CurrentUser != null) {
                MessageRangeData data = new(channelID, first, limit, Context.CurrentUser.ID);
                string json = JsonSerializer.Serialize(data);
                ServerConnection.Send(OperationCode.GetMessageRange, json);
            }
        }



        public void UpdateUser(ID userID, string nickname, string pronoun, string profilePicturePath) {
            UpdateUserData data;
            if (profilePicturePath == "Default") {

                data = new UpdateUserData(userID, nickname, pronoun, null);
            }
            else {
                var picture = ResourceHelper.GetScaledImagePixels(profilePicturePath);
                data = new UpdateUserData(userID, nickname, pronoun, picture);
            }
            string json = JsonSerializer.Serialize(data);
            ServerConnection.Send(OperationCode.UpdateUser, json);
        }


    }
}
