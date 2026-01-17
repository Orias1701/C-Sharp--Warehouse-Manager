using System;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Helpers
{
    /// <summary>
    /// Helper kiểm tra kết nối database và backup
    /// </summary>
    public class DatabaseHelper
    {
        public DatabaseHelper()
        {
            // BaseRepository là abstract, không cần khởi tạo
        }

        /// <summary>
        /// Kiểm tra kết nối đến database
        /// </summary>
        public static bool TestDatabaseConnection()
        {
            try
            {
                var repo = new ProductRepository();
                return repo.TestConnection();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy thông báo lỗi kết nối
        /// </summary>
        public static string GetConnectionError()
        {
            try
            {
                if (!TestDatabaseConnection())
                    return "Không thể kết nối tới database. Vui lòng kiểm tra App.config";
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }

        /// <summary>
        /// Tự động chạy schema.sql để tạo/reset database
        /// </summary>
        public static bool ExecuteSchema()
        {
            try
            {
                string schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "SQL", "schema.sql");
                
                if (!File.Exists(schemaPath))
                    throw new FileNotFoundException("Không tìm thấy file schema.sql");

                string sqlScript = File.ReadAllText(schemaPath, Encoding.UTF8);
                
                // Get connection string without specifying database
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["WarehouseDB"].ConnectionString;
                string connWithoutDb = connectionString.Replace("Database=QL_KhoHang;", "").Replace("QL_KhoHang;", "");
                
                using (var conn = new MySqlConnection(connWithoutDb))
                {
                    conn.Open();
                    
                    // Split script by semicolon and execute each command
                    string[] commands = sqlScript.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (string command in commands)
                    {
                        if (string.IsNullOrWhiteSpace(command))
                            continue;
                            
                        using (var cmd = new MySqlCommand(command.Trim(), conn))
                        {
                            cmd.CommandTimeout = 30;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi chạy schema: " + ex.Message);
            }
        }

        /// <summary>
        /// Tạo file backup đơn giản (chỉ lưu thông tin)
        /// </summary>
        public static bool CreateBackup(string backupPath)
        {
            try
            {
                if (!Directory.Exists(backupPath))
                    Directory.CreateDirectory(backupPath);

                string filename = Path.Combine(backupPath, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine($"Backup created at: {DateTime.Now}");
                    writer.WriteLine("Database: QL_KhoHang");
                    writer.WriteLine("Status: OK");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo backup: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy kích thước database (mô phỏng)
        /// </summary>
        public static string GetDatabaseSize()
        {
            try
            {
                return "Chưa cập nhật - cần triển khai MySQL INFORMATION_SCHEMA";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }
    }
}