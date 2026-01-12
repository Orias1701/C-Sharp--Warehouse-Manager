using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    /// <summary>
    /// Repository để quản lý chi tiết phiếu Nhập/Xuất kho
    /// </summary>
    public class TransactionDetailRepository : BaseRepository
    {
        /// <summary>
        /// Lấy danh sách chi tiết theo Transaction ID
        /// </summary>
        public List<TransactionDetail> GetDetailsByTransactionId(int transactionId)
        {
            var details = new List<TransactionDetail>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT * FROM TransactionDetails WHERE TransactionID=@transId", conn))
                    {
                        cmd.Parameters.AddWithValue("@transId", transactionId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                details.Add(new TransactionDetail
                                {
                                    DetailID = reader.GetInt32("DetailID"),
                                    TransactionID = reader.GetInt32("TransactionID"),
                                    ProductID = reader.GetInt32("ProductID"),
                                    ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? "" : reader.GetString("ProductName"),
                                    Quantity = reader.GetInt32("Quantity"),
                                    UnitPrice = reader.GetDecimal("UnitPrice")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy chi tiết phiếu: " + ex.Message);
            }
            return details;
        }

        /// <summary>
        /// Lấy chi tiết theo ID
        /// </summary>
        public TransactionDetail GetDetailById(int detailId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT * FROM TransactionDetails WHERE DetailID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", detailId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TransactionDetail
                                {
                                    DetailID = reader.GetInt32("DetailID"),
                                    TransactionID = reader.GetInt32("TransactionID"),
                                    ProductID = reader.GetInt32("ProductID"),
                                    ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? "" : reader.GetString("ProductName"),
                                    Quantity = reader.GetInt32("Quantity"),
                                    UnitPrice = reader.GetDecimal("UnitPrice")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết ID {detailId}: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Thêm chi tiết vào phiếu
        /// </summary>
        public int AddTransactionDetail(TransactionDetail detail)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice) " +
                        "VALUES (@transId, @prodId, @prodName, @qty, @price); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd.Parameters.AddWithValue("@transId", detail.TransactionID);
                        cmd.Parameters.AddWithValue("@prodId", detail.ProductID);
                        cmd.Parameters.AddWithValue("@prodName", detail.ProductName ?? "");
                        cmd.Parameters.AddWithValue("@qty", detail.Quantity);
                        cmd.Parameters.AddWithValue("@price", detail.UnitPrice);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật chi tiết phiếu
        /// </summary>
        public bool UpdateTransactionDetail(TransactionDetail detail)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE TransactionDetails SET ProductID=@prodId, ProductName=@prodName, Quantity=@qty, UnitPrice=@price " +
                        "WHERE DetailID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@prodId", detail.ProductID);
                        cmd.Parameters.AddWithValue("@prodName", detail.ProductName ?? "");
                        cmd.Parameters.AddWithValue("@qty", detail.Quantity);
                        cmd.Parameters.AddWithValue("@price", detail.UnitPrice);
                        cmd.Parameters.AddWithValue("@id", detail.DetailID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa chi tiết phiếu
        /// </summary>
        public bool DeleteTransactionDetail(int detailId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "DELETE FROM TransactionDetails WHERE DetailID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", detailId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa tất cả chi tiết của một phiếu
        /// </summary>
        public bool DeleteAllDetails(int transactionId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "DELETE FROM TransactionDetails WHERE TransactionID=@transId", conn))
                    {
                        cmd.Parameters.AddWithValue("@transId", transactionId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy tổng số sản phẩm trong chi tiết phiếu
        /// </summary>
        public int GetTotalQuantity(int transactionId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT SUM(Quantity) FROM TransactionDetails WHERE TransactionID=@transId", conn))
                    {
                        cmd.Parameters.AddWithValue("@transId", transactionId);
                        var result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tính tổng số lượng: " + ex.Message);
            }
        }
    }
}
