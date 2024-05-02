using ChatServer.Database.Enums;
using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ChatServer.Database
{
    public partial class DatabaseCommands {
        public SqliteConnection Connection { get; }

        public DatabaseCommands(SqliteConnection connection) {
            Connection = connection;
        }

        private bool ExecuteNonQuery(string query) {
            using var command = Connection.CreateCommand();
            command.CommandText = query;
            try {
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                return false;
            }
            return true;
        }

        private SqliteDataReader? ExecuteReader(string query) {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            try {
                var reader = command.ExecuteReader();
                return reader;
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
            }
            return null;
        }

        private bool EntityExists(string query) {
            using var command = Connection.CreateCommand();
            command.CommandText = query;
            try {
                var reader = command.ExecuteReader();
                return reader.HasRows;
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
            }
            return false;
        }



        public DatabaseCommandResult RegisterUser(RegisterData data) {
            bool userExists = EntityExists(@$"
                SELECT ID
                FROM Users
                WHERE Users.Email LIKE '{data.Email}' OR Users.Nickname LIKE '{data.Nickname}'
                ;");
            if (userExists) {
                return DatabaseCommandResult.UserExists;
            }
            if (InsertUser(data) == 0) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult TryLogIn(LoginData data, out User? userData) {
            var reader = ExecuteReader(@$"
                SELECT
                    Users.ID,
                    Users.PublicID,
                    Users.Nickname,
                    Users.Pronoun,
                    Users.CreationTime,
                    Users.ProfilePicture,
                    Users.Status
                FROM Users
                WHERE Users.Email LIKE '{data.Email}' AND Users.Password LIKE '{data.Password}'
                ;");
            if (reader is null) {
                userData = null;
                return DatabaseCommandResult.DatabaseError;
            }
            if (reader.Read()) {
                userData = GetUser(reader);
                return DatabaseCommandResult.Success;
            }
            userData = null;
            return DatabaseCommandResult.Fail;
        }

        public DatabaseCommandResult GetFriends(ulong userID, out List<User>? friends) {
            string query = $@"
                SELECT
                    Users.ID,
                    Users.PublicID,
                    Users.Nickname,
                    User.Pronoun,
                    Users.CreationTime,
                    Users.ProfilePicture,
                    Users.Status
                FROM Users INNER JOIN Friends ON Friends.UserID = Users.ID
                ";
            var reader = ExecuteReader(query);
            if (reader is null) {
                friends = null;
                return DatabaseCommandResult.DatabaseError;
            }
            friends = new List<User>();
            while (reader.Read()) {
                User friend = GetUser(reader);
                friends.Add(friend);
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateDefaultGuildPrivilege(ulong guildID) {
            GuildPrivilege privilege = new(0, 0, guildID);
            if (InsertDefaultGuildPrivilege(privilege) == 0) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }

        //public DatabaseCommandResult CreateDefaultCategoryForGuild(ulong guildID,
        //    string categoryName, DateTime creationTime, out ulong defaultCategoryID) {
        //    defaultCategoryID = Insert("DefaultCategories",
        //        new Entry("GuildID", guildID),
        //        new Entry("Name", categoryName),
        //        new Entry("CreationTime", creationTime));

        //    if (defaultCategoryID == 0) {
        //        return DatabaseCommandResult.DatabaseError;
        //    }
        //    return DatabaseCommandResult.Success;
        //}

        public DatabaseCommandResult CreateGuildPrivilegeForOwner(ulong userID, ulong guildID) {
            GuildPrivilege privilege = new(0, userID, guildID) {
                ManageGuild = PrivilegeValue.Positive,
                ManagePrivileges = PrivilegeValue.Positive,

                CreateCategory = PrivilegeValue.Positive,
                UpdateCategory = PrivilegeValue.Positive,
                DeleteCategory = PrivilegeValue.Positive,

                CreateChannel = PrivilegeValue.Positive,
                UpdateChannel = PrivilegeValue.Positive,
                DeleteChannel = PrivilegeValue.Positive,

                Read = PrivilegeValue.Positive,
                Write = PrivilegeValue.Positive
            };

            if (InsertGuildPrivilege(privilege) == 0) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateCategoryPrivilegeForOwner(ulong userID, ulong guildID) {
            CategoryPrivilege privilege = new(0, userID, guildID) {
                UpdateCategory = PrivilegeValue.Positive,
                DeleteCategory = PrivilegeValue.Positive,

                CreateChannel = PrivilegeValue.Positive,
                UpdateChannel = PrivilegeValue.Positive,
                DeleteChannel = PrivilegeValue.Positive,
                ViewCategory = PrivilegeValue.Positive,

                Read = PrivilegeValue.Positive,
                Write = PrivilegeValue.Positive
            };

            if (InsertCategoryPrivilege(privilege) == 0) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateTextChannelPrivilegeForOwner(ulong userID, ulong guildID) {
            TextChannelPrivilege privilege = new(0, userID, guildID) {
                UpdateChannel = PrivilegeValue.Positive,
                DeleteChannel = PrivilegeValue.Positive,

                Read = PrivilegeValue.Positive,
                Write = PrivilegeValue.Positive,
                ViewChannel = PrivilegeValue.Positive
            };

            if (InsertTextChannelPrivilege(privilege) == 0) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateGuild(CreateGuildData data, out Guild? guild) {
            guild = null;
            DateTime creationTime = DateTime.Now;
            string publicID = Guid.NewGuid().ToString();

            ulong guildID = Insert("Guilds",
                new Entry("Name", data.Name),
                new Entry("PublicID", publicID),
                new Entry("Password", data.Password),
                new Entry("OwnerID", data.OwnerID),
                new Entry("CreationTime", creationTime),
                new Entry("Icon", data.Icon));

            if (guildID == 0) {
                return DatabaseCommandResult.DatabaseError;
            }

            // add owner of guild to the guild
            ulong affiliationID = Insert("GuildAffiliations",
                new Entry("GuildID", guildID),
                new Entry("UserID", data.OwnerID));

            if (affiliationID == 0) {
                return DatabaseCommandResult.DatabaseError;
            }

            // create default category
            //const string defaultCategoryName = "GENERAL";
            CreateCategoryData createCategoryData = new(guildID, "GENERAL");
            var r1 = CreateCategory(createCategoryData, out Category? defaultCategory, false);
            if (r1 != DatabaseCommandResult.Success || defaultCategory == null) {
                return r1;
            }

            // create default text channel
            CreateTextChannelData defaultTextChannelData = new(defaultCategory.ID, "general");
            var r2 = CreateTextChannel(defaultTextChannelData, out TextChannel? defaultTextChannel, false);

            if (defaultTextChannel == null) {
                return DatabaseCommandResult.DatabaseError;
            }

            var r3 = CreateDefaultGuildPrivilege(guildID);
            if (r3 != DatabaseCommandResult.Success) {
                return r3;
            }

            var r4 = CreateGuildPrivilegeForOwner(data.OwnerID, guildID);
            if (r4 != DatabaseCommandResult.Success) {
                return r4;
            }

            var r5 = CreateCategoryPrivilegeForOwner(data.OwnerID, guildID);
            if (r5 != DatabaseCommandResult.Success) {
                return r5;
            }

            var r6 = CreateTextChannelPrivilegeForOwner(data.OwnerID, guildID);
            if (r6 != DatabaseCommandResult.Success) {
                return r6;
            }

            guild = new(guildID, publicID, data.Name, data.Password, data.OwnerID, creationTime, data.Icon);
            //Category defaultCategory = new(defaultCategory.ID, guildID, defaultCategoryName, creationTime);
            //TextChannel defaultTextChannel = new(defaultTextChannelID, defaultCategoryID, defaultTextChannelName, creationTime);
            defaultCategory.TextChannels.Add(defaultTextChannel);
            guild.Categories.Add(defaultCategory);

            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateDefaultCategoryPrivilege(ulong categoryID) {
            CategoryPrivilege privilege = new(0, 0, categoryID);
            if (InsertDefaultCategoryPrivilege(privilege) == 0) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - insertion failed.");
                return DatabaseCommandResult.DatabaseError;
            }
            Logger.Info($"Created default privilage for category '{categoryID}'.");
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateCategory(CreateCategoryData data, out Category? category, bool createPrivilege = true) {
            category = null;
            DateTime creationTime = DateTime.Now;

            ulong categoryID = Insert("Categories",
                new Entry("GuildID", data.GuildID),
                new Entry("Name", data.Name),
                new Entry("CreationTime", creationTime));

            if (categoryID == 0) {
                Logger.Error($"{MethodBase.GetCurrentMethod()} - insertion failed.");
                return DatabaseCommandResult.DatabaseError;
            }

            if (createPrivilege) {
                string query = $@"
                    SELECT GuildAffiliations.UserID
                    FROM GuildAffiliations
                        INNER JOIN Guilds ON Guilds.ID = GuildAffiliations.ID
                        INNER JOIN Categories ON Categories.GuildID = Guilds.ID
                    WHERE Categories.ID = {categoryID}";
                var reader = ExecuteReader(query);
                if (reader == null) {
                    return DatabaseCommandResult.DatabaseError;
                }
                while (reader.Read()) {
                    ulong userID = reader.GetFieldValue<ulong>(0);

                    CategoryPrivilege privilege = new(0, userID, categoryID) {
                        UpdateCategory = PrivilegeValue.Neutral,
                        DeleteCategory = PrivilegeValue.Neutral,
                        ViewCategory = PrivilegeValue.Positive,
                        CreateChannel = PrivilegeValue.Neutral,
                        UpdateChannel = PrivilegeValue.Neutral,
                        DeleteChannel = PrivilegeValue.Neutral,
                        Read = PrivilegeValue.Neutral,
                        Write = PrivilegeValue.Neutral
                    };
                    if (InsertCategoryPrivilege(privilege) == 0) {
                        Logger.Error("Failed to insert CategoryPrivilege");
                    }
                }
            }

            var r1 = CreateDefaultCategoryPrivilege(categoryID);
            if (r1 != DatabaseCommandResult.Success) {
                return r1;
            }

            category = new Category(categoryID, data.GuildID, data.Name, creationTime);
            return DatabaseCommandResult.Success;
        }


        public DatabaseCommandResult GetUsersInCategory(Category category, out List<ulong>? users) {
            var reader = ExecuteReader($@"
                SELECT GuildAffiliations.UserID
                FROM GuildAffiliations
                    INNER JOIN Guilds on GuildAffiliations.GuildID = Guilds.ID
                    INNER JOIN Categories on Guilds.ID = Categories.GuildID
                WHERE Categories.ID = {category.ID}
                ;");
            if (reader is null) {
                users = null;
                return DatabaseCommandResult.DatabaseError;
            }
            users = new List<ulong>();
            while (reader.Read()) {
                users.Add(reader.GetFieldValue<ulong>(0));
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult GetUserIDsInTextChannel(ulong textChannelID,
            out List<ulong>? userIDs, TextChannelPrivilege? privilege = null) {
            try {
                SqliteDataReader? reader;
                if (privilege != null) {
                    string query = $@"
                        SELECT TextChannelPrivileges.UserID
                        FROM TextChannelPrivileges
                        WHERE TextChannelPrivileges.ChannelID = {textChannelID}
                            AND TextChannelPrivileges.ViewChannel <= {(byte)privilege.ViewChannel}
                            AND TextChannelPrivileges.Read <= {(byte)privilege.Read}";
                    Logger.Warning(query);
                    reader = ExecuteReader(query);
                }
                else {
                    reader = ExecuteReader($@"
                        SELECT TextChannelPrivileges.UserID
                        FROM TextChannelPrivileges
                        WHERE TextChannelPrivileges.ChannelID = {textChannelID};");
                }

                if (reader == null) {
                    userIDs = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                userIDs = new List<ulong>();
                while (reader.Read()) {
                    ulong userID = reader.GetFieldValue<ulong>(0);
                    userIDs.Add(userID);
                }
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                userIDs = null;
                return DatabaseCommandResult.UnknownError;
            }
        }

        public DatabaseCommandResult CreateDefaultTextChannelPrivilege(ulong textChannelID) {
            TextChannelPrivilege privilege = new(0, 0, textChannelID);
            if (InsertDefaultTextChannelPrivilege(privilege) == 0) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateTextChannel(CreateTextChannelData data, out TextChannel? textChannel, bool createPrivilege = true) {
            DateTime creationTime = DateTime.Now;
            textChannel = null;
            try {
                ulong textChannelID = Insert("TextChannels",
                    new Entry("CategoryID", data.CategoryID),
                    new Entry("Name", data.Name),
                    new Entry("CreationTime", creationTime)
                );

                if (textChannelID == 0) {
                    Logger.Error("ID of new text channel is '0'!");
                    return DatabaseCommandResult.DatabaseError;
                }
                if (createPrivilege) {
                    string query = $@"
                        SELECT GuildAffiliations.UserID
                        FROM GuildAffiliations
                            INNER JOIN Guilds ON Guilds.ID = GuildAffiliations.ID
                            INNER JOIN Categories ON Categories.GuildID = Guilds.ID
                        WHERE Categories.ID = {data.CategoryID}";
                    var reader = ExecuteReader(query);
                    if (reader == null) {
                        return DatabaseCommandResult.DatabaseError;
                    }
                    while (reader.Read()) {
                        ulong userID = reader.GetFieldValue<ulong>(0);
                        Insert("TextChannelPrivileges",
                            new Entry("UserID", userID),
                            new Entry("ChannelID", textChannelID),
                            new Entry("UpdateChannel", (byte)PrivilegeValue.Neutral),
                            new Entry("DeleteChannel", (byte)PrivilegeValue.Neutral),
                            new Entry("Read", (byte)PrivilegeValue.Neutral),
                            new Entry("Write", (byte)PrivilegeValue.Neutral),
                            new Entry("ViewChannel", (byte)PrivilegeValue.Neutral)
                        );
                    }
                }
                
                var r1 = CreateDefaultTextChannelPrivilege(textChannelID);
                if (r1 != DatabaseCommandResult.Success) {
                    return r1;
                }
                

                textChannel = new TextChannel(textChannelID, data.CategoryID, data.Name, creationTime);
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                textChannel = null;
                return DatabaseCommandResult.Fail;
            }
        }


        public DatabaseCommandResult GetTextChannelsInCategory(ulong categoryID, out ObservableCollection<TextChannel>? textChannels) {
            try {
                var textChannelReader = ExecuteReader($@"
                    SELECT TextChannels.ID, TextChannels.CategoryID, TextChannels.Name, TextChannels.CreationTime
                    FROM TextChannels
                    WHERE TextChannels.CategoryID = {categoryID}
                    ;");
                if (textChannelReader is null) {
                    textChannels = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                textChannels = new ObservableCollection<TextChannel>();
                while (textChannelReader.Read()) {
                    byte field = 0;
                    ulong textChannelID = textChannelReader.GetFieldValue<ulong>(field++);
                    ulong textChannelCategoryID = textChannelReader.GetFieldValue<ulong>(field++);
                    string textChannelName = textChannelReader.GetFieldValue<string>(field++);
                    DateTime textChannelCreationTime = textChannelReader.GetFieldValue<DateTime>(field++);

                    TextChannel textChannel = new(textChannelID, textChannelCategoryID,
                        textChannelName, textChannelCreationTime);
                    textChannels.Add(textChannel);
                }
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                textChannels = null;
                return DatabaseCommandResult.Fail;
            }
        }

        public DatabaseCommandResult GetCategoriesInGuild(ulong guildID, out ObservableCollection<Category>? categories) {
            try {
                var categoryReader = ExecuteReader($@"
                SELECT Categories.ID, Categories.GuildID, Categories.Name, Categories.CreationTime
                FROM Categories
                WHERE Categories.GuildID = {guildID}
                ;");

                if (categoryReader is null) {
                    categories = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                categories = new ObservableCollection<Category>();
                while (categoryReader.Read()) {
                    byte field = 0;
                    ulong categoryID = categoryReader.GetFieldValue<ulong>(field++);
                    ulong categoryGuildID = categoryReader.GetFieldValue<ulong>(field++);
                    string categoryName = categoryReader.GetFieldValue<string>(field++);
                    DateTime categoryCreationTime = DateTime.Parse(categoryReader.GetFieldValue<string>(field++));

                    var result = GetTextChannelsInCategory(categoryID, out ObservableCollection<TextChannel>? textChannels);
                    
                    if (result == DatabaseCommandResult.Success && textChannels != null) {
                        Category category = new(categoryID, categoryGuildID, categoryName, categoryCreationTime);
                        category.TextChannels = textChannels;
                        categories.Add(category);
                    }
                    else {
                        categories = null;
                        return result;
                    }
                }
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                categories = null;
                return DatabaseCommandResult.Fail;
            }
        }




        public DatabaseCommandResult GetGuildsForUser(ulong userID, out ObservableCollection<Guild>? guilds) {
            try {
                var guildReader = ExecuteReader(@$"
                    SELECT
                        Guilds.ID,
                        Guilds.PublicID,
                        Guilds.Name,
                        Guilds.Password,
                        Guilds.OwnerID,
                        Guilds.CreationTime,
                        Guilds.Icon
                    FROM Guilds INNER JOIN GuildAffiliations ON Guilds.ID = GuildAffiliations.GuildID
                    WHERE GuildAffiliations.UserID = {userID}
                    ;");
                if (guildReader is null) {
                    guilds = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                guilds = new ObservableCollection<Guild>();
                while (guildReader.Read()) {
                    byte field = 0;
                    ulong guildID = guildReader.GetFieldValue<ulong>(field++);
                    string guildPublicID = guildReader.GetFieldValue<string>(field++);
                    string guildName = guildReader.GetFieldValue<string>(field++);
                    string guildPassword = guildReader.GetFieldValue<string>(field++);
                    ulong guildOwnerID = guildReader.GetFieldValue<ulong>(field++);
                    DateTime guildCreationTime = guildReader.GetFieldValue<DateTime>(field++);
                    byte[] guildIcon = guildReader.GetFieldValue<byte[]>(field++);

                    var result = GetCategoriesInGuild(guildID, out ObservableCollection<Category>? categories);
                    if (result == DatabaseCommandResult.Success && categories != null) {
                        Guild guild = new(guildID, guildPublicID, guildName, guildPassword, guildOwnerID, guildCreationTime, guildIcon);
                        guild.Categories = categories;
                        guilds.Add(guild);
                    }
                    else {
                        guilds = null;
                        return result;
                    }
                }
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                guilds = null;
                return DatabaseCommandResult.Fail;
            }
        }


        public DatabaseCommandResult GetMessageAttachments(ulong messageID, out ObservableCollection<MessageAttachment>? attachments) {
            try {
                var reader = ExecuteReader($@"
                SELECT MessageAttachments.ID, MessageAttachments.Content
                FROM MessageAttachments
                WHERE MessageAttachments.MessageID = {messageID};");
                if (reader == null) {
                    attachments = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                attachments = new ObservableCollection<MessageAttachment>();
                while (reader.Read()) {
                    byte field = 0;
                    ulong id = reader.GetFieldValue<ulong>(field++);
                    byte[] content = reader.GetFieldValue<byte[]>(field++);

                    MessageAttachment attachment = new(id, messageID, content);
                    attachments.Add(attachment);
                }
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                attachments = null;
                return DatabaseCommandResult.UnknownError;
            }
        }

        public DatabaseCommandResult GetMessageRange(ulong textChannelID, ulong first, uint limit, out ObservableCollection<Message>? messages) {
            try {
                var reader = ExecuteReader($@"
                    SELECT Messages.ID, Messages.UserID, Messages.Content, Messages.CreationTime
                    FROM Messages
                    WHERE Messages.ID < {first}
                    LIMIT {limit};");
                if (reader == null) {
                    messages = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                messages = new ObservableCollection<Message>();
                while (reader.Read()) {
                    byte field = 0;
                    ulong id = reader.GetFieldValue<ulong>(field++);
                    ulong authorID = reader.GetFieldValue<ulong>(field++);
                    string content = reader.GetFieldValue<string>(field++);
                    DateTime time = reader.GetFieldValue<DateTime>(field++);

                    var result = GetMessageAttachments(id, out ObservableCollection<MessageAttachment>? attachments);
                    if (result == DatabaseCommandResult.Success && attachments != null) {
                        Message message = new(id, textChannelID, authorID, content, time) {
                            Attachments = attachments
                        };

                        result = GetUser(message.AuthorID, out User? author);
                        if (result == DatabaseCommandResult.Success && author != null) {
                            message.Author = author;
                            messages.Add(message);
                        }
                    }
                }
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                messages = null;
                return DatabaseCommandResult.UnknownError;
            }
        }

        public DatabaseCommandResult GetGuildPrivilegeForUser(ulong userID, out GuildPrivilege? privilege) {
            try {
                SqliteCommand command = Connection.CreateCommand();
                command.CommandText = $@"
                SELECT
                    GuildPrivileges.ID,
                    GuildPrivileges.GuildID,
                    GuildPrivileges.ManageGuild,
                    GuildPrivileges.ManagePrivilages,
                    GuildPrivileges.CreateCategory,
                    GuildPrivileges.UpdateCategory,
                    GuildPrivileges.DeleteCategory,
                    GuildPrivileges.CreateChannel,
                    GuildPrivileges.UpdateChannel,
                    GuildPrivileges.DeleteChannel,
                    GuildPrivileges.Read,
                    GuildPrivileges.Write
                FROM GuildPrivileges
                WHERE GuildPrivileges.UserID = {userID}
                ;";
                var reader = command.ExecuteReader();
                if (reader == null) {
                    privilege = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                if (reader.Read()) {
                    byte field = 0;
                    ulong id = reader.GetFieldValue<ulong>(field++);
                    ulong guildID = reader.GetFieldValue<ulong>(field++);
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

                    privilege = new GuildPrivilege(id, userID, guildID) {
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
                    };
                    return DatabaseCommandResult.Success;
                }
                privilege = null;
                return DatabaseCommandResult.Fail;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                privilege = null;
                return DatabaseCommandResult.UnknownError;
            }
        }

        public DatabaseCommandResult GetCategoryPrivilegeForUser(ulong userID, out CategoryPrivilege? privilege) {
            try {
                SqliteCommand command = Connection.CreateCommand();
                command.CommandText = $@"
                    SELECT
                        CategoryPrivileges.ID,
                        CategoryPrivileges.CategoryID,
                        CategoryPrivileges.UpdateCategory,
                        CategoryPrivileges.DeleteCategory,
                        CategoryPrivileges.ViewCategory,
                        CategoryPrivileges.CreateChannel,
                        CategoryPrivileges.UpdateChannel,
                        CategoryPrivileges.DeleteChannel,
                        CategoryPrivileges.Read,
                        CategoryPrivileges.Write
                    FROM CategoryPrivileges
                    WHERE CategoryPrivileges.UserID = {userID}
                    ;";
                var reader = command.ExecuteReader();
                if (reader == null) {
                    privilege = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                if (reader.Read()) {
                    byte field = 0;
                    ulong id = reader.GetFieldValue<ulong>(field++);
                    ulong categoryID = reader.GetFieldValue<ulong>(field++);
                    PrivilegeValue updateCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue deleteCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue viewCategory = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue createChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);

                    privilege = new CategoryPrivilege(id, userID, categoryID) {
                        UpdateCategory = updateCategory,
                        DeleteCategory = deleteCategory,
                        ViewCategory = viewCategory,
                        CreateChannel = createChannel,
                        UpdateChannel = updateChannel,
                        DeleteChannel = deleteChannel,
                        Read = read,
                        Write = write,
                    };
                    return DatabaseCommandResult.Success;
                }
                privilege = null;
                return DatabaseCommandResult.Fail;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                privilege = null;
                return DatabaseCommandResult.UnknownError;
            }
        }

        public DatabaseCommandResult GetTextChannelPrivilegeForUser(ulong userID, out TextChannelPrivilege? privilege) {
            try {
                SqliteCommand command = Connection.CreateCommand();
                command.CommandText = $@"
                    SELECT
                        TextChannelPrivileges.ID,
                        TextChannelPrivileges.TextChannelID,
                        TextChannelPrivileges.UpdateChannel,
                        TextChannelPrivileges.DeleteChannel,
                        TextChannelPrivileges.Read,
                        TextChannelPrivileges.Write
                        TextChannelPrivileges.ViewChannel,
                    FROM TextChannelPrivileges
                    WHERE TextChannelPrivileges.UserID = {userID}
                    ;";
                var reader = command.ExecuteReader();
                if (reader == null) {
                    privilege = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                if (reader.Read()) {
                    byte field = 0;
                    ulong id = reader.GetFieldValue<ulong>(field++);
                    ulong textChannelID = reader.GetFieldValue<ulong>(field++);
                    PrivilegeValue updateChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue deleteChannel = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue read = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue write = reader.GetFieldValue<PrivilegeValue>(field++);
                    PrivilegeValue viewChannel = reader.GetFieldValue<PrivilegeValue>(field++);

                    privilege = new TextChannelPrivilege(id, userID, textChannelID) {
                        UpdateChannel = updateChannel,
                        DeleteChannel = deleteChannel,
                        Read = read,
                        Write = write,
                        ViewChannel = viewChannel,
                    };
                    return DatabaseCommandResult.Success;
                }
                privilege = null;
                return DatabaseCommandResult.Fail;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                privilege = null;
                return DatabaseCommandResult.UnknownError;
            }
        }

        public DatabaseCommandResult GetUser(ulong userID, out User? user) {
            try {
                var reader = ExecuteReader($@"
                    SELECT
                        Users.ID,
                        Users.PublicID,
                        Users.Nickname,
                        Users.Pronoun,
                        Users.CreationTime,
                        Users.ProfilePicture,
                        Users.Status
                    FROM Users
                    WHERE Users.ID = {userID};");
                if (reader == null) {
                    user = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                if (reader.Read()) {
                    user = GetUser(reader);
                    return DatabaseCommandResult.Success;
                }
                user = null;
                return DatabaseCommandResult.UnknownError;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                user = null;
                return DatabaseCommandResult.UnknownError;
            }
        }


        public DatabaseCommandResult SendMessage(MessageData data, out Message? message) {
            try {
                DateTime creationTime = DateTime.Now;
                ulong id = Insert("Messages",
                    new Entry("UserID", data.UserID),
                    new Entry("TextChannelID", data.TextChannelID),
                    new Entry("Content", data.Content),
                    new Entry("CreationTime", creationTime));
                if (id == 0) {
                    message = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                var result = GetUser(data.UserID, out User? author);
                Logger.Info("----------------- TEST ------------");
                if (result == DatabaseCommandResult.Success) {
                    message = new Message(id, data.TextChannelID, data.UserID, data.Content, creationTime);
                    message.Author = author;
                    return DatabaseCommandResult.Success;
                }
                message = null;
                return DatabaseCommandResult.UnknownError;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{ex.Message.Replace("\n", indent)}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                message = null;
                return DatabaseCommandResult.UnknownError;
            }
        }



        public DatabaseCommandResult GrantPrivilegesToJoiningUser(ulong userID, ulong guildID) {
            try {
                GuildPrivilege? guildPrivilege = GetDefaultGuildPrivilege(guildID);
                if (guildPrivilege == null) {
                    return DatabaseCommandResult.DatabaseError;
                }
                guildPrivilege.UserID = userID;
                var r1 = GetCategoriesInGuild(guildID, out ObservableCollection<Category>? categories);
                if (r1 != DatabaseCommandResult.Success || categories == null) {
                    Logger.Error("Failed to get categories");
                    return r1;
                }


                foreach (var category in categories) {
                    CategoryPrivilege? categoryPrivilege = GetDefaultCategoryPrivilege(category.ID);
                    if (categoryPrivilege == null) {
                        Logger.Error("Category Privilege is null");
                        return DatabaseCommandResult.DatabaseError;
                    }
                    categoryPrivilege.UserID = userID;
                    var r2 = GetTextChannelsInCategory(category.ID, out ObservableCollection<TextChannel>? textChannels);
                    if (r2 != DatabaseCommandResult.Success || textChannels == null) {
                        Logger.Error("Faield to get text channels");
                        return r2;
                    }
                    foreach (var textChannel in textChannels) {
                        TextChannelPrivilege? textChannelPrivilege = GetDefaultTextChannelPrivilege(textChannel.ID);
                        if (textChannelPrivilege == null) {
                            Logger.Error("Text channel privilege is null");
                            return DatabaseCommandResult.DatabaseError;
                        }
                        textChannelPrivilege.UserID = userID;
                        if (InsertTextChannelPrivilege(textChannelPrivilege) == 0) {
                            Logger.Error("Failed to insert text channel privilege");
                            return DatabaseCommandResult.DatabaseError;
                        }
                    }
                    if (InsertCategoryPrivilege(categoryPrivilege) == 0) {
                        Logger.Error("Failed to insert category privilege");
                        return DatabaseCommandResult.DatabaseError;
                    }
                }

                if (InsertGuildPrivilege(guildPrivilege) == 0) {
                    Logger.Error("Failed to insert guild privilege");
                    return DatabaseCommandResult.DatabaseError;
                }
                Logger.Info($"Successfully granted privileges to new user '{userID}'.");
                
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                return DatabaseCommandResult.UnknownError;
            }
        }

        public DatabaseCommandResult JoinGuild(JoinGuildData data, out Guild? guild) {
            try {
                var reader = ExecuteReader($@"
                    SELECT Guilds.ID, Guilds.Name, Guilds.OwnerID, Guilds.CreationTime, Guilds.Icon
                    FROM Guilds
                    WHERE Guilds.PublicID = '{data.PublicID}'
                        AND Guilds.Password = '{data.Password}'
                    ;");

                if (reader == null) {
                    guild = null;
                    return DatabaseCommandResult.DatabaseError;
                }
                if (reader.Read()) {
                    byte field = 0;
                    ulong guildID = reader.GetFieldValue<ulong>(field++);
                    string name = reader.GetFieldValue<string>(field++);
                    ulong owner = reader.GetFieldValue<ulong>(field++);
                    DateTime time = reader.GetFieldValue<DateTime>(field++);
                    byte[] icon = reader.GetFieldValue<byte[]>(field++);

                    Insert("GuildAffiliations",
                        new Entry("GuildID", guildID),
                        new Entry("UserID", data.UserID));
                    guild = new Guild(guildID, data.PublicID, name, "", owner, time, icon);
                    GetCategoriesInGuild(guildID, out ObservableCollection<Category>? categories);
                    if (categories != null) {
                        guild.Categories = categories;
                        foreach (var category in guild.Categories) {
                            GetTextChannelsInCategory(category.ID, out ObservableCollection<TextChannel>? textChannels);
                            if (textChannels != null) {
                                category.TextChannels = textChannels;
                            }
                        }
                    }

                    var r1 = GrantPrivilegesToJoiningUser(data.UserID, guildID);
                    if (r1 != DatabaseCommandResult.Success) {
                        return r1;
                    }

                    return DatabaseCommandResult.Success;
                }
                guild = null;
                return DatabaseCommandResult.Fail;
            }
            catch (Exception ex) {
                var method = MethodBase.GetCurrentMethod();
                if (method != null) {
                    const string indent = "\n\t\t\t       ";
                    string message = ex.Message.Replace("\n", indent);
                    Logger.Error($"{ex.GetType().Name} thrown in [{method.Name}]:{indent}{message}");
                }
                else {
                    Logger.Error(ex.Message);
                }
                guild = null;
                return DatabaseCommandResult.UnknownError;
            }
        }


    }
}
