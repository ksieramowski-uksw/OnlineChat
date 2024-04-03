using ChatShared.DataModels;
using Microsoft.Data.Sqlite;


namespace ChatServer.Database {
    public class DatabaseConnection {
        private readonly SqliteConnection _connection;
        public DatabaseCommands Commands { get; set; }

        public DatabaseConnection() {
            const string connectionString = "Data Source=./Database.db";
            _connection = new SqliteConnection(connectionString);
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            Commands = new(_connection);
        }

        ~DatabaseConnection() {
            _connection.Close();
        }

        public void Connect() {
            _connection.Open();
            Tables.Initialize(_connection);
        }

    }
}
