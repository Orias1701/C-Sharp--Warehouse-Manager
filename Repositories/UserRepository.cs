using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    public class UserRepository : BaseRepository
    {
        // Helper method để tránh lặp code khi map dữ liệu từ DB sang Object
        private User MapUserFromReader(MySqlDataReader reader)
        {
            return new User
            {
                UserID = reader.GetInt32("UserID"),
                Username = reader.GetString("Username"),
                FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? "" : reader.GetString("FullName"),
                Role = reader.GetString("Role"),
                IsActive = reader.GetBoolean("IsActive"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            };
        }

        public User Login(string username, string passwordRaw)
        {
            try
            {
                string passwordHash = HashSHA256(passwordRaw);
                using (var conn = GetConnection())
                {
                    conn.Open();
                    // Query sử dụng tham số để chống SQL Injection
                    string sql = "SELECT * FROM Users WHERE Username=@username AND Password=@password AND IsActive=1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", passwordHash);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) 
                            {
                                return MapUserFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log lỗi ở đây (ví dụ dùng NLog hoặc Serilog)
                throw new Exception("Lỗi hệ thống khi đăng nhập."); 
            }
            return null;
        }

        private string HashSHA256(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Users WHERE UserID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) return MapUserFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Lỗi khi truy vấn ID người dùng.");
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Users ORDER BY CreatedAt DESC", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(MapUserFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Lỗi khi lấy danh sách người dùng.");
            }
            return users;
        }
    }
}