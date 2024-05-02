using ChatClient;
using ChatShared.DataModels;
using System.Text.Json;
using ChatServer.Database;
using System.Collections.ObjectModel;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using ChatServer.Database.Enums;


namespace ChatServer.Network
{
    public partial class ClientConnection {
        private class ClientOperationHandler {
            private readonly ClientConnection _client;
            private readonly DatabaseConnection _database;

            public ClientOperationHandler(ClientConnection client) {
                _client = client;
                _database = client._server.Database;
            }

            public void HandleOperation(OperationCode opCode, string message) {
                switch (opCode) {
                    case OperationCode.LogIn: {
                        LogIn(message);
                    } break;
                    case OperationCode.LogOut: {
                        LogOut(message);
                    } break;
                    case OperationCode.Register: {
                        Register(message);
                    } break;
                    case OperationCode.CreateGuild: {
                        CreateGuild(message);
                    } break;
                    case OperationCode.JoinGuild: {
                        JoinGuild(message);
                    } break;
                    case OperationCode.CreateCategory: {
                        CreateCategory(message);
                    } break;
                    case OperationCode.CreateTextChannel: {
                        CreateTextChannel(message);
                    } break;
                    case OperationCode.GetGuildsForUser: {
                        GetGuildsForUser(message);
                    } break;
                    case OperationCode.GetCategoriesInGuild: {
                        GetCategoriesInGuild(message);
                    } break;
                    case OperationCode.GetTextChannelsInCategory: {
                        GetTextChannelsInCategory(message);
                    } break;
                    case OperationCode.SendMessage: {
                        SendMessage(message);
                    } break;
                    case OperationCode.GetGuildPrivilegeForUser: {
                        GetGuildPrivilegesForUser(message);
                    } break;
                    case OperationCode.GetCategoryPrivilegeForUser: {
                        GetCategoryPrivilegesForUser(message);
                    } break;
                    case OperationCode.GetTextChannelPrivilegeForUser: {
                        GetTextChannelPrivilegesForUser(message);
                    } break;
                    case OperationCode.GetFriendList: {
                        GetFriendList(message);
                    } break;
                    case OperationCode.AddFriend: {
                        AddFriend(message);
                    } break;
                    case OperationCode.RemoveFriend: {
                        RemoveFriend(message);
                    } break;
                    case OperationCode.GetMessageRange: {
                        GetMessageRange(message);
                    } break;
                }
            }

            private void LogIn(string message) {
                try {
                    LoginData? data = JsonSerializer.Deserialize<LoginData>(message);
                    if (data == null) {
                        string errorMsg = "We couldn't log you in with what you have entered.";
                        _client.Send(OperationCode.LogInFail, errorMsg);
                        Logger.Info($"Failed to log in, data was NULL.");
                        return;
                    }

                    DatabaseCommandResult result = _database.Commands.TryLogIn(data, out User? user);

                    if (result == DatabaseCommandResult.Success && user != null) {
                        string json = JsonSerializer.Serialize(user);
                        _client.User = user;
                        Logger.Info($"Successfully logged user \"{user.Nickname}#{user.ID}\" with email \"{data.Email}\".");
                        _client.Send(OperationCode.LogInSuccess, json);
                    }
                    else if (result == DatabaseCommandResult.Fail) {
                        string errorMsg = "We couldn't log you in with what you have entered.";
                        Logger.Warning($"Failed to log in with email \"{data.Email}\"].");
                        _client.Send(OperationCode.LogInFail, errorMsg);
                    }
                    else if (result == DatabaseCommandResult.DatabaseError) {
                        const string error = "DATABASE_ERROR";
                        Logger.Warning($"Failed to log in with email \"{data.Email}\", [{error}].");
                        _client.Send(OperationCode.LogInFail, "Something went wrong...");
                    }
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                }
            }

            private void LogOut(string message) {

            }


            private void Register(string message) {
                RegisterData? data = JsonSerializer.Deserialize<RegisterData>(message);
                if (data == null) {
                    Logger.Warning($"Failed to register user, data structure was NULL.");
                    _client.Send(OperationCode.RegisterFail, "Registration failed...");
                    return;
                }

                DatabaseCommandResult result = _database.Commands.RegisterUser(data);

                if (result == DatabaseCommandResult.Success) {
                    Logger.Info($"Successfully registered user with email \"{data.Email}\".");
                    _client.Send(OperationCode.RegisterSuccess, "Successfuly registered user.");
                }
                else if (result == DatabaseCommandResult.UserExists) {
                    const string error = "USER_ALREADY_EXISTS";
                    Logger.Warning($"Failed to registered user with email \"{data.Email}\", [{error}].");
                    _client.Send(OperationCode.RegisterFail, "User already exists.");
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Warning($"Failed to registered user with email \"{data.Email}\", [{error}].");
                    _client.Send(OperationCode.RegisterFail, "Something went wrong...");
                }
            }



            private void CreateGuild(string message) {
                CreateGuildData? data = JsonSerializer.Deserialize<CreateGuildData>(message);
                if (data is null) {
                    Logger.Info($"Failed to create guild, data was NULL.");
                    return;
                }

                DatabaseCommandResult result = _database.Commands.CreateGuild(data, out Guild? guild);

                if (result == DatabaseCommandResult.Success && guild != null) {
                    Logger.Info($"Successfully created new guild \"{data.Name}\".");
                    string json = JsonSerializer.Serialize(guild);
                    _client.Send(OperationCode.CreateGuildSuccess, json);
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to create guild \"{data.Name}\", [{error}].");
                    _client.Send(OperationCode.CreateGuildFail, error);
                }
                else if (result == DatabaseCommandResult.Fail) {
                    const string error = "UNKNOWN_ERROR";
                    Logger.Error($"Failed to create guild \"{data.Name}\", [{error}].");
                    _client.Send(OperationCode.CreateGuildFail, error);
                }
            }


            private void CreateCategory(string message) {
                CreateCategoryData? data = JsonSerializer.Deserialize<CreateCategoryData>(message);
                if (data is null) {
                    Logger.Info($"Failed to create category, data was NULL.");
                    return;
                }

                DatabaseCommandResult result = _database.Commands.CreateCategory(data, out Category? category);

                if (result == DatabaseCommandResult.Success && category is not null) {
                    Logger.Info($"Successfully created new category \"{data.Name}\".");
                    List<ulong>? targetUsers;
                    result = _database.Commands.GetUsersInCategory(category, out targetUsers);

                    if (result == DatabaseCommandResult.Success && targetUsers is not null) {
                        string json = JsonSerializer.Serialize(category);
                        _client.MultiCast(OperationCode.CreateCategorySuccess, json, (x) => {
                            foreach (ulong id in targetUsers) {
                                if (x.User is User user && user.ID == id) {
                                    return true;
                                }
                            }
                            return false;
                        });
                    }
                    else {
                        Logger.Error($"Failed to multicast category creation \"{data.Name}\" in guild \"{data.GuildID}\"");
                    }
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    Logger.Error($"Failed to create category \"{data.Name}\", [DATABASE_ERROR].");
                }
            }


            private void CreateTextChannel(string message) {
                CreateTextChannelData? data = JsonSerializer.Deserialize<CreateTextChannelData>(message);
                if (data is null) {
                    Logger.Info($"Failed to create text channel, data was NULL.");
                    return;
                }

                DatabaseCommandResult result = _database.Commands.CreateTextChannel(data, out TextChannel? textChannel);

                if (result == DatabaseCommandResult.Success && textChannel != null) {
                    Logger.Info($"Successfully created new text channel \"{data.Name}\".");
                    string json = JsonSerializer.Serialize(textChannel);
                    _client.Send(OperationCode.CreateTextChannelSuccess, json);
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to create text channel \"{data.Name}\", [{error}].");
                    _client.Send(OperationCode.CreateTextChannelFail, error);
                }
                else {
                    const string error = "UNKNOWN_ERROR";
                    Logger.Error($"Failed to create text channel \"{data.Name}\", [{error}].");
                    _client.Send(OperationCode.CreateTextChannelFail, error);
                }
            }


            private void GetGuildsForUser(string message) {
                ulong userID;
                try {
                    userID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                DatabaseCommandResult result = _database.Commands.GetGuildsForUser(userID,
                    out ObservableCollection<Guild>? guilds);

                if (result == DatabaseCommandResult.Success && guilds != null) {
                    Logger.Info($"Successfully fetched {guilds.Count} Guilds for User \"{userID}\".");
                    foreach (Guild guild in guilds) {
                        string json = JsonSerializer.Serialize(guild);
                        _client.Send(OperationCode.GetGuildsForUserSuccess, json);
                    }
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to fetch Guilds for User \"{userID}\", [{error}]");
                    _client.Send(OperationCode.GetGuildsForUserFail, error);
                }
                else {
                    const string error = "UNKNOWN_ERROR";
                    Logger.Error($"Failed to fetch Guilds for User \"{userID}\", [{error}]");
                    _client.Send(OperationCode.GetGuildsForUserFail, error);
                }
            }


            private void GetCategoriesInGuild(string message) {
                ulong guildID;
                try {
                    guildID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                DatabaseCommandResult result = _database.Commands.GetCategoriesInGuild(guildID,
                    out ObservableCollection<Category>? categories);

                if (result == DatabaseCommandResult.Success && categories != null) {
                    Logger.Info($"Successfully fetched {categories.Count} Categories in Guild \"{guildID}\".");
                    foreach (Category category in categories) {
                        string json = JsonSerializer.Serialize(category);
                        _client.Send(OperationCode.GetCategoriesInGuildSuccess, json);
                    }
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to fetch Categories in Guild \"{guildID}\", [{error}]");
                    _client.Send(OperationCode.GetCategoriesInGuildFail, error);
                }
                else {
                    const string error = "UNKNOWN_ERROR";
                    Logger.Error($"Failed to fetch Categories in Guild \"{guildID}\", [{error}]");
                    _client.Send(OperationCode.GetCategoriesInGuildFail, error);
                }
            }

            private void GetTextChannelsInCategory(string message) {
                ulong categoryID;
                try {
                    categoryID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                DatabaseCommandResult result = _database.Commands.GetTextChannelsInCategory(categoryID,
                    out ObservableCollection<TextChannel>? textChannels);

                if (result == DatabaseCommandResult.Success && textChannels != null) {
                    Logger.Info($"Successfully fetched {textChannels.Count} Text Channels in Category \"{categoryID}\".");
                    foreach (TextChannel textChannel in textChannels) {
                        string json = JsonSerializer.Serialize(textChannel);
                        _client.Send(OperationCode.GetTextChannelsInCategorySuccess, json);
                    }
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to fetch Text Channels in Category \"{categoryID}\", [{error}]");
                    _client.Send(OperationCode.GetTextChannelsInCategoryFail, error);
                }
                else {
                    const string error = "UNKNOWN_ERROR";
                    Logger.Error($"Failed to fetch Text Channels in Category \"{categoryID}\", [{error}]");
                    _client.Send(OperationCode.GetTextChannelsInCategoryFail, error);
                }
            }


            private void GetGuildPrivilegesForUser(string message) {
                ulong userID;
                try {
                    userID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                DatabaseCommandResult result = _database.Commands.GetGuildPrivilegeForUser(
                    userID, out GuildPrivilege? privilege);

                if (result == DatabaseCommandResult.Success && privilege != null) {
                    Logger.Info($"Successfully fetched Guild Privileges for User \"{userID}\".");
                    string json = JsonSerializer.Serialize(privilege);
                    _client.Send(OperationCode.GetGuildPrivilegeForUserSuccess, json);
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to fetch Guild Privileges for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetGuildPrivilegeForUserFail, error);
                }
                else {
                    const string error = "UNKNONW_ERROR";
                    Logger.Error($"Failed to fetch Guild Privileges for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetGuildPrivilegeForUserFail, error);
                }
            }

            private void GetCategoryPrivilegesForUser(string message) {
                ulong userID;
                try {
                    userID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                DatabaseCommandResult result = _database.Commands.GetCategoryPrivilegeForUser(
                    userID, out CategoryPrivilege? privilege);

                if (result == DatabaseCommandResult.Success && privilege != null) {
                    Logger.Info($"Successfully fetched Category Privileges for User \"{userID}\".");
                    string json = JsonSerializer.Serialize(privilege);
                    _client.Send(OperationCode.GetCategoryPrivilegeForUserSuccess, json);
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to fetch Category Privileges for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetCategoryPrivilegeForUserFail, error);
                }
                else {
                    const string error = "UNKNONW_ERROR";
                    Logger.Error($"Failed to fetch Category Privileges for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetCategoryPrivilegeForUserFail, error);
                }
            }

            private void GetTextChannelPrivilegesForUser(string message) {
                ulong userID;
                try {
                    userID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                DatabaseCommandResult result = _database.Commands.GetTextChannelPrivilegeForUser(
                    userID, out TextChannelPrivilege? privileges);

                if (result == DatabaseCommandResult.Success && privileges != null) {
                    Logger.Info($"Successfully fetched Text Channel Privileges for User \"{userID}\".");
                    string json = JsonSerializer.Serialize(privileges);
                    _client.Send(OperationCode.GetTextChannelPrivilegeForUserSuccess, json);
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to fetch Text Channel Privileges for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetTextChannelPrivilegeForUserFail, error);
                }
                else {
                    const string error = "UNKNONW_ERROR";
                    Logger.Error($"Failed to fetch Text Channel Privileges for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetTextChannelPrivilegeForUserFail, error);
                }
            }

            private void GetFriendList(string message) {
                ulong userID;
                try {
                    userID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                DatabaseCommandResult result = _database.Commands.GetFriends(userID, out List<User>? friends);

                if (result == DatabaseCommandResult.Success && friends != null) {
                    Logger.Info($"Successfully fetched FriendList for User \"{userID}\".");
                    string json = JsonSerializer.Serialize(friends);
                    _client.Send(OperationCode.GetFriendListSuccess, json);
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to fetch Friend List for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetFriendListFail, error);
                }
                else {
                    const string error = "UNKNONW_ERROR";
                    Logger.Error($"Failed to fetch Friend List for User \"{userID}\", [{error}].");
                    _client.Send(OperationCode.GetFriendListFail, error);
                }
            }

            private void AddFriend(string message) {
                ulong userID;
                try {
                    userID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                // TODO - finish this function
            }

            private void RemoveFriend(string message) {
                ulong userID;
                try {
                    userID = ulong.Parse(message);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    return;
                }

                // TODO - finish this function
            }

            private void SendMessage(string message) {
                MessageData? data = JsonSerializer.Deserialize<MessageData>(message);
                if (data == null) {
                    Logger.Error("Message data was null.");
                    _client.Send(OperationCode.SendMessageFail, "");
                    return;
                }
                _database.Commands.SendMessage(data, out Message? msg);
                if (msg == null) {
                    Logger.Error("Message was null.");
                    _client.Send(OperationCode.SendMessageFail, "");
                    return;
                }
                string json = JsonSerializer.Serialize(msg);
                _client.MultiCast(OperationCode.SendMessage, json, (x) => {
                    TextChannelPrivilege privilege = new(0, 0, msg.ChannelID) {
                        ViewChannel = ChatShared.PrivilegeValue.Neutral,
                        Read = ChatShared.PrivilegeValue.Neutral
                    };
                    var result = _database.Commands.GetUserIDsInTextChannel(msg.ChannelID,
                        out List<ulong>? userIDs, privilege);
                    if (result != DatabaseCommandResult.Success || userIDs == null) {
                        Logger.Error($"Failed to fetch liast of user with access to channel {msg.ChannelID}");
                        return false;
                    }
                    Logger.Warning(userIDs.Count.ToString());
                    foreach (ulong userID in userIDs) {
                        if (x.User != null && userID == x.User.ID) {
                            return true;
                        }
                    }
                    return false;
                });
            }

            private void JoinGuild(string message) {
                JoinGuildData? data = JsonSerializer.Deserialize<JoinGuildData>(message);
                if (data == null) {
                    Logger.Error("Join data is null.");
                    _client.Send(OperationCode.JoinGuildFail, "");
                    return;
                }
                var result = _database.Commands.JoinGuild(data, out Guild? guild);
                if (result == DatabaseCommandResult.Success) {
                    Logger.Info($"User '{data.UserID}' joined guild with public id '{data.PublicID}'.");
                    string json = JsonSerializer.Serialize(guild);
                    _client.Send(OperationCode.JoinGuildSuccess, json);
                }
                else {
                    Logger.Error($"Failed to add user '{data.UserID}' to guild '{data.PublicID}'");
                }
            }

            private void GetMessageRange(string message) {
                MessageRangeData? data = JsonSerializer.Deserialize<MessageRangeData>(message);
                if (data == null) {
                    Logger.Error("MessageRange data was null.");
                    _client.Send(OperationCode.GetMessageRangeFail, "MessageRange data was null.");
                    return;
                }
                var result = _database.Commands.GetMessageRange(data.ChannelID, data.First, data.Limit,
                    out ObservableCollection<Message>? messages);
                if (result == DatabaseCommandResult.Success && messages != null) {
                    string json = JsonSerializer.Serialize(messages);
                    _client.Send(OperationCode.GetMessageRangeSuccess, json);
                    Logger.Info($"Range of {messages.Count} ({data.First}, {data.Limit}) messages sent to user '{data.UserID}'");
                }
                else {
                    string errorMessage = $"Failed to get MessageRange ({data.First}, {data.Limit}) from user '{data.UserID}', {result}";
                    Logger.Error(errorMessage);
                    _client.Send(OperationCode.GetMessageRangeFail, errorMessage);
                }

            }
        }
    }
}
