using ChatShared.DataModels;
using System.Text.Json;
using ChatServer.Database;
using System.Collections.ObjectModel;
using ChatShared.Models;
using ChatServer.Database.Enums;
using System.Reflection;
using ChatShared;


namespace ChatServer.Network {
    public class ClientOperationHandler {
        public ClientConnection Client { get; }
        public DatabaseConnection Database { get; }

        private const string DEFAULT_FAIL_MESSAGE = "Something went wrong...";

        public ClientOperationHandler(ClientConnection client, DatabaseConnection database) {
            Client = client;
            Database = database;
        }

        public void HandleOperation(OperationCode opCode, string message) {
            switch (opCode) {
                case OperationCode.LogIn: {
                    LogIn(message);
                } break;
                case OperationCode.Register: {
                    Register(message);
                } break;
                case OperationCode.CreateGuild: {
                    CreateGuild(message);
                } break;
                case OperationCode.UpdateGuild: {
                UpdateGuild(message);
                } break;
                case OperationCode.JoinGuild: {
                    JoinGuild(message);
                } break;
                case OperationCode.CreateCategory: {
                    CreateCategory(message);
                } break;
                case OperationCode.UpdateCategory: {
                    UpdateCategory(message);
                } break;
                case OperationCode.CreateTextChannel: {
                    CreateTextChannel(message);
                } break;
                case OperationCode.UpdateTextChannel: {
                    UpdateTextChannel(message);
                } break;
                case OperationCode.GetGuildsForCurrentUser: {
                    GetGuildsForCurrentUser();
                } break;
                case OperationCode.GetGuildDetails: {
                    GetGuildDetails(message);
                } break;
                case OperationCode.SendMessage: {
                    SendMessage(message);
                } break;
                case OperationCode.GetMessageRange: {
                    GetMessageRange(message);
                } break;
                case OperationCode.DeleteGuild: {
                    DeleteGuild(message);
                } break;
                case OperationCode.DeleteCategory: {
                    DeleteCategory(message);
                } break;
                case OperationCode.DeleteTextChannel: {
                    DeleteTextChannel(message);
                } break;
                case OperationCode.UpdateUser: {
                    UpdateUser(message);
                } break;
            }
        }



        private void Register(string message) {
            RegisterData? data = JsonSerializer.Deserialize<RegisterData>(message);
            if (data == null) {
                Logger.Warning($"Failed to register user, data structure was NULL.");
                Client.Unicast(OperationCode.RegisterFail, DEFAULT_FAIL_MESSAGE);
                return;
            }

            RegistrationResult result = Database.Commands.RegisterUser(data.Email, data.Password, data.Nickname, data.Pronoun, data.ProfilePicture);

            if (result == RegistrationResult.Success) {
                Logger.Info($"Successfully registered user with email '{data.Email}'.");
                Client.Unicast(OperationCode.RegisterSuccess, "Successfuly registered user.");
            }
            else if (result == RegistrationResult.UserAlreadyExists) {
                Logger.Warning($"Failed to register user with email '{data.Email}', user already exists.");
                Client.Unicast(OperationCode.RegisterFail, "User already exists.");
            }
            else if (result == RegistrationResult.Fail) {
                Logger.Warning($"Failed to register user with email '{data.Email}'.");
                Client.Unicast(OperationCode.RegisterFail, DEFAULT_FAIL_MESSAGE);
            }
        }



        private void LogIn(string message) {
            try {
                LoginData? data = JsonSerializer.Deserialize<LoginData>(message);
                if (data == null) {
                    string errorMsg = "We couldn't log you in with what you have entered.";
                    Client.Unicast(OperationCode.LogInFail, errorMsg);
                    Logger.Info($"Failed to log in, data was NULL.");
                    return;
                }

                User? user = Database.Commands.LogIn(data.Email, data.Password);
                if (user != null) {
                    Client.User = user;
                    string json = JsonSerializer.Serialize(user);
                    Client.Unicast(OperationCode.LogInSuccess, json);
                    Logger.Info($"Successfully logged user '{user.ID}' with email '{data.Email}'.");

                    json = JsonSerializer.Serialize(new UserStatusChangedData(user.ID, UserStatus.Online));
                    List<ID>? targetUsers = Database.Commands.GetKnownUsers(user.ID);
                    Client.Multicast(OperationCode.UserStatusChanged, json, targetUsers);
                }
                else {
                    string errorMsg = "We couldn't log you in with what you have entered.";
                    Client.Unicast(OperationCode.LogInFail, errorMsg);
                    Logger.Warning($"Failed to log in with email '{data.Email}'.");
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return;
            }
        }



        private void CreateGuild(string message) {
            CreateGuildData? data = JsonSerializer.Deserialize<CreateGuildData>(message);
            if (data == null) {
                Client.Unicast(OperationCode.CreateGuildFail, DEFAULT_FAIL_MESSAGE);
                Logger.Info($"Failed to create guild, data was NULL.");
                return;
            }

            Guild? guild = Database.Commands.CreateGuild(data.OwnerID, data.Name, data.Password, data.Icon, data.DefaultPrivilege);

            if (guild != null) {
                Logger.Info($"Successfully created new guild '{data.Name}'.");
                string json = JsonSerializer.Serialize(guild);
                Client.Unicast(OperationCode.CreateGuildSuccess, json);
            }
            else {
                Logger.Error($"Failed to create guild with name '{data.Name}'.", MethodBase.GetCurrentMethod());
                Client.Unicast(OperationCode.CreateGuildFail, DEFAULT_FAIL_MESSAGE);
            }
        }



        private void CreateCategory(string message) {
            CreateCategoryData? data = JsonSerializer.Deserialize<CreateCategoryData>(message);
            if (data is null) {
                Client.Unicast(OperationCode.CreateCategory, DEFAULT_FAIL_MESSAGE);
                Logger.Info($"Failed to create category, data was NULL.");
                return;
            }

            Category? category = Database.Commands.CreateCategory(data.GuildID, data.Name, data.DefaultPrivilege);

            if (category != null) {
                List<ID>? targetUsers = Database.Commands.GetUsersInCategory(category.ID);
                string json = JsonSerializer.Serialize(category);
                Client.Multicast(OperationCode.CreateCategorySuccess, json, targetUsers);
                Logger.Info($"Successfully created new category with name '{data.Name}'.");
            }
            else {
                Client.Unicast(OperationCode.CreateCategory, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to create category with name '{data.Name}'.");
            }
        }



        private void CreateTextChannel(string message) {
            CreateTextChannelData? data = null;
            try { data = JsonSerializer.Deserialize<CreateTextChannelData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Client.Unicast(OperationCode.CreateTextChannelFail, DEFAULT_FAIL_MESSAGE);
                Logger.Info($"Failed to create text channel, data was NULL.");
                return;
            }

            TextChannel? textChannel = Database.Commands.CreateTextChannel(data.CategoryID, data.Name, data.DefaultPrivilege);

            if (textChannel != null) {
                string json = JsonSerializer.Serialize(textChannel);
                List<ID>? targetUsers = Database.Commands.GetUsersInTextChannel(textChannel.ID);
                Client.Multicast(OperationCode.CreateTextChannelSuccess, json, targetUsers);
                Logger.Info($"Successfully created new text channel \"{data.Name}\".");
            }
            else {
                Client.Unicast(OperationCode.CreateTextChannelFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to create text channel '{data.Name}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void UpdateGuild(string message) {
            UpdateGuildData? data = null;
            try { data = JsonSerializer.Deserialize<UpdateGuildData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Client.Unicast(OperationCode.UpdateGuild, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to update guild, data was NULL.", MethodBase.GetCurrentMethod());
                return;
            }

            var update = Database.Commands.UpdateGuild(data.ID, data.Name, data.Password, data.Icon, data.Privilege);

            if (update != null) {
                string json = JsonSerializer.Serialize(update);
                List<ID>? targetUsers = Database.Commands.GetUsersInGuild(data.ID);
                Client.Multicast(OperationCode.UpdateGuildSuccess, json, targetUsers);
                Logger.Info($"Successfully updated guild '{data.ID}'.");
            }
            else {
                Client.Unicast(OperationCode.UpdateGuildFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to update guild '{data.ID}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void UpdateCategory(string message) {
            UpdateCategoryData? data = null;
            try { data = JsonSerializer.Deserialize<UpdateCategoryData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Client.Unicast(OperationCode.UpdateCategoryFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to update category, data is NULL.", MethodBase.GetCurrentMethod());
                return;
            }

            var update = Database.Commands.UpdateCategory(data.CategoryID, data.Name, data.Privilege);
            if (update != null) {
                string json = JsonSerializer.Serialize(update);
                List<ID>? targetUsers = Database.Commands.GetUsersInCategory(data.CategoryID);
                Client.Multicast(OperationCode.UpdateCategorySuccess, json, targetUsers);
                Logger.Info($"Successfully updated category '{data.CategoryID}'.");
            }
            else {
                Client.Unicast(OperationCode.UpdateCategoryFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to update category '{data.CategoryID}'.", MethodBase.GetCurrentMethod());
            }
        }

        private void UpdateTextChannel(string message) {
            UpdateTextChannelData? data = null;
            try { data = JsonSerializer.Deserialize<UpdateTextChannelData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Client.Unicast(OperationCode.UpdateTextChannelFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to update text channel, data is NULL.", MethodBase.GetCurrentMethod());
                return;
            }

            UpdateTextChannelData? update = Database.Commands.UpdateTextChannel(data.ID, data.Name, data.Privilege);
            if (update != null) {
                string json = JsonSerializer.Serialize(update);
                List<ID>? targetUsers = Database.Commands.GetUsersInTextChannel(data.ID);
                Client.Multicast(OperationCode.UpdateTextChannelSuccess, json, targetUsers);
                Logger.Info($"Successfully updated text channel '{data.ID}'.");
            }
            else {
                Client.Unicast(OperationCode.UpdateTextChannelFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to update text channel '{data.ID}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void GetGuildsForCurrentUser() {
            if (Client.User == null || Client.User.ID == 0) {
                Client.Unicast(OperationCode.GetGuildsForCurrentUserFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to send guilds to user, user ID is 0.", MethodBase.GetCurrentMethod());
                return;
            }

            ObservableCollection<Guild>? guilds = Database.Commands.GetGuildsForUser(Client.User.ID);

            if (guilds != null) {
                string json = JsonSerializer.Serialize(guilds);
                Client.Unicast(OperationCode.GetGuildsForCurrentUserSuccess, json);
                Logger.Info($"Successfully sent {guilds.Count} guilds to user '{Client.User.ID}'.");
            }
            else {
                Client.Unicast(OperationCode.GetGuildsForCurrentUserFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to send guilds to user '{Client.User.ID}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void GetGuildDetails(string message) {
            ID guildID = 0;
            try { guildID = ID.Parse(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (guildID == 0) {
                Client.Unicast(OperationCode.GetGuildDetailsFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to get guild details, guild ID is 0.", MethodBase.GetCurrentMethod());
                return;
            }

            GuildDetails? details = Database.Commands.GetGuildDetails(guildID);

            if (details != null) {
                string json = JsonSerializer.Serialize(details);
                Client.Unicast(OperationCode.GetGuildDetailsSuccess, json);
                Logger.Info($"Successfully sent guild details for guild '{guildID}' to user '{Client.User?.ID}'");
            }
            else {
                Client.Unicast(OperationCode.GetGuildDetailsFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to get guild details for guild '{guildID}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void SendMessage(string message) {
            MessageData? data = null;
            try { data = JsonSerializer.Deserialize<MessageData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Client.Unicast(OperationCode.SendMessageFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to send message, data is NULL.", MethodBase.GetCurrentMethod());
                return;
            }

            Message? msg = Database.Commands.SendMessage(data.UserID, data.TextChannelID, data.Content);
            if (msg != null) {
                string json = JsonSerializer.Serialize(msg);
                List<ID>? targetUsers = Database.Commands.GetUsersInTextChannel(data.TextChannelID);
                Client.Multicast(OperationCode.SendMessageSuccess, json, targetUsers);
                Logger.Info($"Successfully sent message by user '{msg.AuthorID}' to text channel '{data.TextChannelID}'.");
            }
            else {
                Client.Unicast(OperationCode.SendMessageFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to send message by user '{data.UserID}' to text channel '{data.TextChannelID}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void JoinGuild(string message) {
            JoinGuildData? data = null;
            try { data = JsonSerializer.Deserialize<JoinGuildData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Client.Unicast(OperationCode.JoinGuildFail, "");
                Logger.Error("Failed to add user to guild, data is NULL.", MethodBase.GetCurrentMethod());
                return;
            }

            JoiningUser? joiningUser = Database.Commands.JoinGuild(data.UserID, data.PublicID, data.Password);
            if (joiningUser != null) {
                if (joiningUser.Guild == null || joiningUser.User == null) {
                    Client.Unicast(OperationCode.JoinGuildFail, joiningUser.Message);
                    Logger.Warning($"User '{data.UserID}' tried to join guild with public id '{data.PublicID}', but couldn't.");
                }
                else {
                    string json = JsonSerializer.Serialize(joiningUser);
                    List<ID>? targetUsers = Database.Commands.GetUsersInGuild(joiningUser.Guild.ID);
                    Client.Multicast(OperationCode.JoinGuildSuccess, json, targetUsers);
                    Logger.Info($"User '{data.UserID}' joined guild with public id '{data.PublicID}'.");
                }
            }
            else {
                Logger.Error($"Failed to add user '{data.UserID}' to guild '{data.PublicID}'", MethodBase.GetCurrentMethod());
            }
        }



        private void GetMessageRange(string message) {
            MessageRangeData? data = null;
            try { data = JsonSerializer.Deserialize<MessageRangeData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Logger.Error("Failed to get range of messages, data is NULL.", MethodBase.GetCurrentMethod());
                Client.Unicast(OperationCode.GetMessageRangeFail, DEFAULT_FAIL_MESSAGE);
                return;
            }

            List<Message>? messages = Database.Commands.GetMessageRange(data.ChannelID, data.First, data.Limit);
            if (messages != null) {
                string json = JsonSerializer.Serialize(messages);
                Client.Unicast(OperationCode.GetMessageRangeSuccess, json);
                if (messages.Count == 0) {
                    Logger.Warning($"No messages found in given range ({data.First}, {data.Limit}) for user '{data.UserID}' in text channel '{data.ChannelID}'.");
                }
                else {
                    Logger.Info($"Range of {messages.Count} ({data.First}, {data.Limit}) messages sent to user '{data.UserID}'.");
                }
            }
            else {
                Client.Unicast(OperationCode.GetMessageRangeFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to get range of messages ({data.First}, {data.Limit}) for user '{data.UserID}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void DeleteGuild(string message) {
            ID guildID = 0;
            try { guildID = ID.Parse(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (guildID == 0) {
                Client.Unicast(OperationCode.DeleteGuildFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to delete guild, data is NULL", MethodBase.GetCurrentMethod());
                return;
            }
            // get targe users in channel before the guild is deleted
            List<ID>? targetUsers = Database.Commands.GetUsersInGuild(guildID);
            if (Database.Commands.DeleteGuild(guildID)) {
                Client.Multicast(OperationCode.DeleteGuildSuccess, message, targetUsers);
                Logger.Info($"Successfully deleted guild '{guildID}'.");
            }
            else {
                Client.Unicast(OperationCode.DeleteGuildFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to delete guild '{guildID}'.", MethodBase.GetCurrentMethod());
            }
        }



        private void DeleteCategory(string message) {
            ID categoryID = 0;
            try { categoryID = ID.Parse(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (categoryID == 0) {
                Client.Unicast(OperationCode.DeleteCategoryFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to delete category, data is NULL", MethodBase.GetCurrentMethod());
                return;
            }

            // get targe users in channel before the category is deleted
            List<ID>? targetUsers = Database.Commands.GetUsersInCategory(categoryID);
            if (Database.Commands.DeleteCategory(categoryID)) {
                Client.Multicast(OperationCode.DeleteCategorySuccess, message, targetUsers);
                Logger.Info($"Successfully deleted category '{categoryID}'.");
            }
            else {
                Client.Unicast(OperationCode.DeleteCategoryFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to delete category", MethodBase.GetCurrentMethod());
            }
        }



        private void DeleteTextChannel(string message) {
            ID textChannelID = 0;
            try { textChannelID = ID.Parse(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (textChannelID == 0) {
                Client.Unicast(OperationCode.DeleteTextChannelFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error("Failed to delete text channel, data is NULL.", MethodBase.GetCurrentMethod());
                return;
            }

            // get targe users in channel before the channel is deleted
            List<ID>? targetUsers = Database.Commands.GetUsersInTextChannel(textChannelID);
            if (Database.Commands.DeleteTextChannel(textChannelID)) {
                Client.Multicast(OperationCode.DeleteTextChannelSuccess, message, targetUsers);
                Logger.Info($"Successfully deleted text channel '{textChannelID}'.");
            }
            else {
                Client.Unicast(OperationCode.DeleteTextChannelFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to delete text channel.", MethodBase.GetCurrentMethod());
            }
        }



        private void UpdateUser(string message) {
            UpdateUserData? data = null;
            try { data = JsonSerializer.Deserialize<UpdateUserData>(message); }
            catch (Exception ex) { Logger.Error(ex, MethodBase.GetCurrentMethod()); }
            if (data == null) {
                Client.Unicast(OperationCode.UpdateUserFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to update user, data is NULL.", MethodBase.GetCurrentMethod());
                return;
            }

            if (Database.Commands.UpdateUser(data.UserID, data.Nickname, data.Pronoun, data.ProfilePicture)) {
                List<ID>? targetUsers = Database.Commands.GetKnownUsers(data.UserID);
                targetUsers?.Add(data.UserID);
                Client.Multicast(OperationCode.UpdateUserSuccess, message, targetUsers);
                Logger.Info($"Successfully updated user '{data.UserID}'.");
            }
            else {
                Client.Unicast(OperationCode.UpdateUserFail, DEFAULT_FAIL_MESSAGE);
                Logger.Error($"Failed to update user '{data.UserID}'.", MethodBase.GetCurrentMethod());
            }
        }


    }
}
