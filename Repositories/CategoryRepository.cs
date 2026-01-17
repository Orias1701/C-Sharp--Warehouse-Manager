using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    /// <summary>
    /// Repository để quản lý danh mục sản phẩm
    /// </summary>
    public class CategoryRepository : BaseRepository
    {
        /// <summary>
        /// Lấy danh sách tất cả danh mục (chỉ visible records)
        /// </summary>
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Categories WHERE Visible=TRUE ORDER BY CategoryID DESC", conn))
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
                throw new Exception("Lỗi khi lấy danh sách danh mục: " + ex.Message);
            }
            return categories;
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        public Category GetCategoryById(int categoryId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Categories WHERE CategoryID = @id AND Visible=TRUE", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Category
                                {
                                    CategoryID = reader.GetInt32("CategoryID"),
                                    CategoryName = reader.GetString("CategoryName")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh mục ID {categoryId}: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Thêm danh mục mới
        /// </summary>
        public int AddCategory(Category category)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO Categories (CategoryName, Visible) " +
                        "VALUES (@name, TRUE); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", category.CategoryName);
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
        public bool UpdateCategory(Category category)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Categories SET CategoryName=@name WHERE CategoryID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", category.CategoryName);
                        cmd.Parameters.AddWithValue("@id", category.CategoryID);
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
        /// Xóa danh mục (soft delete)
        /// </summary>
        public bool DeleteCategory(int categoryId)
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
                throw new Exception("Lỗi khi xóa danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Phục hồi danh mục đã xóa (restore deleted category)
        /// </summary>
        public bool RestoreDeletedCategory(int categoryId, string categoryName)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Categories SET Visible=TRUE, CategoryName=@name WHERE CategoryID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        cmd.Parameters.AddWithValue("@name", categoryName);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi phục hồi danh mục: " + ex.Message);
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
                        "SELECT COUNT(*) FROM Products WHERE CategoryID=@catId AND Visible=TRUE", conn))
                    {
                        cmd.Parameters.AddWithValue("@catId", categoryId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi kiểm tra danh mục: " + ex.Message);
            }
        }
    }
}