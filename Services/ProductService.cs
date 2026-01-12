using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;
using Newtonsoft.Json;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic sản phẩm và danh mục
    /// 
    /// CHỨC NĂNG:
    /// - Quản lý sản phẩm (CRUD): Thêm, sửa, xóa
    /// - Quản lý danh mục (CRUD): Thêm, sửa, xóa
    /// - Tìm kiếm sản phẩm: Theo tên, danh mục
    /// - Cảnh báo tồn kho: Kiểm tra ngưỡng tối thiểu
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra đầu vào (ID, tên, giá, v.v...)
    /// 2. Repository call: Gọi DB để thực hiện thao tác
    /// 3. Logging: Ghi nhật ký ActionLogs
    /// 4. Change tracking: Gọi SaveManager.MarkAsChanged()
    /// 5. Return: Trả về kết quả
    /// </summary>
    public class ProductService
    {
        private readonly ProductRepository _productRepo;
        private readonly LogRepository _logRepo;

        public ProductService()
        {
            _productRepo = new ProductRepository();
            _logRepo = new LogRepository();
        }

        // ========== PRODUCT CRUD ==========

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
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra tên, giá, số lượng, ngưỡng tối thiểu
        /// 2. Repository.AddProduct(): Thêm vào database
        /// 3. LogAction(): Ghi nhật ký (DataBefore trống vì chưa tồn tại)
        /// 4. MarkAsChanged(): Đánh dấu có thay đổi
        /// 5. Return: Trả về ID sản phẩm vừa thêm
        /// </summary>
        public int AddProduct(string name, int categoryId, decimal price, int quantity, int minThreshold)
        {
            try
            {
                // Validation các trường đầu vào
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

                // Tạo đối tượng Product và gọi repository
                var product = new Product
                {
                    ProductName = name.Trim(),
                    CategoryID = categoryId,
                    Price = price,
                    Quantity = quantity,
                    MinThreshold = minThreshold
                };

                int productId = _productRepo.AddProduct(product);
                
                // Ghi nhật ký (DataBefore trống vì đây là thêm mới)
                _logRepo.LogAction("ADD_PRODUCT", 
                    $"Thêm sản phẩm: {name}", 
                    "");
                
                // Mark as changed for save manager
                SaveManager.Instance.MarkAsChanged();
                
                return productId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra tất cả các trường
        /// 2. GetProductById(): Lấy dữ liệu cũ trước khi thay đổi
        /// 3. Repository.UpdateProduct(): Cập nhật vào database
        /// 4. LogAction(): Ghi nhật ký với dữ liệu cũ (beforeData)
        /// 5. MarkAsChanged(): Đánh dấu có thay đổi
        /// 6. Return: Trả về kết quả thành công/thất bại
        /// </summary>
        public bool UpdateProduct(int productId, string name, int categoryId, decimal price, int quantity, int minThreshold)
        {
            try
            {
                // Validation các trường đầu vào
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

                // Lấy dữ liệu cũ trước khi cập nhật (để ghi nhật ký)
                var oldProduct = _productRepo.GetProductById(productId);
                if (oldProduct == null)
                    throw new ArgumentException("Sản phẩm không tồn tại");

                var beforeData = new
                {
                    ProductID = oldProduct.ProductID,
                    ProductName = oldProduct.ProductName,
                    CategoryID = oldProduct.CategoryID,
                    Price = oldProduct.Price,
                    Quantity = oldProduct.Quantity,
                    MinThreshold = oldProduct.MinThreshold
                };

                // Tạo đối tượng Product với dữ liệu mới
                var product = new Product
                {
                    ProductID = productId,
                    ProductName = name.Trim(),
                    CategoryID = categoryId,
                    Price = price,
                    Quantity = quantity,
                    MinThreshold = minThreshold
                };

                // Cập nhật vào database
                bool result = _productRepo.UpdateProduct(product);
                
                if (result)
                {
                    // Ghi nhật ký với dữ liệu cũ
                    _logRepo.LogAction("UPDATE_PRODUCT",
                        $"Cập nhật sản phẩm: {name}",
                        JsonConvert.SerializeObject(beforeData));
                    
                    // Đánh dấu có thay đổi chưa lưu
                    SaveManager.Instance.MarkAsChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật sản phẩm: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa sản phẩm (soft delete)
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra ID sản phẩm
        /// 2. GetProductById(): Lấy dữ liệu trước khi xóa
        /// 3. Repository.DeleteProduct(): Xóa mềm (soft delete)
        /// 4. LogAction(): Ghi nhật ký xóa với dữ liệu cũ
        /// 5. MarkAsChanged(): Đánh dấu có thay đổi
        /// 6. Return: Trả về kết quả thành công/thất bại
        /// </summary>
        public bool DeleteProduct(int productId)
        {
            try
            {
                if (productId <= 0)
                    throw new ArgumentException("ID sản phẩm không hợp lệ");
                
                // Lấy dữ liệu sản phẩm trước khi xóa (để ghi nhật ký)
                var product = _productRepo.GetProductById(productId);
                if (product != null)
                {
                    var beforeData = new
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        CategoryID = product.CategoryID,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        MinThreshold = product.MinThreshold
                    };
                    
                    // Xóa mềm: set Visible=FALSE trong database
                    bool result = _productRepo.DeleteProduct(productId);
                    
                    if (result)
                    {
                        // Ghi nhật ký xóa với dữ liệu cũ
                        _logRepo.LogAction("DELETE_PRODUCT",
                            $"Xóa sản phẩm: {product.ProductName}",
                            JsonConvert.SerializeObject(beforeData));
                        
                        // Mark as changed for save manager
                        SaveManager.Instance.MarkAsChanged();
                    }
                    
                    return result;
                }
                
                return false;
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

        // ========== CATEGORY CRUD ==========

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
                
                // Thêm danh mục vào database
                int categoryId = _productRepo.AddCategory(categoryName);
                
                // Ghi nhật ký (DataBefore trống vì đây là thêm mới)
                _logRepo.LogAction("ADD_CATEGORY",
                    $"Thêm danh mục: {categoryName}",
                    "");
                
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
        /// 2. GetAllCategories(): Lấy dữ liệu cũ trước khi cập nhật
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
                var categories = _productRepo.GetAllCategories();
                var oldCategory = categories.FirstOrDefault(c => c.CategoryID == categoryId);
                
                var beforeData = new
                {
                    CategoryID = oldCategory?.CategoryID,
                    CategoryName = oldCategory?.CategoryName
                };
                
                // Cập nhật danh mục vào database
                bool result = _productRepo.UpdateCategory(categoryId, categoryName);
                
                if (result)
                {
                    // Ghi nhật ký với dữ liệu cũ
                    _logRepo.LogAction("UPDATE_CATEGORY",
                        $"Cập nhật danh mục: {categoryName}",
                        JsonConvert.SerializeObject(beforeData));
                    
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
        /// 2. GetAllCategories(): Lấy dữ liệu cũ trước khi xóa
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
                
                // Lấy dữ liệu danh mục trước khi xóa
                var categories = _productRepo.GetAllCategories();
                var category = categories.FirstOrDefault(c => c.CategoryID == categoryId);
                
                if (category != null)
                {
                    var beforeData = new
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName
                    };
                    
                    // Xóa mềm: set Visible=FALSE trong database
                    bool result = _productRepo.DeleteCategory(categoryId);
                    
                    if (result)
                    {
                        // Ghi nhật ký xóa với dữ liệu cũ
                        _logRepo.LogAction("DELETE_CATEGORY",
                            $"Xóa danh mục: {category.CategoryName}",
                            JsonConvert.SerializeObject(beforeData));
                        
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
        /// Kiểm tra sản phẩm có phụ thuộc khóa ngoài hay không
        /// </summary>
        public bool ProductHasDependencies(int productId)
        {
            try
            {
                return _productRepo.HasForeignKeyReferences(productId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kiểm tra phụ thuộc: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra danh mục có sản phẩm hay không
        /// </summary>
        public bool CategoryHasProducts(int categoryId)
        {
            try
            {
                return _productRepo.CategoryHasProducts(categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kiểm tra danh mục: " + ex.Message);
            }
        }
    }
}
