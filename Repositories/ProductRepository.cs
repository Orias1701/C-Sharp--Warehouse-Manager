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
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Products", conn))
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
                                    MinThreshold = reader.GetInt32("MinThreshold")
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
                                    MinThreshold = reader.GetInt32("MinThreshold")
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
                        "INSERT INTO Products (ProductName, CategoryID, Price, Quantity, MinThreshold) " +
                        "VALUES (@name, @catId, @price, @qty, @threshold); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.ProductName);
                        cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@qty", product.Quantity);
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
                        "Quantity=@qty, MinThreshold=@threshold WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.ProductName);
                        cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@qty", product.Quantity);
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
        /// Xóa sản phẩm
        /// </summary>
        public bool DeleteProduct(int productId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DELETE FROM Products WHERE ProductID=@id", conn))
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
        /// Cập nhật tồn kho
        /// </summary>
        public bool UpdateQuantity(int productId, int newQuantity)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UPDATE Products SET Quantity=@qty WHERE ProductID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@qty", newQuantity);
                        cmd.Parameters.AddWithValue("@id", productId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật tồn kho: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// </summary>
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Categories ORDER BY CategoryID", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new Category
                                {
                                    CategoryID = reader.GetInt32("CategoryID"),
                                    CategoryName = reader.GetString("CategoryName")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh mục: " + ex.Message);
            }
            return categories;
        }

        /// <summary>
        /// Thêm danh mục mới
        /// </summary>
        public int AddCategory(string categoryName)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO Categories (CategoryName) VALUES (@name); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", categoryName);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        public bool UpdateCategory(int categoryId, string categoryName)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Categories SET CategoryName=@name WHERE CategoryID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", categoryName);
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        public bool DeleteCategory(int categoryId)
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
                throw new Exception("Lỗi khi xóa danh mục: " + ex.Message);
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
    }
}

