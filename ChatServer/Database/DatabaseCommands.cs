using ChatServer.Database.Enums;
using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;


namespace ChatServer.Database
{
    public partial class DatabaseCommands {
        public SqliteConnection Connection { get; }

        public DatabaseCommands(SqliteConnection connection) {
            Connection = connection;
        }



        public RegistrationResult RegisterUser(string email, string password, string nickname, string pronoun, byte[]? profilePicture) {
            DateTime creationTime = DateTime.Now;
            string? publicID = GetUniquePublicID("users");
            if (publicID == null) {
                Logger.Error($"Failed to generate unique public ID for user with email '{email}'.", MethodBase.GetCurrentMethod());
                return RegistrationResult.Fail;
            }

            try {
                if (GetUserByEmail(email) != null) {
                    return RegistrationResult.UserAlreadyExists;
                }

                if (profilePicture == null) {
                    profilePicture = ResourceHelper.GetDefaultProfilePicture();
                }

                ID userID = RegisterUser(publicID, email, password, nickname, pronoun, profilePicture, creationTime);
                if (userID == 0) {
                    Logger.Error($"Failed to register user with email '{email}'.", MethodBase.GetCurrentMethod());
                    return RegistrationResult.Fail;
                }
                return RegistrationResult.Success;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return RegistrationResult.Fail;
            }
        }



        public User? LogIn(string email, string password) {
            User? user = GetUserByLoginDetails(email, password);
            if (user != null) {
                SetUserStatus(user.ID, UserStatus.Online);
                user.Status = UserStatus.Online;
            }
            return user;
        }



        public Guild? CreateGuild(ID ownerID, string name, string password, byte[] icon, GuildPrivilege defaultPrivilege) {
            DateTime creationTime = DateTime.Now;
            string? publicID = GetUniquePublicID("Guilds");
            if (publicID == null) { return null; }

            try {
                // create guild 
                ID guildID = CreateGuild(ownerID, publicID, name, password, icon, creationTime);
                if (guildID == 0) {
                    Logger.Error($"Failed to create guild for user '{ownerID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                defaultPrivilege.GuildID = guildID;

                // create guild affiliation for owner
                ID affiliationID = CreateGuildAffiliation(ownerID, guildID);
                if (affiliationID == 0) {
                    Logger.Error($"Failed to create affiliation for guild '{guildID}' and it's owner '{ownerID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                // create default privilege
                ID defaultPrivilegeID = CreateDefaultGuildPrivilege(defaultPrivilege);
                if (defaultPrivilegeID == 0) {
                    Logger.Error($"Failed to create default privilege for guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                // create privilege for owner
                ID ownerPrivilegeID = CreateGuildPrivilegeForOwner(ownerID, guildID);
                if (ownerPrivilegeID == 0) {
                    Logger.Error($"Failed to create privilege for owner '{ownerID}' of guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                // create first category
                Category? category = CreateCategory(guildID, "GENERAL", null);
                if (category == null) {
                    Logger.Error($"Failed to create first category in new guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                // create first text channel
                TextChannel? textChannel = CreateTextChannel(category.ID, "general", null);
                if (textChannel == null) {
                    Logger.Error($"Failed to create first text channel in new guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                // return new guild
                Guild guild = new(guildID, publicID, name, ownerID, icon, creationTime);
                //guild.DefaultPrivilege = defaultPrivilege;
                //guild.Categories = [category];
                //category.TextChannels = [textChannel];
                return guild;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public Category? CreateCategory(ID guildID, string categoryName, CategoryPrivilege? defaultPrivilege) {
            try {
                DateTime creationTime = DateTime.Now;

                // get guild owner
                ID ownerID = GetGuildOwnerID(guildID);
                if (ownerID == 0) {
                    Logger.Error($"Failed to get owner of guild '{guildID}'.");
                    return null;
                }

                // create category
                ID categoryID = CreateCategory(guildID, categoryName, creationTime);
                if (categoryID == 0) {
                    Logger.Error($"Failed to create category in guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                Category category = new(categoryID, guildID, categoryName, creationTime);

                // create default privilege
                if (defaultPrivilege == null) {
                    defaultPrivilege = new CategoryPrivilege(0, 0, categoryID);
                }
                defaultPrivilege.CategoryID = category.ID;

                ID defaultPrivilegeID = CreateDefaultCategoryPrivilege(defaultPrivilege);
                if (defaultPrivilegeID == 0) {
                    Logger.Error($"Failed to create default privilege for new category '{categoryID}' in guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                category.DefaultPrivilege = new CategoryPrivilege(defaultPrivilege) {
                    ID = defaultPrivilegeID,
                    CategoryID = categoryID,
                };

                // assign default privilege to existing users
                var users = GetUsersInGuild(guildID);
                if (users == null) {
                    Logger.Error($"Failed to get list of users in guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                
                category.Privileges = new();
                foreach (var userID in users) {
                    // privilege for guild owner
                    if (userID == ownerID) {
                        CategoryPrivilege privilege = CategoryPrivilege.OwnerPrivilege(ownerID, categoryID);
                        if (CreateCategoryPrivilege(privilege) == 0) {
                            Logger.Error($"Failed to create category privilege for guild owner in new category '{categoryID}' in guild '{guildID}'.", MethodBase.GetCurrentMethod());
                            return null;
                        }
                        category.Privileges.Add(privilege);
                    }
                    // privilege for regular user
                    else {
                        CategoryPrivilege privilege = new(category.DefaultPrivilege) { UserID = userID };
                        if (CreateCategoryPrivilege(privilege) == 0) {
                            Logger.Error($"Failed to assign category privilege privilege in new category '{categoryID}'.", MethodBase.GetCurrentMethod());
                            return null;
                        }
                        category.Privileges.Add(privilege);
                    }
                }

                return category;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }


        public TextChannel? CreateTextChannel(ID categoryID, string textChannelName, TextChannelPrivilege? defaultPrivilege) {
            try {
                DateTime creationTime = DateTime.Now;

                // get guild owner (category owner)
                ID ownerID = GetCategoryOwnerID(categoryID);
                if (ownerID == 0) {
                    Logger.Error($"Failed to get owner of category '{categoryID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                // create text channel
                ID textChannelID = CreateTextChannel(categoryID, textChannelName, creationTime);
                if (textChannelID == 0) {
                    Logger.Error($"Failed to create text channel in category '{categoryID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                TextChannel textChannel = new(textChannelID, categoryID, textChannelName, creationTime);

                // create defult privilege
                if (defaultPrivilege == null) {
                    defaultPrivilege = new TextChannelPrivilege();
                }
                defaultPrivilege.ChannelID = textChannel.ID;

                ID defaultPrivilegeID = CreateDefaultTextChannelPrivilege(defaultPrivilege);
                if (defaultPrivilegeID == 0) {
                    Logger.Error($"Failed to create default privilege for new text channel '{textChannelID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                textChannel.DefaultPrivilege = new TextChannelPrivilege(defaultPrivilege) {
                    ID = defaultPrivilegeID,
                    ChannelID = textChannelID,
                };

                // assign default privilege to existing users
                var users = GetUsersInCategory(categoryID);
                if (users == null) {
                    Logger.Error($"Failed to get users in category '{categoryID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                textChannel.Privileges = new();
                foreach (var userID in users) {
                    // privilege for guild owner
                    if (userID == ownerID) {
                        TextChannelPrivilege privilege = TextChannelPrivilege.OwnerPrivilege(ownerID, textChannelID);
                        if (CreateTextChannelPrivilege(privilege) == 0) {
                            Logger.Error($"Failed to create privilege for guild owner '{ownerID}' in new text channel '{textChannelID}'", MethodBase.GetCurrentMethod());
                            return null;
                        }
                        textChannel.Privileges.Add(privilege);
                    }
                    // privilege for regular users
                    else {
                        TextChannelPrivilege privilege = new(textChannel.DefaultPrivilege) { UserID = userID };
                        if (CreateTextChannelPrivilege(privilege) == 0) {
                            Logger.Error($"Failed to create privilege for user '{userID}' in new text channel '{textChannelID}'", MethodBase.GetCurrentMethod());
                            return null;
                        }
                        textChannel.Privileges.Add(privilege);
                    }
                }

                return textChannel;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public UpdateGuildData? UpdateGuild(ID guildID, string name, string password, byte[]? icon, GuildPrivilege? privilege) {
            try {
                // update name
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                            UPDATE Guilds
                                SET
                                    Name = @Name
                                WHERE Guilds.ID = @GuildID
                            ;";
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@GuildID", guildID);
                }
                // update password
                if (!string.IsNullOrWhiteSpace(password)) {
                    using (var command = Connection.CreateCommand()) {
                        command.CommandText = $@"
                            UPDATE Guilds
                                SET
                                    Password = @Password
                                WHERE Guilds.ID = @GuildID
                            ;";
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@GuildID", guildID);
                        command.ExecuteNonQuery();
                    }
                }
                // update icon
                if (icon != null) {
                    using (var command = Connection.CreateCommand()) {
                        command.CommandText = $@"
                            UPDATE Guilds
                                SET
                                    Icon = @Icon
                                WHERE Guilds.ID = @GuildID
                            ;";
                        command.Parameters.AddWithValue("@Icon", icon);
                        command.Parameters.AddWithValue("@GuildID", guildID);
                        command.ExecuteNonQuery();
                    }
                }
                // update default privilege
                if (privilege != null) {
                    // default
                    if (privilege.UserID == 0) {
                        using (var command = Connection.CreateCommand()) {
                            command.CommandText = $@"
                                UPDATE DefaultGuildPrivileges
                                    SET
                                        ManageGuild = @ManageGuild,
                                        ManagePrivileges = @ManagePrivileges,
                                        CreateCategory = @CreateCategory,
                                        UpdateCategory = @UpdateCategory,
                                        DeleteCategory = @DeleteCategory,
                                        CreateChannel = @CreateChannel,
                                        UpdateChannel = @UpdateChannel,
                                        DeleteChannel = @DeleteChannel,
                                        Read = @Read,
                                        Write = @Write
                                    WHERE DefaultGuildPrivileges.GuildID = @GuildID
                                ;";
                            command.Parameters.AddWithValue("@ManageGuild", (sbyte)privilege.ManageGuild);
                            command.Parameters.AddWithValue("@ManagePrivileges", (sbyte)privilege.ManagePrivileges);
                            command.Parameters.AddWithValue("@CreateCategory", (sbyte)privilege.CreateCategory);
                            command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                            command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteCategory);
                            command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                            command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                            command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                            command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                            command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);
                            command.Parameters.AddWithValue("@GuildID", guildID);
                            command.ExecuteNonQuery();
                        }
                    }
                    // user
                    else {
                        using (var command = Connection.CreateCommand()) {
                            command.CommandText = $@"
                                UPDATE GuildPrivileges
                                    SET
                                        ManageGuild = @ManageGuild,
                                        ManagePrivileges = @ManagePrivileges,
                                        CreateCategory = @CreateCategory,
                                        UpdateCategory = @UpdateCategory,
                                        DeleteCategory = @DeleteCategory,
                                        CreateChannel = @CreateChannel,
                                        UpdateChannel = @UpdateChannel,
                                        DeleteChannel = @DeleteChannel,
                                        Read = @Read,
                                        Write = @Write
                                    WHERE GuildPrivileges.GuildID = @GuildID
                                        AND GuildPrivileges.UserID = @UserID
                                ;";
                            command.Parameters.AddWithValue("@ManageGuild", (sbyte)privilege.ManageGuild);
                            command.Parameters.AddWithValue("@ManagePrivileges", (sbyte)privilege.ManagePrivileges);
                            command.Parameters.AddWithValue("@CreateCategory", (sbyte)privilege.CreateCategory);
                            command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                            command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteCategory);
                            command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                            command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                            command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                            command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                            command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);
                            command.Parameters.AddWithValue("@GuildID", guildID);
                            command.Parameters.AddWithValue("@UserID", privilege.UserID);
                            command.ExecuteNonQuery();
                        }
                    }

                }
                return new UpdateGuildData(guildID, name, password, icon, privilege);
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public UpdateCategoryData? UpdateCategory(ID categoryID, string categoryName, CategoryPrivilege? privilege) {
            try {
                // update name
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        UPDATE Categories
                            SET Name = @Name
                            WHERE Categories.ID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@Name", categoryName);
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                // update default privilege
                if (privilege != null) {
                    // default
                    if (privilege.UserID == 0) {
                        using (var command = Connection.CreateCommand()) {
                            command.CommandText = $@"
                                UPDATE DefaultCategoryPrivileges
                                    SET 
                                        ViewCategory = @ViewCategory,
                                        UpdateCategory = @UpdateCategory,
                                        DeleteCategory = @DeleteCategory,
                                        CreateChannel = @CreateChannel,
                                        UpdateChannel = @UpdateChannel,
                                        DeleteChannel = @DeleteChannel,
                                        Read = @Read,
                                        Write = @Write
                                    WHERE DefaultCategoryPrivileges.CategoryID = @CategoryID
                                ;";
                            command.Parameters.AddWithValue("@ViewCategory", (sbyte)privilege.ViewCategory);
                            command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                            command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteCategory);
                            command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                            command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                            command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                            command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                            command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);
                            command.Parameters.AddWithValue("@CategoryID", categoryID);
                            command.ExecuteNonQuery();
                        }
                    }
                    // user
                    else {
                        using (var command = Connection.CreateCommand()) {
                            command.CommandText = $@"
                                UPDATE CategoryPrivileges
                                    SET 
                                        ViewCategory = @ViewCategory,
                                        UpdateCategory = @UpdateCategory,
                                        DeleteCategory = @DeleteCategory,
                                        CreateChannel = @CreateChannel,
                                        UpdateChannel = @UpdateChannel,
                                        DeleteChannel = @DeleteChannel,
                                        Read = @Read,
                                        Write = @Write
                                    WHERE CategoryPrivileges.CategoryID = @CategoryID
                                        AND CategoryPrivileges.UserID = @UserID
                                ;";
                            command.Parameters.AddWithValue("@ViewCategory", (sbyte)privilege.ViewCategory);
                            command.Parameters.AddWithValue("@UpdateCategory", (sbyte)privilege.UpdateCategory);
                            command.Parameters.AddWithValue("@DeleteCategory", (sbyte)privilege.DeleteCategory);
                            command.Parameters.AddWithValue("@CreateChannel", (sbyte)privilege.CreateChannel);
                            command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                            command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                            command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                            command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);
                            command.Parameters.AddWithValue("@CategoryID", categoryID);
                            command.Parameters.AddWithValue("@UserID", privilege.UserID);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                return new UpdateCategoryData(categoryID, categoryName, privilege);
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public UpdateTextChannelData? UpdateTextChannel(ID textChannelID, string textChannelName, TextChannelPrivilege? privilege) {
            try {
                // update name
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        UPDATE TextChannels
                            SET Name = @Name
                            WHERE TextChannels.ID = @TextChannelID
                        ;";
                    command.Parameters.AddWithValue("@Name", textChannelName);
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);
                    command.ExecuteNonQuery();
                }
                if (privilege == null) {
                    Logger.Warning($"Skipped update on text channel privileges in text channel '{textChannelID}', becuase privilege is NULL.");
                    return new UpdateTextChannelData(textChannelID, textChannelName, privilege);
                }
                // update default privilege
                if (privilege != null) {
                    // default
                    if (privilege.UserID == 0) {
                        using (var command = Connection.CreateCommand()) {
                            command.CommandText = $@"
                                UPDATE DefaultTextChannelPrivileges
                                    SET
                                        ViewChannel = @ViewChannel,
                                        UpdateChannel = @UpdateChannel,
                                        DeleteChannel = @DeleteChannel,
                                        Read = @Read,
                                        Write = @Write
                                    WHERE DefaultTextChannelPrivileges.ChannelID = @TextChannelID
                                ;";
                            command.Parameters.AddWithValue("@ViewChannel", (sbyte)privilege.ViewChannel);
                            command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                            command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                            command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                            command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);
                            command.Parameters.AddWithValue("@TextChannelID", textChannelID);
                            command.ExecuteNonQuery();
                        }
                    }
                    // user
                    else {
                        using (var command = Connection.CreateCommand()) {
                            command.CommandText = $@"
                                UPDATE TextChannelPrivileges
                                    SET
                                        ViewChannel = @ViewChannel,
                                        UpdateChannel = @UpdateChannel,
                                        DeleteChannel = @DeleteChannel,
                                        Read = @Read,
                                        Write = @Write
                                    WHERE TextChannelPrivileges.ChannelID = @TextChannelID
                                        AND TextChannelPrivileges.UserID = @UserID
                                ;";
                            command.Parameters.AddWithValue("@ViewChannel", (sbyte)privilege.ViewChannel);
                            command.Parameters.AddWithValue("@UpdateChannel", (sbyte)privilege.UpdateChannel);
                            command.Parameters.AddWithValue("@DeleteChannel", (sbyte)privilege.DeleteChannel);
                            command.Parameters.AddWithValue("@Read", (sbyte)privilege.Read);
                            command.Parameters.AddWithValue("@Write", (sbyte)privilege.Write);
                            command.Parameters.AddWithValue("@TextChannelID", textChannelID);
                            command.Parameters.AddWithValue("@UserID", privilege.UserID);
                            command.ExecuteNonQuery();
                        }
                    }

                }
                return new UpdateTextChannelData(textChannelID, textChannelName, privilege);
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public ObservableCollection<Guild>? GetGuildsForUser(ID userID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        SELECT
                            Guilds.ID,
                            Guilds.PublicID,
                            Guilds.Name,
                            Guilds.OwnerID,
                            Guilds.CreationTime,
                            Guilds.Icon
                        FROM Guilds INNER JOIN GuildAffiliations ON Guilds.ID = GuildAffiliations.GuildID
                        WHERE GuildAffiliations.UserID = @UserID
                        ;";
                    command.Parameters.AddWithValue("@UserID", userID);

                    ObservableCollection<Guild>? guilds = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID guildID = reader.GetFieldValue<ID>(field++);
                            string guildPublicID = reader.GetFieldValue<string>(field++);
                            string guildName = reader.GetFieldValue<string>(field++);
                            ID guildOwnerID = reader.GetFieldValue<ID>(field++);
                            DateTime guildCreationTime = reader.GetFieldValue<DateTime>(field++);
                            byte[] guildIcon = reader.GetFieldValue<byte[]>(field++);

                            Guild guild = new(guildID, guildPublicID, guildName, guildOwnerID, guildIcon, guildCreationTime);
                            guilds.Add(guild);
                        }
                        return guilds;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public GuildDetails? GetGuildDetails(ID guildID) {
            try {
                GuildDetails guildDetails = new();
                guildDetails.Users = GetUsersInGuild<User>(guildID);
                if (guildDetails.Users == null) {
                    Logger.Error("Failed to get users in guild.", MethodBase.GetCurrentMethod());
                    return null;
                }

                guildDetails.Categories = GetCategoriesInGuild(guildID);
                if (guildDetails.Categories == null) {
                    Logger.Error($"Failed to get categories in guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }
                guildDetails.DefaultPrivilege = GetDefaultGuildPrivilege(guildID);
                if (guildDetails.DefaultPrivilege == null) {
                    Logger.Error($"Failed to get default privilege in guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                guildDetails.Privileges = GetAllGuildPrivileges(guildID);
                if (guildDetails.Privileges == null) {
                    Logger.Error($"Failed to get privileges in guild '{guildID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                guildDetails.GuildID = guildID;

                return guildDetails;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public List<Message>? GetMessageRange(ID textChannelID, ID first, uint limit) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT Messages.ID, Messages.UserID, Messages.Content, Messages.CreationTime
                            FROM Messages
                                INNER JOIN TextChannels ON Messages.TextChannelID = TextChannels.ID
                            WHERE Messages.ID < @First
                                AND TextChannels.ID = @TextChannelID
                            ORDER BY Messages.ID DESC
                            LIMIT @Limit
                        ;";
                    command.Parameters.AddWithValue("@First", first);
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);
                    command.Parameters.AddWithValue("@Limit", limit);

                    List<Message> messages = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID id = reader.GetFieldValue<ID>(field++);
                            ID authorID = reader.GetFieldValue<ID>(field++);
                            string content = reader.GetFieldValue<string>(field++);
                            DateTime time = reader.GetFieldValue<DateTime>(field++);

                           messages.Add(new Message(id, textChannelID, authorID, content, time));
                        }
                    }
                    return messages;
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public ObservableCollection<GuildPrivilege>? GetAllGuildPrivileges(ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = $@"
                        SELECT
                            GuildPrivileges.ID,
                            GuildPrivileges.UserID,
                            GuildPrivileges.ManageGuild,
                            GuildPrivileges.ManagePrivileges,
                            GuildPrivileges.CreateCategory,
                            GuildPrivileges.UpdateCategory,
                            GuildPrivileges.DeleteCategory,
                            GuildPrivileges.CreateChannel,
                            GuildPrivileges.UpdateChannel,
                            GuildPrivileges.DeleteChannel,
                            GuildPrivileges.Read,
                            GuildPrivileges.Write
                        FROM GuildPrivileges
                        WHERE GuildPrivileges.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);

                    ObservableCollection<GuildPrivilege> privileges = new();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            byte field = 0;
                            ID id = reader.GetFieldValue<ID>(field++);
                            ID userID = reader.GetFieldValue<ID>(field++);
                            PrivilegeValue manageGuild = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue managePrivileges = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue createCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue updateCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue deleteCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue createChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                            PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                            privileges.Add(new GuildPrivilege(id, userID, guildID) {
                                ManageGuild = manageGuild,
                                ManagePrivileges = managePrivileges,
                                CreateCategory = createCategory,
                                UpdateCategory = updateCategory,
                                DeleteCategory = deleteCategory,
                                CreateChannel = createChannel,
                                UpdateChannel = updateChannel,
                                DeleteChannel = deleteChannel,
                                Read = read,
                                Write = write,
                            });
                        }
                    }
                    return privileges;
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public Message? SendMessage(ID userID, ID textChannelID, string msgContent) {
            try {
                DateTime creationTime = DateTime.Now;

                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        INSERT INTO messages (UserID, TextChannelID, Content, CreationTime)
                            VALUES (@UserID, @TextChannelID, @Content, @CreationTime)
                            RETURNING ID
                        ;";
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@TextChannelID", textChannelID);
                    command.Parameters.AddWithValue("@Content", msgContent);
                    command.Parameters.AddWithValue("@CreationTime", creationTime);
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            ID id = reader.GetFieldValue<ID>(0);
                            return new Message(id, textChannelID, userID, msgContent, creationTime);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public JoiningUser? JoinGuild(ID userID, string guildPublicID, string guildPassword) {
            try {
                Guild? guild = GetGuildByJoinDetails(guildPublicID, guildPassword);
                if (guild == null) {
                    Logger.Warning($"Failed to get guild while adding user '{userID}' to guild with publicID '{guildPublicID}'.");
                    return new JoiningUser(null, null, "Invalid join details.");
                }

                ID affiliationID = GetGuildAffiliationID(userID, guild.ID);
                if (affiliationID != 0) {
                    Logger.Warning($"User '{userID}' is already member of guild '{guild.ID}'.");
                    return new JoiningUser(null, null, "User is already member of this server.");
                }

                User? user = GetUser(userID);
                if (user == null) {
                    Logger.Error($"Failed to get user '{userID}' while adding him to guild '{guild.ID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                ObservableCollection<Category>? categories = GetCategoriesInGuild(guild.ID);
                if (categories == null) {
                    Logger.Error($"Failed to get categories in guild '{guild.ID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                foreach (var category in categories) {
                    ObservableCollection<TextChannel>? textChannels = GetTextChannelsInCategory(category.ID);
                    if (textChannels == null) { return null; }
                    foreach (var textChannel in textChannels) {
                        if (CreateDefaultTextChannelPrivilegeForUser(userID, textChannel.ID) == 0) {
                            Logger.Error("Failed to create text channel privilege for joining user.", MethodBase.GetCurrentMethod());
                            return null;
                        }
                    }
                    if (CreateDefaultCategoryPrivilegeForUser(userID, category.ID) == 0) {
                        Logger.Error("Failed to create category privilege for joining user.", MethodBase.GetCurrentMethod());
                        return null;
                    }
                }

                affiliationID = CreateGuildAffiliation(user.ID, guild.ID);
                if (affiliationID == 0) {
                    Logger.Error($"Failed to create guild affiliation for user '{user.ID}' in guild '{guild.ID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                GuildPrivilege? privilege = GetDefaultGuildPrivilege(guild.ID);
                if (privilege == null) {
                    Logger.Error($"Failed to get default privilege in guild '{guild.ID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }

                privilege.UserID = userID;

                ID privilegeID = CreateGuildPrivilege(privilege);
                if (privilegeID == 0) {
                    Logger.Error($"Failed to create guild privilege for user '{user.ID}' in guild '{guild.ID}'.", MethodBase.GetCurrentMethod());
                    return null;
                }


                return new JoiningUser(user, guild, string.Empty);
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return null;
            }
        }



        public bool DeleteGuild(ID guildID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM Messages
                            WHERE Messages.ID IN (
                                SELECT Messages.ID
                                    FROM Messages
                                        INNER JOIN TextChannels ON TextChannels.ID = Messages.TextChannelID
                                        INNER JOIN Categories ON Categories.ID = TextChannels.CategoryID
                                    WHERE Categories.GuildID = @GuildID
                            )
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM DefaultTextChannelPrivileges
                            WHERE DefaultTextChannelPrivileges.ID IN (
                                SELECT DefaultTextChannelPrivileges.ID
                                    FROM DefaultTextChannelPrivileges
                                        INNER JOIN TextChannels ON TextChannels.ID = DefaultTextChannelPrivileges.ChannelID
                                        INNER JOIN Categories ON Categories.ID = TextChannels.CategoryID
                                    WHERE Categories.GuildID = @GuildID
                            )
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM TextChannelPrivileges
                            WHERE TextChannelPrivileges.ID IN (
                                SELECT TextChannelPrivileges.ID
                                    FROM TextChannelPrivileges
                                        INNER JOIN TextChannels ON TextChannels.ID = TextChannelPrivileges.ChannelID
                                        INNER JOIN Categories ON Categories.ID = TextChannels.CategoryID
                                    WHERE Categories.GuildID = @GuildID
                            )
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM TextChannels
                            WHERE TextChannels.ID IN (
                                SELECT TextChannels.ID
                                    FROM TextChannels
                                        INNER JOIN Categories ON Categories.ID = TextChannels.CategoryID
                                    WHERE Categories.GuildID = @GuildID
                            )
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM DefaultCategoryPrivileges
                            WHERE DefaultCategoryPrivileges.ID IN (
                                SELECT DefaultCategoryPrivileges.ID
                                    FROM DefaultCategoryPrivileges
                                        INNER JOIN Categories ON Categories.ID = DefaultCategoryPrivileges.CategoryID
                                    WHERE Categories.GuildID = @GuildID
                            )
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM CategoryPrivileges
                            WHERE CategoryPrivileges.ID IN (
                                SELECT CategoryPrivileges.ID
                                    FROM CategoryPrivileges
                                        INNER JOIN Categories ON Categories.ID = CategoryPrivileges.CategoryID
                                    WHERE Categories.GuildID = @GuildID
                            )
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM Categories
                            WHERE Categories.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM DefaultGuildPrivileges
                            WHERE DefaultGuildPrivileges.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM GuildPrivileges
                            WHERE GuildPrivileges.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM Guilds
                            WHERE Guilds.ID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM GuildAffiliations
                            WHERE GuildAffiliations.GuildID = @GuildID
                        ;";
                    command.Parameters.AddWithValue("@GuildID", guildID);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return false;
            }
        }


        public bool DeleteCategory(ID categoryID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM Messages
                            WHERE Messages.ID IN (
                                SELECT Messages.ID
                                    FROM Messages
                                        INNER JOIN TextChannels ON TextChannels.ID = Messages.TextChannelID
                                    WHERE TextChannels.CategoryID = @CategoryID
                            )
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM DefaultTextChannelPrivileges
                            WHERE DefaultTextChannelPrivileges.ID IN (
                                SELECT DefaultTextChannelPrivileges.ID
                                    FROM DefaultTextChannelPrivileges
                                        INNER JOIN TextChannels ON TextChannels.ID = DefaultTextChannelPrivileges.ChannelID
                                    WHERE TextChannels.CategoryID = @CategoryID
                            )
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM TextChannelPrivileges
                            WHERE TextChannelPrivileges.ID IN (
                                SELECT TextChannelPrivileges.ID
                                    FROM TextChannelPrivileges
                                        INNER JOIN TextChannels ON TextChannels.ID = TextChannelPrivileges.ChannelID
                                    WHERE TextChannels.CategoryID = @CategoryID
                            )
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM TextChannels
                            WHERE TextChannels.CategoryID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM DefaultCategoryPrivileges
                            WHERE DefaultCategoryPrivileges.CategoryID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM CategoryPrivileges
                            WHERE CategoryPrivileges.CategoryID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM Categories
                            WHERE Categories.ID = @CategoryID
                        ;";
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return false;
            }
        }

        public bool DeleteTextChannel(ID textChannelID) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM Messages
                            WHERE Messages.TextChannelID = @ChannelID
                        ;";
                    command.Parameters.AddWithValue("@ChannelID", textChannelID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM DefaultTextChannelPrivileges
                            WHERE DefaultTextChannelPrivileges.ChannelID = @ChannelID
                        ;";
                    command.Parameters.AddWithValue("@ChannelID", textChannelID);
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM TextChannelPrivileges
                            WHERE TextChannelPrivileges.ChannelID = @ChannelID
                        ;";
                    command.Parameters.AddWithValue("@ChannelID", textChannelID);
                    command.ExecuteNonQuery();
                }
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        DELETE FROM TextChannels
                            WHERE TextChannels.ID = @ChannelID
                        ;";
                    command.Parameters.AddWithValue("@ChannelID", textChannelID);
                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return false;
            }
        }

        public bool UpdateUser(ID userID, string nickname, string pronoun, byte[]? profilePicture) {
            try {
                using (var command = Connection.CreateCommand()) {
                    if (profilePicture != null) {
                        command.CommandText = @"
                            UPDATE Users
                                SET
                                    Nickname = @Nickname,
                                    Pronoun = @Pronoun,
                                    ProfilePicture = @ProfilePicture
                                WHERE Users.ID = @UserID
                            ;";
                        command.Parameters.AddWithValue("@Nickname", nickname);
                        command.Parameters.AddWithValue("@Pronoun", pronoun);
                        command.Parameters.AddWithValue("@ProfilePicture", profilePicture);
                        command.Parameters.AddWithValue("@UserID", userID);

                    }
                    else {
                        command.CommandText = @"
                            UPDATE Users
                                SET
                                    Nickname = @Nickname,
                                    Pronoun = @Pronoun
                                WHERE Users.ID = @UserID;
                            ";
                        command.Parameters.AddWithValue("@Nickname", nickname);
                        command.Parameters.AddWithValue("@Pronoun", pronoun);
                        command.Parameters.AddWithValue("@UserID", userID);
                    }
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex) {
                Logger.Error (ex, MethodBase.GetCurrentMethod());
                return false;
            }
        }

        public void ResetUserStatuses() {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        UPDATE Users
                            SET Status = @Status
                        ;";
                    command.Parameters.AddWithValue("@Status", UserStatus.Offline);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return;
            }
        }

        public bool SetUserStatus(ID userID, UserStatus status) {
            try {
                using (var command = Connection.CreateCommand()) {
                    command.CommandText = @"
                        UPDATE Users
                            SET Status = @Status
                            WHERE Users.ID = @UserID
                        ;";
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex) {
                Logger.Error(ex, MethodBase.GetCurrentMethod());
                return false;
            }
        }
    }
}
