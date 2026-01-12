using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;
using Newtonsoft.Json;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic danh mục sản phẩm
    /// 
    /// CHỨC NĂNG:
    /// - Quản lý danh mục (CRUD): Thêm, sửa, xóa
    /// - Tìm kiếm danh mục: Theo tên
    /// - Kiểm tra phụ thuộc: Kiểm tra danh mục có sản phẩm hay không
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra đầu vào (ID, tên, v.v...)
    /// 2. Repository call: Gọi DB để thực hiện thao tác
    /// 3. Logging: Ghi nhật ký ActionLogs
    /// 4. Change tracking: Gọi SaveManager.MarkAsChanged()
    /// 5. Return: Trả về kết quả
    /// </summary>
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepo;
        private readonly ActionLogRepository _logRepo;

        public CategoryService()
        {
            _categoryRepo = new CategoryRepository();
            _logRepo = new ActionLogRepository();
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// </summary>
        public List<Category> GetAllCategories()
        {
            try
            {
                return _categoryRepo.GetAllCategories();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        public Category GetCategoryById(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    throw new ArgumentException("ID danh mục không hợp lệ");
                return _categoryRepo.GetCategoryById(categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Tìm kiếm danh mục theo tên (không phân biệt hoa thường)
        /// </summary>
        public List<Category> SearchCategoryByName(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllCategories();

                var categories = GetAllCategories();
                return categories.Where(c => c.CategoryName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm kiếm danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Thêm danh mục mới
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra tên danh mục không trống, <= 100 ký tự
        /// 2. Repository.AddCategory(): Thêm vào database
        /// 3. LogAction(): Ghi nhật ký (DataBefore trống)
        /// 4. MarkAsChanged(): Đánh dấu có thay đổi
        /// 5. Return: Trả về ID danh mục vừa thêm
        /// </summary>
        public int AddCategory(string categoryName)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(categoryName))
                    throw new ArgumentException("Tên danh mục không được trống");
                if (categoryName.Length > 100)
                    throw new ArgumentException("Tên danh mục không được vượt 100 ký tự");

                var category = new Category
                {
                    CategoryName = categoryName.Trim()
                };

                // Thêm danh mục vào database
                int categoryId = _categoryRepo.AddCategory(category);

                // Ghi nhật ký
                var log = new ActionLog
                {
                    ActionType = "ADD_CATEGORY",
                    Descriptions = $"Thêm danh mục: {categoryName}",
                    DataBefore = "",
                    CreatedAt = DateTime.Now
                };
                _logRepo.LogAction(log);

                // Đánh dấu có thay đổi chưa lưu
                SaveManager.Instance.MarkAsChanged();
                return categoryId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật danh mục
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra ID và tên danh mục
        /// 2. GetCategoryById(): Lấy dữ liệu cũ trước khi cập nhật
        /// 3. Repository.UpdateCategory(): Cập nhật vào database
        /// 4. LogAction(): Ghi nhật ký với dữ liệu cũ
        /// 5. MarkAsChanged(): Đánh dấu có thay đổi
        /// 6. Return: Trả về kết quả thành công/thất bại
        /// </summary>
        public bool UpdateCategory(int categoryId, string categoryName)
        {
            try
            {
                // Validation
                if (categoryId <= 0)
                    throw new ArgumentException("ID danh mục không hợp lệ");
                if (string.IsNullOrWhiteSpace(categoryName))
                    throw new ArgumentException("Tên danh mục không được trống");
                if (categoryName.Length > 100)
                    throw new ArgumentException("Tên danh mục không được vượt 100 ký tự");

                // Lấy dữ liệu cũ trước khi cập nhật
                var oldCategory = _categoryRepo.GetCategoryById(categoryId);
                if (oldCategory == null)
                    throw new ArgumentException("Danh mục không tồn tại");

                var beforeData = new
                {
                    CategoryID = oldCategory.CategoryID,
                    CategoryName = oldCategory.CategoryName
                };

                // Cập nhật danh mục vào database
                var category = new Category
                {
                    CategoryID = categoryId,
                    CategoryName = categoryName.Trim()
                };

                bool result = _categoryRepo.UpdateCategory(category);

                if (result)
                {
                    // Ghi nhật ký với dữ liệu cũ
                    var log = new ActionLog
                    {
                        ActionType = "UPDATE_CATEGORY",
                        Descriptions = $"Cập nhật danh mục: {categoryName}",
                        DataBefore = JsonConvert.SerializeObject(beforeData),
                        CreatedAt = DateTime.Now
                    };
                    _logRepo.LogAction(log);

                    // Đánh dấu có thay đổi chưa lưu
                    SaveManager.Instance.MarkAsChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa danh mục (soft delete)
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra ID danh mục
        /// 2. GetCategoryById(): Lấy dữ liệu cũ trước khi xóa
        /// 3. Repository.DeleteCategory(): Xóa mềm (soft delete)
        /// 4. LogAction(): Ghi nhật ký xóa với dữ liệu cũ
        /// 5. MarkAsChanged(): Đánh dấu có thay đổi
        /// 6. Return: Trả về kết quả thành công/thất bại
        /// </summary>
        public bool DeleteCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    throw new ArgumentException("ID danh mục không hợp lệ");

                // Kiểm tra danh mục có sản phẩm hay không
                if (_categoryRepo.CategoryHasProducts(categoryId))
                    throw new ArgumentException("Không thể xóa danh mục vì còn sản phẩm");

                // Lấy dữ liệu danh mục trước khi xóa
                var category = _categoryRepo.GetCategoryById(categoryId);

                if (category != null)
                {
                    var beforeData = new
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName
                    };

                    // Xóa mềm: set Visible=FALSE trong database
                    bool result = _categoryRepo.DeleteCategory(categoryId);

                    if (result)
                    {
                        // Ghi nhật ký xóa với dữ liệu cũ
                        var log = new ActionLog
                        {
                            ActionType = "DELETE_CATEGORY",
                            Descriptions = $"Xóa danh mục: {category.CategoryName}",
                            DataBefore = JsonConvert.SerializeObject(beforeData),
                            CreatedAt = DateTime.Now
                        };
                        _logRepo.LogAction(log);

                        // Đánh dấu có thay đổi chưa lưu
                        SaveManager.Instance.MarkAsChanged();
                    }

                    return result;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra danh mục có sản phẩm hay không
        /// </summary>
        public bool CategoryHasProducts(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    throw new ArgumentException("ID danh mục không hợp lệ");
                return _categoryRepo.CategoryHasProducts(categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kiểm tra danh mục: " + ex.Message);
            }
        }
    }
}
