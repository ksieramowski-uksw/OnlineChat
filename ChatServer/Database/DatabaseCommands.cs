using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using ChatShared.Models.Privileges;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ChatServer.Database {
    public class DatabaseCommands {
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


        private ulong Insert(string tableName, params Entry[] entries) {
            StringBuilder query = new($"INSERT INTO {tableName}(");
            for (int i = 0; i < entries.Length; i++) {
                if (i != (entries.Length - 1)) {
                    query.Append($"{entries[i].Key}, ");
                    continue;
                }
                query.Append($"{entries[i].Key}) VALUES (");
            }
            for (int i = 0; i < entries.Length; i++) {
                if (i != (entries.Length - 1)) {
                    query.Append($"@{entries[i].Key}, ");
                    continue;
                }
                query.Append($"@{entries[i].Key}) RETURNING ID;");
            }

            using var command = Connection.CreateCommand();
            command.CommandText = query.ToString();

            for (int i = 0; i < entries.Length; i++) {
                command.Parameters.AddWithValue($"@{entries[i].Key}", entries[i].Value);
            }

            ulong id = 0;
            try {
                var reader = command.ExecuteReader();
                if (reader is null) {
                    return 0;
                }
                if (reader.Read()) {
                    id = reader.GetFieldValue<ulong>(0);
                }
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
            }

            return id;
        }

        //private Tuple<object[], Type[]> Select(string query, Type[] types) {
        //    SqliteCommand command = Connection.CreateCommand();

        //    object[] instances = new object[types.Length];

        //    for (int i = 0; i < types.Length; i++) {
        //        instances = 
        //    }

        //    string publicID = Guid.NewGuid().ToString();
        //    ulong id = reader.GetFieldValue<ulong>(0);
        //    string nickname = reader.GetFieldValue<string>(1);
        //    string email = reader.GetFieldValue<string>(2);
        //    string pronoun = reader.GetFieldValue<string>(3);
        //    DateTime creationTime = DateTime.Parse(reader.GetFieldValue<string>(4));
        //    byte[] profilePicture = reader.GetFieldValue<byte[]>(5);
        //    UserStatus status = reader.GetFieldValue<UserStatus>(6);

        //    var t = Tuple.Create(instances, types);
        //    t
        //    return ;
        //}


        public DatabaseCommandResult TryRegisterUser(RegisterData data) {
            DateTime creationTime = DateTime.Now;
            bool userExists = EntityExists(@$"
                SELECT ID
                FROM Users
                WHERE Users.Email LIKE '{data.Email}' OR Users.Nickname LIKE '{data.Nickname}'
                ;");
            if (userExists) { return DatabaseCommandResult.UserExists; }

            ulong id = Insert("Users",
                new Entry("Email", data.Email),
                new Entry("Password", data.Password),
                new Entry("Nickname", data.Nickname),
                new Entry("Pronoun", data.Pronoun),
                new Entry("CreationTime", creationTime),
                new Entry("ProfilePicture", data.ProfilePicture),
                new Entry("Status", UserStatus.Offline));

            if (id == 0) { return DatabaseCommandResult.DatabaseError; }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult TryLogIn(LoginData data, out User? userData) {
            var reader = ExecuteReader(@$"
                SELECT Users.ID, Users.Nickname, Users.Email, Users.Pronoun, Users.CreationTime, Users.ProfilePicture, Users.Status
                FROM Users
                WHERE Users.Email LIKE '{data.Email}' AND Users.Password LIKE '{data.Password}'
                ;");
            if (reader is null) {
                userData = null;
                return DatabaseCommandResult.DatabaseError;
            }
            if (reader.Read()) {
                string publicID = Guid.NewGuid().ToString();
                ulong id = reader.GetFieldValue<ulong>(0);
                string nickname = reader.GetFieldValue<string>(1);
                string email = reader.GetFieldValue<string>(2);
                string pronoun = reader.GetFieldValue<string>(3);
                DateTime creationTime = DateTime.Parse(reader.GetFieldValue<string>(4));
                byte[] profilePicture = reader.GetFieldValue<byte[]>(5);
                UserStatus status = reader.GetFieldValue<UserStatus>(6);

                userData = new User(id, publicID, nickname, email, pronoun, creationTime, profilePicture, status);
                return DatabaseCommandResult.Success;
            }
            userData = null;
            return DatabaseCommandResult.Fail;
        }

        public DatabaseCommandResult GetFriends(ulong userID, out List<User>? friends) {
            const string query = $@"
                SELECT Users.ID, Users.PublicID, Users.Nickname, User.Pronoun, Users.CreationTime, Users.ProfilePicture, Users.Status
                FROM Users INNER JOIN Friends ON Friends.UserID = Users.ID
                ";
            var reader = ExecuteReader(query);
            if (reader is null) {
                friends = null;
                return DatabaseCommandResult.DatabaseError;
            }
            friends = new List<User>();
            while (reader.Read()) {
                ulong id = reader.GetFieldValue<ulong>(0);
                string publicID = reader.GetFieldValue<string>(1);
                string nickname = reader.GetFieldValue<string>(2);
                string pronoun = reader.GetFieldValue<string>(3);
                DateTime creationTime = reader.GetFieldValue<DateTime>(4);
                byte[] profilePicture = reader.GetFieldValue<byte[]>(5);
                UserStatus status = reader.GetFieldValue<UserStatus>(6);

                User friend = new(id, publicID, nickname, string.Empty, pronoun, creationTime, profilePicture, status);
                friends.Add(friend);
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateGuild(CreateGuildData data, out Guild? guild) {
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
                guild = null;
                return DatabaseCommandResult.DatabaseError;
            }

            ulong affiliationID = Insert("GuildAffiliations",
                new Entry("GuildID", guildID),
                new Entry("UserID", data.OwnerID));

            if (affiliationID == 0) {
                guild = null;
                return DatabaseCommandResult.DatabaseError;
            }

            guild = new(guildID, publicID, data.Name, data.Password, data.OwnerID, creationTime, data.Icon);
            return DatabaseCommandResult.Success;
        }


        public DatabaseCommandResult CreateCategory(CreateCategoryData data, out Category? category) {
            DateTime creationTime = DateTime.Now;

            ulong categoryID = Insert("Categories",
                new Entry("GuildID", data.GuildID),
                new Entry("Name", data.Name),
                new Entry("CreationTime", creationTime));

            if (categoryID == 0) {
                category = null;
                return DatabaseCommandResult.DatabaseError;
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

        public DatabaseCommandResult CreateTextChannel(CreateTextChannelData data, out TextChannel? textChannel) {
            DateTime creationTime = DateTime.Now;

            ulong textChannelID = Insert("TextChannels",
                new Entry("CategoryID", data.CategoryID),
                new Entry("Name", data.Name),
                new Entry("CreationTime", creationTime));

            if (textChannelID == 0) {
                textChannel = null;
                return DatabaseCommandResult.DatabaseError;
            }

            textChannel = new TextChannel(textChannelID, data.CategoryID, data.Name, creationTime);
            return DatabaseCommandResult.Success;
        }


        public DatabaseCommandResult GetCategoriesInGuild(ulong guildID, out ObservableCollection<Category>? categories) {
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
                ulong categoryID = categoryReader.GetFieldValue<ulong>(0);
                ulong categoryGuildID = categoryReader.GetFieldValue<ulong>(1);
                string categoryName = categoryReader.GetFieldValue<string>(2);
                DateTime categoryCreationTime = DateTime.Parse(categoryReader.GetFieldValue<string>(3));

                Category category = new(categoryID, categoryGuildID, categoryName, categoryCreationTime);

                var textChannelReader = ExecuteReader($@"
                    SELECT TextChannels.ID, TextChannels.CategoryID, TextChannels.Name
                    FROM TextChannels
                    WHERE TextChannels.CategoryID = {categoryID}
                    ;");
                if (textChannelReader is null) {
                    categories = null;
                    return DatabaseCommandResult.DatabaseError;
                }

                while (textChannelReader.Read()) {
                    ulong textChannelID = textChannelReader.GetFieldValue<ulong>(0);
                    ulong textChannelCategoryID = textChannelReader.GetFieldValue<ulong>(1);
                    string textChannelName = textChannelReader.GetFieldValue<string>(2);
                    DateTime textChannelCreationTime = DateTime.Parse(textChannelReader.GetFieldValue<string>(3));

                    TextChannel textChannel = new(textChannelID, textChannelCategoryID,
                        textChannelName, textChannelCreationTime);
                    category.TextChannels.Add(textChannel);
                }

                categories.Add(category);
            }
            return DatabaseCommandResult.Success;
        }


        public DatabaseCommandResult GetTextChannelsInCategory(ulong categoryID, out ObservableCollection<TextChannel>? textChannels) {
            var textChannelReader = ExecuteReader($@"
                    SELECT TextChannels.ID, TextChannels.CategoryID, TextChannels.Name
                    FROM TextChannels
                    WHERE TextChannels.CategoryID = {categoryID}
                    ;");
            if (textChannelReader is null) {
                textChannels = null;
                return DatabaseCommandResult.DatabaseError;
            }
            textChannels = new ObservableCollection<TextChannel>();
            while (textChannelReader.Read()) {
                ulong textChannelID = textChannelReader.GetFieldValue<ulong>(0);
                ulong textChannelCategoryID = textChannelReader.GetFieldValue<ulong>(1);
                string textChannelName = textChannelReader.GetFieldValue<string>(2);
                DateTime textChannelCreationTime = textChannelReader.GetFieldValue<DateTime>(3);

                TextChannel textChannel = new(textChannelID, textChannelCategoryID,
                    textChannelName, textChannelCreationTime);
                textChannels.Add(textChannel);
            }
            return DatabaseCommandResult.Success;
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
                    ulong guildID = guildReader.GetFieldValue<ulong>(0);
                    string guildPublicID = guildReader.GetFieldValue<string>(1);
                    string guildName = guildReader.GetFieldValue<string>(2);
                    string guildPassword = guildReader.GetFieldValue<string>(3);
                    ulong guildOwnerID = guildReader.GetFieldValue<ulong>(4);
                    DateTime guildCreationTime = guildReader.GetFieldValue<DateTime>(5);
                    byte[] guildIcon = guildReader.GetFieldValue<byte[]>(6);

                    Guild guild = new(guildID, guildPublicID, guildName, guildPassword, guildOwnerID, guildCreationTime, guildIcon);
                    guilds.Add(guild);

                    var result = GetCategoriesInGuild(guildID, out ObservableCollection<Category>? categories);
                    if (categories == null) {
                        guilds = null;
                        return DatabaseCommandResult.DatabaseError;
                    }
                    guild.Categories = categories;

                    foreach (var category in guild.Categories) {
                        GetTextChannelsInCategory(category.ID, out ObservableCollection<TextChannel>? textChannels);
                        if (textChannels == null) {
                            guilds = null;
                            return DatabaseCommandResult.DatabaseError;
                        }
                        category.TextChannels = textChannels;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                guilds = null;
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }


        //public DatabaseCommandResult GetCompleteGuildsInfo(ulong userID, out ObservableCollection<Guild>? collection) {
        //    try {
        //        var guildReader = ExecuteReader(@$"
        //            SELECT
        //                Guilds.ID,
        //                Guilds.PublicID,
        //                Guilds.Name,
        //                Guilds.Password,
        //                Guilds.OwnerID,
        //                Guilds.CreationTime,
        //                Guilds.Icon
        //            FROM Guilds INNER JOIN GuildAffiliations ON Guilds.ID = GuildAffiliations.GuildID
        //            WHERE GuildAffiliations.UserID = {userID}
        //            ;");
        //        if (guildReader is null) {
        //            collection = null;
        //            return DatabaseCommandResult.DatabaseError;
        //        }

        //        ObservableCollection<Guild> guilds = new();
        //        while (guildReader.Read()) {
        //            ulong guildID = guildReader.GetFieldValue<ulong>(0);
        //            string guildPublicID = guildReader.GetFieldValue<string>(1);
        //            string guildName = guildReader.GetFieldValue<string>(2);
        //            string guildPassword = guildReader.GetFieldValue<string>(3);
        //            ulong guildOwnerID = guildReader.GetFieldValue<ulong>(4);
        //            DateTime guildCreationTime = guildReader.GetFieldValue<DateTime>(5);
        //            byte[] guildIcon = guildReader.GetFieldValue<byte[]>(6);

        //            Guild guild = new(guildID, guildPublicID, guildName, guildPassword, guildOwnerID, guildCreationTime, guildIcon);

        //            var categoryReader = ExecuteReader($@"
        //                SELECT Categories.ID, Categories.GuildID, Categories.Name, Categories.CreationTime
        //                FROM Categories
        //                WHERE Categories.GuildID = {guildID}
        //                ;");
        //            if (categoryReader is null) {
        //                collection = null;
        //                return DatabaseCommandResult.DatabaseError;
        //            }

        //            ObservableCollection<Category> categories = new();
        //            while (categoryReader.Read()) {
        //                ulong categoryID = categoryReader.GetFieldValue<ulong>(0);
        //                ulong categoryGuildID = categoryReader.GetFieldValue<ulong>(1);
        //                string categoryName = categoryReader.GetFieldValue<string>(2);
        //                DateTime categoryCreationTime = DateTime.Parse(categoryReader.GetFieldValue<string>(3));

        //                Category category = new(categoryID, categoryGuildID, categoryName, categoryCreationTime);

        //                var textChannelReader = ExecuteReader($@"
        //                    SELECT TextChannels.ID, TextChannels.CategoryID, TextChannels.Name
        //                    FROM TextChannels
        //                    WHERE TextChannels.CategoryID = {categoryID}
        //                    ;");
        //                if (textChannelReader is null ) {
        //                    collection = null;
        //                    return DatabaseCommandResult.DatabaseError;
        //                }

        //                while (textChannelReader.Read()) {
        //                    ulong textChannelID = textChannelReader.GetFieldValue<ulong>(0);
        //                    ulong textChannelCategoryID = textChannelReader.GetFieldValue<ulong>(1);
        //                    string textChannelName = textChannelReader.GetFieldValue<string>(2);
        //                    DateTime textChannelCreationTime = textChannelReader.GetFieldValue<DateTime>(3);

        //                    TextChannel textChannel = new(textChannelID, textChannelCategoryID, textChannelName, categoryCreationTime);
        //                    category.TextChannels.Add(textChannel);
        //                }

        //                categories.Add(category);
        //            }

        //            guilds.Add(guild);
        //        }

        //        collection = guilds;
        //        return DatabaseCommandResult.Success;
        //    }
        //    catch (Exception ex) {
        //        Logger.Error(ex.Message);
        //        collection = null;
        //        return DatabaseCommandResult.DatabaseError;
        //    }
        //}

        public DatabaseCommandResult GetGuildPrivilegeForUser(ulong userID, out GuildPrivilege? privilege) {
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
                    ManagePrivilages = managePrivileges,
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

        public DatabaseCommandResult GetCategoryPrivilegeForUser(ulong userID, out CategoryPrivilege? privilege) {
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

        public DatabaseCommandResult GetTextChannelPrivilegeForUser(ulong userID, out TextChannelPrivilege? privilege) {
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


        public DatabaseCommandResult SendMessage(MessageData data) {
            DateTime creationTime = DateTime.Now;
            bool result = ExecuteNonQuery($@"
                INSERT INTO Messages (UserID, TextChannelID, Text, Time)
                    VALUES ('
                        {data.UserID}', '{data.TextChannelID}', '{data.Text}', '{creationTime}'
                    )
                ;");
            if (result == false) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }


        //public DatabaseCommandResult CreateRole(RoleData data) {

        //}
    }
}
