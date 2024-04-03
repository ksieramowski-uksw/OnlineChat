using ChatClient;
using ChatShared.DataModels;
using System.Text.Json;
using ChatServer.Database;
using System.Collections.ObjectModel;
using ChatShared.Models;


namespace ChatServer.Network
{
    public partial class ClientConnection {
        private class ClientOperationHandler {
            private readonly ClientConnection _client;
            //private readonly Server _server;
            private readonly DatabaseConnection _database;

            public ClientOperationHandler(ClientConnection client) {
                _client = client;
                //_server = client._server;
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
                    case OperationCode.CreateCategory: {
                        CreateCategory(message);
                    } break;
                    case OperationCode.CreateTextChannel: {
                        CreateTextChannel(message);
                    } break;
                    case OperationCode.CompleteGuildInfo: {
                        CompleteGuildsInfo(message);
                    } break;
                    case OperationCode.SendMessage: {
                        SendMessage(message);
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
                        Logger.Info($"Succesfully logged user \"{user.Nickname}#{user.Id}\" with email \"{data.Email}\".");
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

                DatabaseCommandResult result = _database.Commands.TryRegisterUser(data);

                if (result == DatabaseCommandResult.Success) {
                    Logger.Info($"Succesfully registered user with email \"{data.Email}\".");
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

            private void GetFriends(string message) {
                ulong userId = ulong.Parse(message);

                DatabaseCommandResult result = _database.Commands.GetFriends(userId, out List<User>? friends);

                if (result == DatabaseCommandResult.Success && friends != null) {
                    Logger.Info($"Successfully fetched FriendList for user with Id \"{userId}\".");
                    string json = JsonSerializer.Serialize(friends);
                    _client.Send(OperationCode.GetFriendListSuccess, json);
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    const string error = "DATABASE_ERROR";
                    Logger.Error($"Failed to get FriendList for user with id \"{userId}\", [{error}].");
                    _client.Send(OperationCode.GetFriendListFail, error);
                }
                else {
                    const string error = "UNKNONW_ERROR";
                    Logger.Error($"Failed to get FriendList for user with id \"{userId}\", [{error}].");
                    _client.Send(OperationCode.GetFriendListFail, error);
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
                    result = _database.Commands.GetUsersAccessingCategory(category, out targetUsers);

                    if (result == DatabaseCommandResult.Success && targetUsers is not null) {
                        string json = JsonSerializer.Serialize(category);
                        _client.MultiCast(OperationCode.CreateCategorySuccess, json, (x) => {
                            foreach (ulong id in targetUsers) {
                                if (x.User is User user && user.Id == id) {
                                    return true;
                                }
                            }
                            return false;
                        });
                    }
                    else {
                        Logger.Error($"Failed to multicast category creation \"{data.Name}\" in guild \"{data.GuildId}\"");
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

                DatabaseCommandResult result = _database.Commands.CreateTextChannel(data);

                if (result == DatabaseCommandResult.Success) {
                    Logger.Info($"Successfully created new text channel \"{data.Name}\".");
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    Logger.Error($"Failed to create text channel \"{data.Name}\", [DATABASE_ERROR].");
                }
            }



            private void CompleteGuildsInfo(string message) {
                ulong userId = ulong.Parse(message);

                DatabaseCommandResult result = _database.Commands.GetCompleteGuildsInfo(userId,
                    out ObservableCollection<Guild>? guilds);

                if (result == DatabaseCommandResult.Success && guilds != null) {
                    foreach (Guild guild in guilds) {
                        string json = JsonSerializer.Serialize(guild);
                        _client.Send(OperationCode.CompleteGuildInfoSuccess, json);
                        //Thread.Sleep(100);
                    }
                }
                else if (result == DatabaseCommandResult.DatabaseError) {
                    Logger.Error($"Failed to get complete guild info for user with id \"{userId}\"");
                    _client.Send(OperationCode.CompleteGuildInfoFail, result.ToString());
                }
            }


            private void SendMessage(string message) {

            }

        }
    }
}
