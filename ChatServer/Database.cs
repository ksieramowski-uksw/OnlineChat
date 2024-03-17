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
        }

        public void InitTables() {
            using var createUserTable = _connection.CreateCommand();
            createUserTable.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    ID INTEGER AUTO_INCREMENT PRIMARY KEY NOT NULL,
                    Email TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    ConfirmPassword TEXT NOT NULL,
                    Nickname TEXT NOT NULL
                );";
            createUserTable.ExecuteNonQuery();
        }

        public bool CanRegisterNewUser(RegisterData registerData) {
            using var command = _connection.CreateCommand();
            command.CommandText = @$"
                SELECT ID
                FROM Users
                WHERE Users.Email LIKE '{registerData.Email}'
                    OR Users.Nickname LIKE '{registerData.Nickname}'
                ;";
            var reader = command.ExecuteReader();
            return !reader.HasRows;
        }

        public bool TryRegisterNewUser(RegisterData data) {
            bool canRegister = CanRegisterNewUser(data);
            if (!canRegister) {
                Logger.Error("REGSITER - CAN'T REGISTER");
                return false;
            }
            using var command = _connection.CreateCommand();
            command.CommandText = @$"
                INSERT INTO Users (
                    Email, Password, ConfirmPassword, Nickname
                )
                VALUES (
                    '{data.Email}', '{data.Password}',
                    '{data.ConfirmPassword}', '{data.Nickname}'
                );";
            command.ExecuteNonQuery();
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
            var reader = command.ExecuteReader();
            if (!reader.HasRows) { return null; }

            reader.Read();
            return new ClientData(ulong.Parse(reader.GetString(0)), reader.GetString(1));
        }

    }
}
