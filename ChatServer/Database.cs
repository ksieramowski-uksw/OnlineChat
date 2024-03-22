using ChatShared.DataModels;
using Microsoft.Data.Sqlite;
using System.Text;


namespace ChatServer {
    public class Database {
        private readonly SqliteConnection _connection;

        public Database() {
            const string connectionString = "Data Source=Database.db";
            _connection = new SqliteConnection(connectionString);
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
        }

        ~Database() {
            _connection.Close();
        }

        public void Connect() {
            _connection.Open();
            InitTables();
        }

        public bool InitTables() {
            Logger.Info("Database: Initialization started.");
            int checkSum = 0;

            const string userTableName = "Users";
            using var createUserTable = _connection.CreateCommand();
            createUserTable.CommandText = $@"
                CREATE TABLE IF NOT EXISTS {userTableName} (
                    ID INTEGER PRIMARY KEY,
                    Email TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    ConfirmPassword TEXT NOT NULL,
                    Nickname TEXT NOT NULL
                );";
            try {
                createUserTable.ExecuteNonQuery();
                //if (!VerifyTableCreation(userTableName)) {
                //    Logger.Error($"Failed to initialize table: \"{userTableName}\"");
                //    checkSum += 1;
                //}
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                Logger.Error($"Failed to initialize table: \"{userTableName}\"");
                checkSum += 1;
            }

            if (checkSum == 0) {
                Logger.Info("Database: Initialization successful.");
                return true;
            }
            return false;
        }

        //public bool VerifyTableCreation(string tableName) {
        //    using var verifyCommand = _connection.CreateCommand();
        //    verifyCommand.CommandText = $@"
        //        SELECT name
        //        FROM sqlite_master
        //        WHERE type='table' AND name='{tableName}'
        //        ";
        //    try {
        //        var reader = verifyCommand.ExecuteReader();
        //        return reader.HasRows;
        //    }
        //    catch (Exception ex) {
        //        Logger.Error(ex.Message);
        //    }
        //    return false;
        //}

        public bool CanRegisterNewUser(RegisterData data) {
            using var command = _connection.CreateCommand();
            command.CommandText = @$"
                SELECT ID
                FROM Users
                WHERE Users.Email LIKE '{data.Email}' OR Users.Nickname LIKE '{data.Nickname}'
                ;";
            try {
                var reader = command.ExecuteReader();
                if (reader.HasRows) {
                    Logger.Warning($"Register: user with email \"{data.Email}\" already exists.");
                }
                return !reader.HasRows;
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
            }
            return false;
        }

        public bool TryRegisterNewUser(RegisterData data) {
            if (!CanRegisterNewUser(data)) { return false; }
            Logger.Info($"Registering new user with email \"{data.Email}\"...");
            using var command = _connection.CreateCommand();
            command.CommandText = @$"
                INSERT INTO Users (Email, Password, ConfirmPassword, Nickname)
                VALUES (
                    '{data.Email}', '{data.Password}',
                    '{data.ConfirmPassword}', '{data.Nickname}'
                );";
            try {
                command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                return false;
            }
            Logger.Info($"Registered new user with email \"{data.Email}\".");
            return true;
        }

        public ClientData? TryLogIn(LoginData data) {
            using var command = _connection.CreateCommand();
            command.CommandText = @$"
                SELECT Id, Nickname
                FROM Users
                WHERE Users.Email LIKE '{data.Email}'
                    AND Users.Password LIKE '{data.Password}'
                ;";
            try {
                var reader = command.ExecuteReader();
                if (!reader.HasRows) {
                    Logger.Warning($"No matching data found for loggin in with email \"{data.Email}\"");
                    return null;
                }
                if (reader.Read()) {
                    Logger.Info($"Found matching login data for email \"{data.Email}\"");
                    return new ClientData(ulong.Parse(reader.GetString(0)), reader.GetString(1));
                }
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
            }
            return null;
        }

    }
}
