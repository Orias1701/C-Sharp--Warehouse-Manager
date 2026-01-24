using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    /// <summary>
    /// Repository để quản lý sản phẩm (CRUD + cập nhật tồn kho)
    /// </summary>
    public class ProductRepository : BaseRepository
    {
        /// <summary>
        /// Lấy danh sách tất cả sản phẩm (chỉ những sản phẩm Visible=true, trừ khi includeHidden=true)
        /// </summary>
        public List<Product> GetAllProducts(bool includeHidden = false)
        {
            var products = new List<Product>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string query = includeHidden
                        ? "SELECT * FROM Products ORDER BY ProductID DESC"
                        : "SELECT * FROM Products WHERE Visible=TRUE ORDER BY ProductID DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    ProductID = reader.GetInt32("ProductID"),
                                    ProductName = reader.GetString("ProductName"),
                                    CategoryID = reader.GetInt32("CategoryID"),
                                    Price = reader.GetDecimal("Price"),
                                    Quantity = reader.GetInt32("Quantity"),
                                    MinThreshold = reader.GetInt32("MinThreshold"),
                                    InventoryValue = reader.GetDecimal("InventoryValue")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sản phẩm: " + ex.Message);
            }
            return products;
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên
        /// </summary>
        public List<Product> SearchProductByName(string keyword)
        {
            var list = new List<Product>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Products WHERE ProductName LIKE @keyword AND Visible=TRUE", conn))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Product
                                {
                                    ProductID = reader.GetInt32("ProductID"),
                                    ProductName = reader.GetString("ProductName"),
                                    CategoryID = reader.GetInt32("CategoryID"),
                                    Price = reader.GetDecimal("Price"),
                                    Quantity = reader.GetInt32("Quantity"),
                                    MinThreshold = reader.GetInt32("MinThreshold"),
                                    InventoryValue = reader.GetDecimal("InventoryValue")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm sản phẩm: " + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        public Product GetProductById(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Products WHERE ProductID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Product
                                {
                                    ProductID = reader.GetInt32("ProductID"),
                                    ProductName = reader.GetString("ProductName"),
                                    CategoryID = reader.GetInt32("CategoryID"),
                                    Price = reader.GetDecimal("Price"),
                                    Quantity = reader.GetInt32("Quantity"),
                                    MinThreshold = reader.GetInt32("MinThreshold"),
                                    InventoryValue = reader.GetDecimal("InventoryValue")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sản phẩm ID {productId}: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Thêm sản phẩm mới
        /// </summary>
        public int AddProduct(Product product)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO Products (ProductName, CategoryID, Price, Quantity, InventoryValue, MinThreshold) " +
                        "VALUES (@name, @catId, @price, @qty, @invValue, @threshold); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.ProductName);
                        cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@qty", product.Quantity);
                        cmd.Parameters.AddWithValue("@invValue", product.Quantity * product.Price);
                        cmd.Parameters.AddWithValue("@threshold", product.MinThreshold);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        public bool UpdateProduct(Product product)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Products SET ProductName=@name, CategoryID=@catId, Price=@price, " +
                        "Quantity=@qty, InventoryValue=@invValue, MinThreshold=@threshold WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.ProductName);
                        cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@qty", product.Quantity);
                        cmd.Parameters.AddWithValue("@invValue", product.Quantity * product.Price);
                        cmd.Parameters.AddWithValue("@threshold", product.MinThreshold);
                        cmd.Parameters.AddWithValue("@id", product.ProductID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa sản phẩm - kiểm tra phụ thuộc khóa và xóa mềm hoặc vật lý
        /// </summary>
        public bool DeleteProduct(int productId)
        {
            try
            {
                // Kiểm tra phụ thuộc khóa
                if (HasForeignKeyReferences(productId))
                {
                    // Có phụ thuộc - xóa mềm (soft delete)
                    return SoftDeleteProduct(productId);
                }
                else
                {
                    // Không có phụ thuộc - xóa vật lý (hard delete)
                    return HardDeleteProduct(productId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật tồn kho
        /// </summary>
        public bool UpdateQuantity(int productId, int newQuantity)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    // First get the price to calculate InventoryValue
                    using (var cmd = new MySqlCommand("SELECT Price FROM Products WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        var price = cmd.ExecuteScalar();
                        if (price == null || price == DBNull.Value)
                            throw new Exception($"Sản phẩm ID {productId} không tồn tại");

                        decimal productPrice = Convert.ToDecimal(price);
                        decimal inventoryValue = newQuantity * productPrice;

                        using (var updateCmd = new MySqlCommand("UPDATE Products SET Quantity=@qty, InventoryValue=@invValue WHERE ProductID=@id", conn))
                        {
                            updateCmd.Parameters.AddWithValue("@qty", newQuantity);
                            updateCmd.Parameters.AddWithValue("@invValue", inventoryValue);
                            updateCmd.Parameters.AddWithValue("@id", productId);
                            return updateCmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật tồn kho: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra xem sản phẩm với ID đã tồn tại hay chưa
        /// </summary>
        public bool ProductIdExists(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM Products WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi kiểm tra tồn tại sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra sản phẩm có được tham chiếu bởi TransactionDetails hoặc các bảng khác hay không
        /// </summary>
        public bool HasForeignKeyReferences(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    // Kiểm tra TransactionDetails
                    using (var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM TransactionDetails WHERE ProductID=@id AND Visible=TRUE", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kiểm tra phụ thuộc khóa ngoài: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa mềm (soft delete) - đặt Visible = false
        /// </summary>
        public bool SoftDeleteProduct(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Products SET Visible=FALSE WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa mềm sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa vật lý (hard delete) - xóa toàn bộ bản ghi
        /// </summary>
        public bool HardDeleteProduct(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "DELETE FROM Products WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra danh mục có sản phẩm hay không
        /// </summary>
        public bool CategoryHasProducts(int categoryId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM Products WHERE CategoryID=@id AND Visible=TRUE", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kiểm tra danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa mềm danh mục (Visible = false)
        /// </summary>
        public bool SoftDeleteCategory(int categoryId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Categories SET Visible=FALSE WHERE CategoryID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa mềm danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa vật lý danh mục
        /// </summary>
        public bool HardDeleteCategory(int categoryId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "DELETE FROM Categories WHERE CategoryID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa vật lý danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Đảo ngược trạng thái ẩn hiện của sản phẩm (Visible: 1 -> 0, 0 -> 1)
        /// </summary>
        public bool HideProduct(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UPDATE Products SET Visible = NOT Visible WHERE ProductID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đảo trạng thái sản phẩm: " + ex.Message);
            }
        }
        /// <summary>
        /// Phục hồi sản phẩm đã xóa (restore deleted product)
        /// </summary>
        public bool RestoreDeletedProduct(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Products SET Visible=TRUE WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi phục hồi sản phẩm: " + ex.Message);
            }
        }
    }
}