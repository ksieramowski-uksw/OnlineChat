using Microsoft.Data.Sqlite;


namespace ChatServer.Database {
    public static class Tables {


        public static int Initialize(SqliteConnection connection) {
            int checkSum = 0;

            if (!InitializeUserTable(connection)) { checkSum += 1; }
            if (!InitializeFriendsTable(connection)) { checkSum += 1; }
            if (!InitializeGuildTable(connection)) { checkSum += 1; }
            if (!InitializeGuildAffiliationTable(connection)) { checkSum += 1; }
            if (!InitializeCategoriesTable(connection)) { checkSum += 1; }
            if (!InitializeTextChannelTable(connection)) { checkSum += 1; }
            if (!InitializeMessageTable(connection)) { checkSum += 1; }
            if (!InitializeMessageAttachmentTable(connection)) { checkSum += 1; }
            if (!InitializeTextChannelPrivilegeTable(connection)) { checkSum += 1; }
            if (!InitializeCategoryPrivilegeTable(connection)) { checkSum += 1; }
            if (!InitializeGuildPrivilegeTable(connection)) { checkSum += 1; }
            if (!InitializeDefaultTextChannelPrivilegeTable(connection)) { checkSum += 1; }
            if (!InitializeDefaultCategoryPrivilegeTable(connection)) { checkSum += 1; }
            if (!InitializeDefaultGuildPrivilegeTable(connection)) { checkSum += 1; }

            return checkSum;
        }

        private static bool InitializeTable(SqliteConnection connection, string tableName, string query) {
            using var command = connection.CreateCommand();
            command.CommandText = query;
            try {
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                Logger.Error($"Failed to initialize table: \"{tableName}\"");
                return false;
            }
            return true;
        }

        public static bool InitializeUserTable(SqliteConnection connection) {
            const string tableName = "Users";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    PublicID TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    Nickname TEXT NOT NULL,
                    Pronoun TEXT NOT NULL,
                    CreationTime TEXT NOT NULL,
                    ProfilePicture BLOB NOT NULL,
                    Status INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeFriendsTable(SqliteConnection connection) {
            const string tableName = "Friends";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    UserID INTEGER NOT NULL,
                    FriendID INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeGuildTable(SqliteConnection connection) {
            const string tableName = "Guilds";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    PublicID TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    OwnerID INTEGER NOT NULL,
                    CreationTime TEXT NOT NULL,
                    Icon BLOB NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeGuildAffiliationTable(SqliteConnection connection) {
            const string tableName = "GuildAffiliations";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    GuildID INTEGER NOT NULL,
                    UserID INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeCategoriesTable(SqliteConnection connection) {
            const string tableName = "Categories";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    GuildID INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    CreationTime TEXT NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeTextChannelTable(SqliteConnection connection) {
            const string tableName = "TextChannels";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    CategoryID INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    CreationTime TEXT NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeMessageTable(SqliteConnection connection) {
            const string tableName = "Messages";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    UserID INTEGER NOT NULL,
                    TextChannelID INTEGER NOT NULL,
                    Content TEXT NOT NULL,
                    CreationTime TEXT NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeMessageAttachmentTable(SqliteConnection connection) {
            const string tableName = "MessageAttachments";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    MessageID INTEGER NOT NULL,
                    Content BLOB NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeTextChannelPrivilegeTable(SqliteConnection connection) {
            const string tableName = "TextChannelPrivileges";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    UserID INTEGER NOT NULL,
                    ChannelID INTEGER,
                    ViewChannel INTEGER NOT NULL,
                    UpdateChannel INTEGER NOT NULL,
                    DeleteChannel INTEGER NOT NULL,
                    Read INTEGER NOT NULL,
                    Write INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeCategoryPrivilegeTable(SqliteConnection connection) {
            const string tableName = "CategoryPrivileges";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    UserID INTEGER NOT NULL,
                    CategoryID INTEGER,
                    ViewCategory INTEGER NOT NULL,
                    UpdateCategory INTEGER NOT NULL,
                    DeleteCategory INTEGER NOT NULL,
                    CreateChannel INTERER NOT NULL,
                    UpdateChannel INTEGER NOT NULL,
                    DeleteChannel INTEGER NOT NULL,
                    Read INTEGER NOT NULL,
                    Write INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeGuildPrivilegeTable(SqliteConnection connection) {
            const string tableName = "GuildPrivileges";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    UserID INTEGER NOT NULL,
                    GuildID INTEGER,
                    ManageGuild INTEGER NOT NULL,
                    ManagePrivileges INTEGER NOT NULL,
                    CreateCategory INTEGER NOT NULL,
                    UpdateCategory INTEGER NOT NULL,
                    DeleteCategory INTEGER NOT NULL,
                    CreateChannel INTERER NOT NULL,
                    UpdateChannel INTEGER NOT NULL,
                    DeleteChannel INTEGER NOT NULL,
                    Read INTEGER NOT NULL,
                    Write INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }


        public static bool InitializeDefaultTextChannelPrivilegeTable(SqliteConnection connection) {
            const string tableName = "DefaultTextChannelPrivileges";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    ChannelID INTEGER,
                    ViewChannel INTEGER NOT NULL,
                    UpdateChannel INTEGER NOT NULL,
                    DeleteChannel INTEGER NOT NULL,
                    Read INTEGER NOT NULL,
                    Write INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeDefaultCategoryPrivilegeTable(SqliteConnection connection) {
            const string tableName = "DefaultCategoryPrivileges";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    CategoryID INTEGER,
                    ViewCategory INTEGER NOT NULL,
                    UpdateCategory INTEGER NOT NULL,
                    DeleteCategory INTEGER NOT NULL,
                    CreateChannel INTERER NOT NULL,
                    UpdateChannel INTEGER NOT NULL,
                    DeleteChannel INTEGER NOT NULL,
                    Read INTEGER NOT NULL,
                    Write INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeDefaultGuildPrivilegeTable(SqliteConnection connection) {
            const string tableName = "DefaultGuildPrivileges";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    ID INTEGER PRIMARY KEY,
                    GuildID INTEGER,
                    ManageGuild INTEGER NOT NULL,
                    ManagePrivileges INTEGER NOT NULL,
                    CreateCategory INTEGER NOT NULL,
                    UpdateCategory INTEGER NOT NULL,
                    DeleteCategory INTEGER NOT NULL,
                    CreateChannel INTERER NOT NULL,
                    UpdateChannel INTEGER NOT NULL,
                    DeleteChannel INTEGER NOT NULL,
                    Read INTEGER NOT NULL,
                    Write INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

    }
}
