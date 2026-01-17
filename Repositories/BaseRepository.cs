using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace WarehouseManagement.Repositories
{
    /// <summary>
    /// Lớp cơ sở cho tất cả repositories - khởi tạo kết nối MySQL
    /// </summary>
    public abstract class BaseRepository
    {
        protected string _connectionString;

        public BaseRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDB"]?.ConnectionString
                ?? throw new ConfigurationErrorsException("Connection string 'WarehouseDB' not found in App.config");
        }

        /// <summary>
        /// Lấy kết nối mới đến database
        /// </summary>
        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Kiểm tra kết nối tới database
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}