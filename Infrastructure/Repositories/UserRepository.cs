using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbHelper _dbHelper;

        public UserRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public User GetUserByUsername(string username)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT UserID, Username, PasswordHash FROM Users WHERE Username = @username", connection);
                command.Parameters.AddWithValue("@username", username);
                
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserID = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2)
                        };
                    }
                }
            }

            return null;
        }

        public void RegisterUser(User user)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@username, @password)", connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.PasswordHash);
                command.ExecuteNonQuery();
            }
        }
    }
}
