using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    /// <summary>
    /// Repository để quản lý phiếu Nhập/Xuất kho
    /// </summary>
    public class StockTransactionRepository : BaseRepository
    {
        private readonly TransactionDetailRepository _detailRepo;

        public StockTransactionRepository()
        {
            _detailRepo = new TransactionDetailRepository();
        }

        /// <summary>
        /// Lấy danh sách tất cả phiếu
        /// </summary>
        public List<StockTransaction> GetAllTransactions()
        {
            var transactions = new List<StockTransaction>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM StockTransactions ORDER BY DateCreated DESC", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactions.Add(new StockTransaction
                                {
                                    TransactionID = reader.GetInt32("TransactionID"),
                                    Type = reader.GetString("Type"),
                                    DateCreated = reader.GetDateTime("DateCreated"),
                                    CreatedByUserID = reader.IsDBNull(reader.GetOrdinal("CreatedByUserID")) ? 0 : reader.GetInt32("CreatedByUserID"),
                                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString("Note")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách phiếu: " + ex.Message);
            }
            return transactions;
        }

        /// <summary>
        /// Lấy phiếu theo ID (bao gồm chi tiết)
        /// </summary>
        public StockTransaction GetTransactionById(int transactionId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    
                    var transaction = new StockTransaction { TransactionID = transactionId };
                    
                    // Lấy thông tin giao dịch
                    using (var cmd = new MySqlCommand("SELECT * FROM StockTransactions WHERE TransactionID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", transactionId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                transaction.TransactionID = reader.GetInt32("TransactionID");
                                transaction.Type = reader.GetString("Type");
                                transaction.DateCreated = reader.GetDateTime("DateCreated");
                                transaction.CreatedByUserID = reader.IsDBNull(reader.GetOrdinal("CreatedByUserID")) ? 0 : reader.GetInt32("CreatedByUserID");
                                transaction.Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString("Note");
                            }
                        }
                    }

                    // Lấy chi tiết giao dịch
                    transaction.Details = _detailRepo.GetDetailsByTransactionId(transactionId);
                    
                    return transaction;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy phiếu ID {transactionId}: " + ex.Message);
            }
        }

        /// <summary>
        /// Tạo phiếu nhập/xuất mới
        /// </summary>
        public int CreateTransaction(StockTransaction transaction)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO StockTransactions (Type, DateCreated, CreatedByUserID, Note) " +
                        "VALUES (@type, @date, @userId, @note); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", transaction.Type);
                        cmd.Parameters.AddWithValue("@date", transaction.DateCreated);
                        cmd.Parameters.AddWithValue("@userId", transaction.CreatedByUserID);
                        cmd.Parameters.AddWithValue("@note", transaction.Note ?? "");
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin phiếu (không cập nhật chi tiết)
        /// </summary>
        public bool UpdateTransaction(StockTransaction transaction)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE StockTransactions SET Type=@type, Note=@note WHERE TransactionID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", transaction.Type);
                        cmd.Parameters.AddWithValue("@note", transaction.Note ?? "");
                        cmd.Parameters.AddWithValue("@id", transaction.TransactionID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa phiếu (CASCADE xóa chi tiết tự động)
        /// </summary>
        public bool DeleteTransaction(int transactionId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DELETE FROM StockTransactions WHERE TransactionID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", transactionId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách phiếu theo loại (Import/Export)
        /// </summary>
        public List<StockTransaction> GetTransactionsByType(string type)
        {
            var transactions = new List<StockTransaction>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT * FROM StockTransactions WHERE Type=@type ORDER BY DateCreated DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", type);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactions.Add(new StockTransaction
                                {
                                    TransactionID = reader.GetInt32("TransactionID"),
                                    Type = reader.GetString("Type"),
                                    DateCreated = reader.GetDateTime("DateCreated"),
                                    CreatedByUserID = reader.IsDBNull(reader.GetOrdinal("CreatedByUserID")) ? 0 : reader.GetInt32("CreatedByUserID"),
                                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString("Note")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy phiếu theo loại: " + ex.Message);
            }
            return transactions;
        }

        /// <summary>
        /// Lấy danh sách phiếu trong một khoảng thời gian
        /// </summary>
        public List<StockTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            var transactions = new List<StockTransaction>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT * FROM StockTransactions WHERE DateCreated BETWEEN @start AND @end ORDER BY DateCreated DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@start", startDate);
                        cmd.Parameters.AddWithValue("@end", endDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactions.Add(new StockTransaction
                                {
                                    TransactionID = reader.GetInt32("TransactionID"),
                                    Type = reader.GetString("Type"),
                                    DateCreated = reader.GetDateTime("DateCreated"),
                                    CreatedByUserID = reader.IsDBNull(reader.GetOrdinal("CreatedByUserID")) ? 0 : reader.GetInt32("CreatedByUserID"),
                                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString("Note")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy phiếu theo ngày: " + ex.Message);
            }
            return transactions;
        }
    }
}