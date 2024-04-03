using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Database {
    public static class Tables {


        public static int Initialize(SqliteConnection connection) {
            int checkSum = 0;

            if (!InitializeUserTable(connection)) { checkSum += 1; }
            if (!InitializeFriendsTable(connection)) { checkSum += 1; }
            if (!InitializeGuildTable(connection)) { checkSum += 1; }
            if (!InitializeGuildAffiliationTable(connection)) { checkSum += 1; }
            if (!InitializeCategoriesTable(connection)) { checkSum += 1; }
            if (!InitializeChannelTable(connection)) { checkSum += 1; }
            if (!InitializeMessageTable(connection)) { checkSum += 1; }

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
                    Id INTEGER PRIMARY KEY,
                    Email TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    Nickname TEXT NOT NULL,
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
                    Id INTEGER PRIMARY KEY,
                    UserId INTEGER NOT NULL,
                    FriendId INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeGuildTable(SqliteConnection connection) {
            const string tableName = "Guilds";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    Id INTEGER PRIMARY KEY,
                    PublicId TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    OwnerId INTEGER NOT NULL,
                    CreationTime TEXT NOT NULL,
                    Icon BLOB NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeGuildAffiliationTable(SqliteConnection connection) {
            const string tableName = "GuildAffiliations";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    Id INTEGER PRIMARY KEY,
                    GuildId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeCategoriesTable(SqliteConnection connection) {
            const string tableName = "Categories";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    Id INTEGER PRIMARY KEY,
                    GuildId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    CreationTime TEXT NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeChannelTable(SqliteConnection connection) {
            const string tableName = "TextChannels";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    Id INTEGER PRIMARY KEY,
                    CategoryId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    CreationTime TEXT NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }

        public static bool InitializeMessageTable(SqliteConnection connection) {
            const string tableName = "Messages";
            const string query = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    Id INTEGER PRIMARY KEY,
                    UserId INTEGER NOT NULL,
                    TextChannelId INTEGER NOT NULL,
                    Text TEXT NOT NULL,
                    Time TEXT NOT NULL
                );";
            return InitializeTable(connection, tableName, query);
        }
    }
}
