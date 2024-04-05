using ChatShared;
using ChatShared.DataModels;
using ChatShared.Models;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Data;
using System.Resources;
using System.Text;
using System.Text.Json;


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


        public DatabaseCommandResult TryRegisterUser(RegisterData data) {
            DateTime creationTime = DateTime.Now;
            bool userExists = EntityExists(@$"
                SELECT ID
                FROM Users
                WHERE Users.Email LIKE '{data.Email}' OR Users.Nickname LIKE '{data.Nickname}'
                ;");
            if (userExists) {
                return DatabaseCommandResult.UserExists;
            }

            using var command = Connection.CreateCommand();
            command.CommandText = @$"
                INSERT INTO Users (Email, Password, Nickname, Pronoun, CreationTime, ProfilePicture, Status)
                    VALUES (
                        '{data.Email}',
                        '{data.Password}',
                        '{data.Nickname}',
                        '{data.Pronoun}',
                        '{creationTime}',
                        @ProfilePicture,
                        '{UserStatus.Offline}'
                    )
                ;";
            command.Parameters.AddWithValue("@ProfilePicture", data.ProfilePicture);
            try {
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                return DatabaseCommandResult.DatabaseError;
            }

            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult TryLogIn(LoginData data, out User? userData) {
            var reader = ExecuteReader(@$"
                SELECT Users.Id, Users.Nickname, Users.Email, Users.Pronoun, Users.CreationTime, Users.ProfilePicture, Users.Status
                FROM Users
                WHERE Users.Email LIKE '{data.Email}' AND Users.Password LIKE '{data.Password}'
                ;");
            if (reader is null) {
                userData = null;
                return DatabaseCommandResult.DatabaseError;
            }
            if (reader.Read()) {
                ulong id = reader.GetFieldValue<ulong>(0);
                string nickname = reader.GetFieldValue<string>(1);
                string email = reader.GetFieldValue<string>(2);
                string pronoun = reader.GetFieldValue<string>(3);
                DateTime creationTime = DateTime.Parse(reader.GetFieldValue<string>(4));
                byte[] profilePicture = reader.GetFieldValue<byte[]>(5);
                UserStatus status = reader.GetFieldValue<UserStatus>(6);

                userData = new User(id, nickname, email, pronoun, creationTime, profilePicture, status);
                return DatabaseCommandResult.Success;
            }
            userData = null;
            return DatabaseCommandResult.Fail;
        }

        public DatabaseCommandResult GetFriends(ulong userId, out List<User>? friends) {
            SqliteCommand command = Connection.CreateCommand();
            command.CommandText = $@"
                SELECT Users.Id, Users.Nickname, User.Pronoun, Users.CreationTime, Users.ProfilePicture, Users.Status
                FROM Users INNER JOIN Friends ON Friends.UserId = Users.Id
                ";
            var reader = command.ExecuteReader();
            if (reader is null) {
                friends = null;
                return DatabaseCommandResult.DatabaseError;
            }
            friends = new List<User>();
            while (reader.Read()) {
                ulong id = reader.GetFieldValue<ulong>(0);
                string nickname = reader.GetFieldValue<string>(1);
                string pronoun = reader.GetFieldValue<string>(2);
                DateTime creationTime = reader.GetFieldValue<DateTime>(3);
                byte[] profilePicture = reader.GetFieldValue<byte[]>(4);
                UserStatus status = reader.GetFieldValue<UserStatus>(5);
                User friend = new(id, nickname, string.Empty, pronoun, creationTime, profilePicture, status);
                friends.Add(friend);
            }
            return DatabaseCommandResult.Success;
        }

        public DatabaseCommandResult CreateGuild(CreateGuildData data, out Guild? guild) {
            DateTime creationTime = DateTime.Now;
            string publicId = Guid.NewGuid().ToString();
            SqliteCommand command = Connection.CreateCommand();
                command.CommandText = @$"
                    INSERT INTO Guilds (Name, PublicId, Password, OwnerId, CreationTime, Icon)
                        VALUES (
                            @Name,
                            @PublicId,
                            @Password,
                            @OwnerId,
                            @CreationTime,
                            @Icon
                        )
                    ;";
                try {
                    command.Parameters.AddWithValue("@Name", data.Name);
                    command.Parameters.AddWithValue("@PublicId", publicId);
                    command.Parameters.AddWithValue("@Password", data.Password);
                    command.Parameters.AddWithValue("@OwnerId", data.OwnerId);
                    command.Parameters.AddWithValue("@CreationTime", creationTime);
                    command.Parameters.AddWithValue("@Icon", data.Icon);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    guild = null;
                    return DatabaseCommandResult.DatabaseError;
                }
            

            var reader = ExecuteReader($@"
                SELECT Guilds.Id
                FROM Guilds
                WHERE Guilds.PublicId like '{publicId}'
                ;");
            if (reader is null) {
                guild = null;
                return DatabaseCommandResult.DatabaseError;
            }
            if (reader.Read()) {
                ulong guildId = reader.GetFieldValue<ulong>(0);

                using (SqliteCommand command2 = Connection.CreateCommand()) {
                    command2.CommandText = $@"
                        INSERT INTO GuildAffiliations (GuildId, UserId)
                            VALUES (
                                @GuildId,
                                @OwnerId
                            )
                        ;";
                    try {
                        command2.Parameters.AddWithValue("@GuildId", guildId);
                        command2.Parameters.AddWithValue("@OwnerId", data.OwnerId);
                        command2.ExecuteNonQuery();
                    }
                    catch (Exception ex) {
                        Logger.Error(ex.Message);
                        guild = null;
                        return DatabaseCommandResult.DatabaseError;
                    }
                }

                guild = new Guild(0, publicId, data.Name, data.Password, data.OwnerId, creationTime, data.Icon);
                return DatabaseCommandResult.Success;

                //var reader2 = ExecuteReader($@"
                //    SELECT Guilds.Id
                //    FROM Guilds
                //    WHERE Guilds.PublicId = '{publicId}' AND Guilds.CreationTime = '{creationTime}'
                //    ;");
                //if (reader2 is null) {
                //    guild = null;
                //    return DatabaseCommandResult.DatabaseError;
                //}
                //if (reader2.Read()) {
                //    ulong id = reader2.GetFieldValue<ulong>(0);
                //    guild = new Guild(id, publicId, data.Name, data.Password, data.OwnerId, creationTime, data.Icon);
                //    return DatabaseCommandResult.Success;
                //}
            }
            guild = null;
            return DatabaseCommandResult.Fail;
        }

        public DatabaseCommandResult CreateCategory(CreateCategoryData data, out Category? category) {
            DateTime creationTime = DateTime.Now;
            bool result = ExecuteNonQuery($@"
                INSERT INTO Categories (GuildId, Name)
                    VALUES ('
                        {data.GuildId}', '{data.Name}'
                    )
                ;");
            if (result == false) {
                category = null;
                return DatabaseCommandResult.DatabaseError;
            }
            var reader = ExecuteReader($@"
                SELECT Categories.Id
                FROM Categories
                WHERE Categories.GuildId LIKE '{data.GuildId}' AND Categories.Name LIKE '{data.Name}'
                ;");
            if (reader is null) {
                category = null;
                return DatabaseCommandResult.DatabaseError;
            }
            if (reader.Read()) {
                category = new Category(reader.GetFieldValue<ulong>(0), data.GuildId, data.Name, creationTime);
                return DatabaseCommandResult.Success;
            }
            category = null;
            return DatabaseCommandResult.Fail;
        }

        public DatabaseCommandResult GetUsersAccessingCategory(Category category, out List<ulong>? users) {
            var reader = ExecuteReader($@"
                SELECT GuildAffiliations.UserId
                FROM GuildAffiliations
                    INNER JOIN Guilds on GuildAffiliations.GuildId = Guilds.Id
                    INNER JOIN Categories on Guilds.Id = Categories.GuildId
                WHERE Categories.Id = {category.Id}
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

        public DatabaseCommandResult CreateTextChannel(CreateTextChannelData data) {
            bool result = ExecuteNonQuery($@"
                INSERT INTO TextChannels (CategoryId, Name)
                    VALUES ('
                        {data.CategoryId}', '{data.Name}'
                    )
                ;");
            if (result == false) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }


        public DatabaseCommandResult GetCompleteGuildsInfo(ulong userId, out ObservableCollection<Guild>? collection) {
            try {
                var guildReader = ExecuteReader(@$"
                    SELECT
                        Guilds.Id,
                        Guilds.PublicId,
                        Guilds.Name,
                        Guilds.Password,
                        Guilds.OwnerId,
                        Guilds.CreationTime,
                        Guilds.Icon
                    FROM Guilds INNER JOIN GuildAffiliations ON Guilds.Id = GuildAffiliations.GuildId
                    WHERE GuildAffiliations.UserId = {userId}
                    ;");
                if (guildReader is null) {
                    collection = null;
                    return DatabaseCommandResult.DatabaseError;
                }

                ObservableCollection<Guild> guilds = new();
                while (guildReader.Read()) {
                    ulong guildId = guildReader.GetFieldValue<ulong>(0);
                    string guildPublicId = guildReader.GetFieldValue<string>(1);
                    string guildName = guildReader.GetFieldValue<string>(2);
                    string guildPassword = guildReader.GetFieldValue<string>(3);
                    ulong guildOwnerId = guildReader.GetFieldValue<ulong>(4);
                    DateTime guildCreationTime = guildReader.GetFieldValue<DateTime>(5);
                    byte[] guildIcon = guildReader.GetFieldValue<byte[]>(6);

                    Guild guild = new(guildId, guildPublicId, guildName, guildPassword, guildOwnerId, guildCreationTime, guildIcon);

                    var categoryReader = ExecuteReader($@"
                        SELECT Categories.Id, Categories.GuildId, Categories.Name, Categories.CreationTime
                        FROM Categories
                        WHERE Categories.GuildId = {guildId}
                        ;");
                    if (categoryReader is null) {
                        collection = null;
                        return DatabaseCommandResult.DatabaseError;
                    }

                    ObservableCollection<Category> categories = new();
                    while (categoryReader.Read()) {
                        ulong categoryId = categoryReader.GetFieldValue<ulong>(0);
                        ulong categoryGuildId = categoryReader.GetFieldValue<ulong>(1);
                        string categoryName = categoryReader.GetFieldValue<string>(2);
                        DateTime categoryCreationTime = DateTime.Parse(categoryReader.GetFieldValue<string>(3));

                        Category category = new(categoryId, categoryGuildId, categoryName, categoryCreationTime);

                        var textChannelReader = ExecuteReader($@"
                            SELECT TextChannels.Id, TextChannels.CategoryId, TextChannels.Name
                            FROM TextChannels
                            WHERE TextChannels.CategoryId = {categoryId}
                            ;");
                        if (textChannelReader is null ) {
                            collection = null;
                            return DatabaseCommandResult.DatabaseError;
                        }

                        while (textChannelReader.Read()) {
                            ulong textChannelId = textChannelReader.GetFieldValue<ulong>(0);
                            ulong textChannelCategoryId = textChannelReader.GetFieldValue<ulong>(1);
                            string textChannelName = textChannelReader.GetFieldValue<string>(2);
                            DateTime textChannelCreationTime = DateTime.Parse(textChannelReader.GetFieldValue<string>(3));

                            TextChannel textChannel = new(textChannelId, textChannelCategoryId, textChannelName, categoryCreationTime);
                            category.Channels.Add(textChannel);
                        }

                        categories.Add(category);
                    }



                    guilds.Add(guild);
                }

                collection = guilds;
                return DatabaseCommandResult.Success;
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                collection = null;
                return DatabaseCommandResult.DatabaseError;
            }
        }


        public DatabaseCommandResult SendMessage(MessageData data) {
            DateTime creationTime = DateTime.Now;
            bool result = ExecuteNonQuery($@"
                INSERT INTO Messages (UserId, TextChannelId, Text, Time)
                    VALUES ('
                        {data.UserId}', '{data.TextChannelId}', '{data.Text}', '{creationTime}'
                    )
                ;");
            if (result == false) {
                return DatabaseCommandResult.DatabaseError;
            }
            return DatabaseCommandResult.Success;
        }


        public  DatabaseCommandResult CreateRole(RoleData data) {

        }
    }
}
