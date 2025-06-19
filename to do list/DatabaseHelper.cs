using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace ToDoApp
{
    public class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=todo.db;Version=3;";

        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL
                    )";
                using (var command = new SQLiteCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                
                string createTasksTable = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        Description TEXT NOT NULL,
                        IsCompleted INTEGER NOT NULL,
                        CreatedDate TEXT NOT NULL,
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    )";
                using (var command = new SQLiteCommand(createTasksTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool RegisterUser(string username, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string hashedPassword = HashPassword(password);
                    string query = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public int? AuthenticateUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, PasswordHash FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["PasswordHash"].ToString();
                            int userId = Convert.ToInt32(reader["Id"]);
                            if (VerifyPassword(password, storedHash))
                            {
                                return userId;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            string hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }

        public void AddTask(int userId, string description)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Tasks (UserId, Description, IsCompleted, CreatedDate) VALUES (@UserId, @Description, @IsCompleted, @CreatedDate)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@IsCompleted", 0);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
            }
        }

        public SQLiteDataReader GetTasks(int userId)
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            string query = "SELECT Id, Description, IsCompleted, CreatedDate FROM Tasks WHERE UserId = @UserId";
            var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        public void UpdateTask(int taskId, string description, bool isCompleted)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Tasks SET Description = @Description, IsCompleted = @IsCompleted WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@IsCompleted", isCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@Id", taskId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTask(int taskId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM Tasks WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", taskId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}