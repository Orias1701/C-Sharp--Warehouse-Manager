using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến danh mục sản phẩm
    /// </summary>
    public class CategoryController
    {
        private readonly CategoryService _categoryService;

        public CategoryController()
        {
            _categoryService = new CategoryService();
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// </summary>
        public List<Category> GetAllCategories()
        {
            return _categoryService.GetAllCategories();
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        public Category GetCategoryById(int categoryId)
        {
            return _categoryService.GetCategoryById(categoryId);
        }

        /// <summary>
        /// Tìm kiếm danh mục theo tên
        /// </summary>
        public List<Category> SearchCategory(string keyword)
        {
            return _categoryService.SearchCategoryByName(keyword);
        }

        /// <summary>
        /// Thêm danh mục mới
        /// </summary>
        public int CreateCategory(string categoryName)
        {
            return _categoryService.AddCategory(categoryName);
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        public bool UpdateCategory(int categoryId, string categoryName)
        {
            return _categoryService.UpdateCategory(categoryId, categoryName);
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        public bool DeleteCategory(int categoryId)
        {
            return _categoryService.DeleteCategory(categoryId);
        }

        /// <summary>
        /// Kiểm tra danh mục có sản phẩm hay không
        /// </summary>
        public bool CategoryHasProducts(int categoryId)
        {
            return _categoryService.CategoryHasProducts(categoryId);
        }
    }
}