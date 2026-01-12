using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến sản phẩm
    /// </summary>
    public class ProductController
    {
        private readonly ProductService _productService;

        public ProductController()
        {
            _productService = new ProductService();
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        public List<Product> GetAllProducts()
        {
            return _productService.GetAllProducts();
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên
        /// </summary>
        public List<Product> SearchProduct(string keyword)
        {
            return _productService.SearchProductByName(keyword);
        }

        /// <summary>
        /// Lấy sản phẩm theo danh mục
        /// </summary>
        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _productService.GetProductsByCategory(categoryId);
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        public Product GetProductById(int productId)
        {
            return _productService.GetProductById(productId);
        }

        /// <summary>
        /// Thêm sản phẩm mới (overload)
        /// </summary>
        public int AddProduct(Product product)
        {
            return _productService.AddProduct(product.ProductName, product.CategoryID, product.Price, product.Quantity, product.MinThreshold);
        }

        /// <summary>
        /// Thêm sản phẩm mới
        /// </summary>
        public int CreateProduct(string name, int categoryId, decimal price, int quantity, int minThreshold)
        {
            return _productService.AddProduct(name, categoryId, price, quantity, minThreshold);
        }

        /// <summary>
        /// Cập nhật sản phẩm (overload)
        /// </summary>
        public bool UpdateProduct(Product product)
        {
            return _productService.UpdateProduct(product.ProductID, product.ProductName, product.CategoryID, product.Price, product.Quantity, product.MinThreshold);
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        public bool UpdateProductFull(int productId, string name, int categoryId, decimal price, int quantity, int minThreshold)
        {
            return _productService.UpdateProduct(productId, name, categoryId, price, quantity, minThreshold);
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        public bool DeleteProduct(int productId)
        {
            return _productService.DeleteProduct(productId);
        }

        /// <summary>
        /// Kiểm tra sản phẩm có cảnh báo tồn kho
        /// </summary>
        public bool IsLowStock(int productId)
        {
            return _productService.IsProductLowStock(productId);
        }

        /// <summary>
        /// Kiểm tra sản phẩm có phụ thuộc khóa ngoài
        /// </summary>
        public bool ProductHasDependencies(int productId)
        {
            return _productService.ProductHasDependencies(productId);
        }
    }
}
