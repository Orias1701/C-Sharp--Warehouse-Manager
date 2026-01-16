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
    public class TransactionRepository : BaseRepository
    {
        /// <summary>
        /// Lấy danh sách tất cả phiếu (bao gồm chi tiết)
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
                    
                    // Load chi tiết cho mỗi phiếu
                    foreach (var trans in transactions)
                    {
                        using (var detailCmd = new MySqlCommand(
                            "SELECT * FROM TransactionDetails WHERE TransactionID=@transId", conn))
                        {
                            detailCmd.Parameters.AddWithValue("@transId", trans.TransactionID);
                            using (var detailReader = detailCmd.ExecuteReader())
                            {
                                while (detailReader.Read())
                                {
                                    trans.Details.Add(new TransactionDetail
                                    {
                                        DetailID = detailReader.GetInt32("DetailID"),
                                        TransactionID = detailReader.GetInt32("TransactionID"),
                                        ProductID = detailReader.GetInt32("ProductID"),
                                        ProductName = detailReader.IsDBNull(detailReader.GetOrdinal("ProductName")) ? "" : detailReader.GetString("ProductName"),
                                        Quantity = detailReader.GetInt32("Quantity"),
                                        UnitPrice = detailReader.GetDecimal("UnitPrice")
                                    });
                                }
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

                    // Lấy chi tiết giao dịch - reader cũ đã đóng
                    using (var detailCmd = new MySqlCommand(
                        "SELECT * FROM TransactionDetails WHERE TransactionID=@transId", conn))
                    {
                        detailCmd.Parameters.AddWithValue("@transId", transactionId);
                        using (var detailReader = detailCmd.ExecuteReader())
                        {
                            int detailCount = 0;
                            while (detailReader.Read())
                            {
                                transaction.Details.Add(new TransactionDetail
                                {
                                    DetailID = detailReader.GetInt32("DetailID"),
                                    TransactionID = detailReader.GetInt32("TransactionID"),
                                    ProductID = detailReader.GetInt32("ProductID"),
                                    ProductName = detailReader.IsDBNull(detailReader.GetOrdinal("ProductName")) ? "" : detailReader.GetString("ProductName"),
                                    Quantity = detailReader.GetInt32("Quantity"),
                                    UnitPrice = detailReader.GetDecimal("UnitPrice")
                                });
                                detailCount++;
                            }
                        }
                    }
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
        /// Thêm chi tiết vào phiếu
        /// </summary>
        public bool AddTransactionDetail(TransactionDetail detail)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice) " +
                        "VALUES (@transId, @prodId, @prodName, @qty, @price)", conn))
                    {
                        cmd.Parameters.AddWithValue("@transId", detail.TransactionID);
                        cmd.Parameters.AddWithValue("@prodId", detail.ProductID);
                        cmd.Parameters.AddWithValue("@prodName", detail.ProductName ?? "");
                        cmd.Parameters.AddWithValue("@qty", detail.Quantity);
                        cmd.Parameters.AddWithValue("@price", detail.UnitPrice);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chi tiết phiếu: " + ex.Message);
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
    }
}

