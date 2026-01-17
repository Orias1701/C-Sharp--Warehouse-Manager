using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;
using WarehouseManagement.Helpers;

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
                Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? "" : reader.GetString("Password"),
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
                
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwordRaw))
                {
                    return null;
                }

                using (var conn = GetConnection())
                {
                    conn.Open();
                    // Query sử dụng tham số để chống SQL Injection
                    string sql = "SELECT * FROM Users WHERE Username=@username AND IsActive=1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) 
                            {
                                var user = MapUserFromReader(reader);
                                
                                // Xác minh mật khẩu bằng IdGenerator
                                if (user.VerifyPassword(passwordRaw))
                                {
                                    return user;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw new Exception("Lỗi hệ thống khi đăng nhập."); 
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