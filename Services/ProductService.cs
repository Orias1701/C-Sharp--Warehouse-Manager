using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic sản phẩm (tìm kiếm, kiểm tra ngưỡng)
    /// </summary>
    public class ProductService
    {
        private readonly ProductRepository _productRepo;

        public ProductService()
        {
            _productRepo = new ProductRepository();
        }

        /// <summary>
        /// Lấy tất cả sản phẩm
        /// </summary>
        public List<Product> GetAllProducts()
        {
            try
            {
                return _productRepo.GetAllProducts();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên (không phân biệt hoa thường)
        /// </summary>
        public List<Product> SearchProductByName(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return GetAllProducts();

                var products = GetAllProducts();
                return products.Where(p => p.ProductName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm kiếm sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo danh mục
        /// </summary>
        public List<Product> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = GetAllProducts();
                return products.Where(p => p.CategoryID == categoryId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy sản phẩm theo danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra sản phẩm có cảnh báo tồn kho hay không
        /// </summary>
        public bool IsProductLowStock(int productId)
        {
            try
            {
                var product = _productRepo.GetProductById(productId);
                return product != null && product.IsLowStock;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi kiểm tra tồn kho: " + ex.Message);
            }
        }

        /// <summary>
        /// Thêm sản phẩm mới
        /// </summary>
        public int AddProduct(string name, int categoryId, decimal price, int quantity, int minThreshold)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Tên sản phẩm không được để trống");
                if (name.Length > 200)
                    throw new ArgumentException("Tên sản phẩm không được vượt quá 200 ký tự");
                if (price < 0)
                    throw new ArgumentException("Giá sản phẩm phải >= 0");
                if (price > 999999999)
                    throw new ArgumentException("Giá sản phẩm quá lớn");
                if (quantity < 0)
                    throw new ArgumentException("Số lượng không được âm");
                if (quantity > 999999)
                    throw new ArgumentException("Số lượng quá lớn");
                if (minThreshold < 0)
                    throw new ArgumentException("Ngưỡng tối thiểu không được âm");
                if (minThreshold > quantity)
                    throw new ArgumentException("Ngưỡng tối thiểu không được vượt quá số lượng hiện tại");
                if (categoryId <= 0)
                    throw new ArgumentException("Danh mục không hợp lệ");

                var product = new Product
                {
                    ProductName = name.Trim(),
                    CategoryID = categoryId,
                    Price = price,
                    Quantity = quantity,
                    MinThreshold = minThreshold
                };

                return _productRepo.AddProduct(product);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        public bool UpdateProduct(int productId, string name, int categoryId, decimal price, int quantity, int minThreshold)
        {
            try
            {
                // Validation
                if (productId <= 0)
                    throw new ArgumentException("ID sản phẩm không hợp lệ");
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Tên sản phẩm không được để trống");
                if (name.Length > 200)
                    throw new ArgumentException("Tên sản phẩm không được vượt quá 200 ký tự");
                if (price < 0)
                    throw new ArgumentException("Giá sản phẩm phải >= 0");
                if (price > 999999999)
                    throw new ArgumentException("Giá sản phẩm quá lớn");
                if (quantity < 0)
                    throw new ArgumentException("Số lượng không được âm");
                if (quantity > 999999)
                    throw new ArgumentException("Số lượng quá lớn");
                if (minThreshold < 0)
                    throw new ArgumentException("Ngưỡng tối thiểu không được âm");
                if (minThreshold > quantity)
                    throw new ArgumentException("Ngưỡng tối thiểu không được vượt quá số lượng hiện tại");
                if (categoryId <= 0)
                    throw new ArgumentException("Danh mục không hợp lệ");

                var product = new Product
                {
                    ProductID = productId,
                    ProductName = name.Trim(),
                    CategoryID = categoryId,
                    Price = price,
                    Quantity = quantity,
                    MinThreshold = minThreshold
                };

                return _productRepo.UpdateProduct(product);
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
                if (productId <= 0)
                    throw new ArgumentException("ID sản phẩm không hợp lệ");
                return _productRepo.DeleteProduct(productId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID
        /// </summary>
        public Product GetProductById(int productId)
        {
            try
            {
                if (productId <= 0)
                    throw new ArgumentException("ID sản phẩm không hợp lệ");
                return _productRepo.GetProductById(productId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy thông tin sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// </summary>
        public List<Category> GetAllCategories()
        {
            try
            {
                return _productRepo.GetAllCategories();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh mục: " + ex.Message);
            }
        }

        /// <summary>
        /// Thêm danh mục mới
        /// </summary>
        public int AddCategory(string categoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                    throw new ArgumentException("Tên danh mục không được trống");
                if (categoryName.Length > 100)
                    throw new ArgumentException("Tên danh mục không được vượt 100 ký tự");
                return _productRepo.AddCategory(categoryName);
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
                if (categoryId <= 0)
                    throw new ArgumentException("ID danh mục không hợp lệ");
                if (string.IsNullOrWhiteSpace(categoryName))
                    throw new ArgumentException("Tên danh mục không được trống");
                if (categoryName.Length > 100)
                    throw new ArgumentException("Tên danh mục không được vượt 100 ký tự");
                return _productRepo.UpdateCategory(categoryId, categoryName);
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
                if (categoryId <= 0)
                    throw new ArgumentException("ID danh mục không hợp lệ");
                return _productRepo.DeleteCategory(categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa danh mục: " + ex.Message);
            }
        }
    }
}
