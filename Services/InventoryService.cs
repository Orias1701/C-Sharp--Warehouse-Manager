using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;
using Newtonsoft.Json;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic Nhập/Xuất kho
    /// 
    /// CHỨC NĂNG:
    /// - Nhập kho (ImportStock): Một phiếu nhập, một sản phẩm
    /// - Xuất kho (ExportStock): Một phiếu xuất, một sản phẩm
    /// - Nhập batch (ImportStockBatch): Một phiếu nhập, nhiều sản phẩm
    /// - Xuất batch (ExportStockBatch): Một phiếu xuất, nhiều sản phẩm
    /// - Tính toán tồn kho: Tự động cập nhật số lượng
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra sản phẩm, số lượng, giá
    /// 2. CreateTransaction(): Tạo phiếu nhập/xuất
    /// 3. AddTransactionDetail(): Thêm chi tiết phiếu
    /// 4. UpdateQuantity(): Cập nhật tồn kho
    /// 5. LogAction(): Ghi nhật ký
    /// 6. MarkAsChanged(): Đánh dấu có thay đổi
    /// 7. Return: Trả về kết quả
    /// </summary>
    public class InventoryService
    {
        private readonly ProductRepository _productRepo;
        private readonly TransactionRepository _transactionRepo;
        private readonly LogRepository _logRepo;
        private readonly CategoryRepository _categoryRepo;

        public InventoryService()
        {
            _productRepo = new ProductRepository();
            _transactionRepo = new TransactionRepository();
            _logRepo = new LogRepository();
            _categoryRepo = new CategoryRepository();
        }

        // ========== SINGLE IMPORT/EXPORT ==========

        /// <summary>
        /// Thực hiện phiếu nhập kho (một sản phẩm)
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra sản phẩm, số lượng, giá, không vượt giới hạn
        /// 2. CreateTransaction(): Tạo phiếu nhập
        /// 3. AddTransactionDetail(): Thêm chi tiết phiếu
        /// 4. UpdateQuantity(): Cập nhật tồn kho = cũ + số lượng nhập
        /// 5. LogAction(): Ghi nhật ký
        /// 6. MarkAsChanged(): Đánh dấu có thay đổi
        /// 7. Return: true nếu thành công
        /// </summary>
        public bool ImportStock(int productId, int quantity, decimal unitPrice, string note = "")
        {
            try
            {
                // Validation các trường đầu vào
                if (productId <= 0)
                    throw new ArgumentException("ID sản phẩm không hợp lệ");
                if (quantity <= 0)
                    throw new ArgumentException("Số lượng nhập phải lớn hơn 0");
                if (quantity > 999999)
                    throw new ArgumentException("Số lượng quá lớn");
                if (unitPrice < 0)
                    throw new ArgumentException("Đơn giá không được âm");
                if (unitPrice > 999999999)
                    throw new ArgumentException("Đơn giá quá lớn");

                var product = _productRepo.GetProductById(productId);
                if (product == null)
                    throw new ArgumentException("Sản phẩm không tồn tại");

                // Lưu dữ liệu cũ trước khi thay đổi (để ghi nhật ký)
                var oldData = new { product.Quantity, product.ProductID };
                
                // Tạo phiếu
                var transaction = new StockTransaction
                {
                    Type = "Import",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim()
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                // Thêm chi tiết
                var detail = new TransactionDetail
                {
                    TransactionID = transId,
                    ProductID = productId,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };
                _transactionRepo.AddTransactionDetail(detail);

                // Cập nhật tồn kho
                int newQuantity = product.Quantity + quantity;
                if (newQuantity > 999999)
                    throw new Exception("Tồn kho sẽ vượt quá giới hạn cho phép");

                _productRepo.UpdateQuantity(productId, newQuantity);

                // Ghi nhật ký
                var newData = new { Quantity = newQuantity, ProductID = productId };
                _logRepo.LogAction("IMPORT_STOCK", 
                    $"Nhập {quantity} sản phẩm ID {productId}",
                    JsonConvert.SerializeObject(oldData));

                ActionsService.Instance.MarkAsChanged();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi nhập kho: " + ex.Message);
            }
        }

        /// <summary>
        /// Thực hiện phiếu xuất kho (một sản phẩm)
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra sản phẩm, số lượng, giá
        /// 2. Check: Kiểm tra tồn kho có đủ để xuất không
        /// 3. CreateTransaction(): Tạo phiếu xuất
        /// 4. AddTransactionDetail(): Thêm chi tiết phiếu
        /// 5. UpdateQuantity(): Cập nhật tồn kho = cũ - số lượng xuất
        /// 6. LogAction(): Ghi nhật ký
        /// 7. MarkAsChanged(): Đánh dấu có thay đổi
        /// 8. Return: true nếu thành công
        /// </summary>
        public bool ExportStock(int productId, int quantity, decimal unitPrice, string note = "")
        {
            try
            {
                // Validation các trường đầu vào
                if (productId <= 0)
                    throw new ArgumentException("ID sản phẩm không hợp lệ");
                if (quantity <= 0)
                    throw new ArgumentException("Số lượng xuất phải lớn hơn 0");
                if (quantity > 999999)
                    throw new ArgumentException("Số lượng quá lớn");
                if (unitPrice < 0)
                    throw new ArgumentException("Đơn giá không được âm");
                if (unitPrice > 999999999)
                    throw new ArgumentException("Đơn giá quá lớn");

                var product = _productRepo.GetProductById(productId);
                if (product == null)
                    throw new ArgumentException("Sản phẩm không tồn tại");

                if (product.Quantity < quantity)
                    throw new Exception("Tồn kho không đủ để xuất (hiện có: " + product.Quantity + ")");

                // Lưu dữ liệu cũ
                var oldData = new { product.Quantity, product.ProductID };

                // Tạo phiếu
                var transaction = new StockTransaction
                {
                    Type = "Export",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim()
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                // Thêm chi tiết
                var detail = new TransactionDetail
                {
                    TransactionID = transId,
                    ProductID = productId,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };
                _transactionRepo.AddTransactionDetail(detail);

                // Cập nhật tồn kho
                int newQuantity = product.Quantity - quantity;
                _productRepo.UpdateQuantity(productId, newQuantity);

                // Ghi nhật ký
                _logRepo.LogAction("EXPORT_STOCK",
                    $"Xuất {quantity} sản phẩm ID {productId}",
                    JsonConvert.SerializeObject(oldData));

                ActionsService.Instance.MarkAsChanged();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xuất kho: " + ex.Message);
            }
        }

        // ========== BATCH IMPORT/EXPORT ==========

        /// <summary>
        /// Thực hiện phiếu nhập kho batch (nhiều sản phẩm, 1 phiếu)
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra danh sách không trống, không trùng lặp ID
        /// 2. CreateTransaction(): Tạo 1 phiếu nhập chung
        /// 3. Loop từng sản phẩm:
        ///    - Validation: Kiểm tra sản phẩm, số lượng, giá
        ///    - AddTransactionDetail(): Thêm chi tiết cho sản phẩm này
        ///    - UpdateQuantity(): Cập nhật tồn kho += số lượng
        /// 4. LogAction(): Ghi nhật ký 1 lần cho cả batch
        /// 5. MarkAsChanged(): Đánh dấu có thay đổi
        /// 6. Return: true nếu thành công
        /// </summary>
        public bool ImportStockBatch(List<(int ProductId, int Quantity, decimal UnitPrice)> details, string note = "")
        {
            try
            {
                if (details == null || details.Count == 0)
                    throw new ArgumentException("Danh sách sản phẩm không thể rỗng");

                // Kiểm tra trùng lặp ID trong list và kiểm tra sản phẩm tồn tại trước khi xử lý
                var productIds = new List<int>();
                foreach (var (productId, quantity, unitPrice) in details)
                {
                    if (productIds.Contains(productId))
                    {
                        throw new ArgumentException($"Sản phẩm ID {productId} bị trùng lặp trong phiếu nhập");
                    }
                    
                    if (!_productRepo.ProductIdExists(productId))
                    {
                        throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại trong hệ thống");
                    }
                    
                    productIds.Add(productId);
                }

                // Tạo transaction
                var transaction = new StockTransaction
                {
                    Type = "Import",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim()
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                // Xử lý từng sản phẩm
                foreach (var (productId, quantity, unitPrice) in details)
                {
                    // Validation
                    if (productId <= 0)
                        throw new ArgumentException("ID sản phẩm không hợp lệ");
                    if (quantity <= 0)
                        throw new ArgumentException("Số lượng nhập phải lớn hơn 0");
                    if (quantity > 999999)
                        throw new ArgumentException("Số lượng quá lớn");
                    if (unitPrice < 0)
                        throw new ArgumentException("Đơn giá không được âm");
                    if (unitPrice > 999999999)
                        throw new ArgumentException("Đơn giá quá lớn");

                    var product = _productRepo.GetProductById(productId);
                    if (product == null)
                        throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại");

                    // Thêm chi tiết
                    var detail = new TransactionDetail
                    {
                        TransactionID = transId,
                        ProductID = productId,
                        ProductName = product.ProductName,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    };
                    _transactionRepo.AddTransactionDetail(detail);

                    // Cập nhật tồn kho
                    int newQuantity = product.Quantity + quantity;
                    if (newQuantity > 999999)
                        throw new Exception("Tồn kho sẽ vượt quá giới hạn cho phép");

                    _productRepo.UpdateQuantity(productId, newQuantity);
                }

                // Cập nhật tổng giá trị của phiếu sau khi thêm tất cả chi tiết
                _transactionRepo.UpdateTransactionTotalValue(transId);

                // Ghi nhật ký
                _logRepo.LogAction("IMPORT_BATCH", 
                    $"Nhập {details.Count} sản phẩm, Transaction ID {transId}",
                    "");

                ActionsService.Instance.MarkAsChanged();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi nhập kho batch: " + ex.Message);
            }
        }

        /// <summary>
        /// Thực hiện phiếu xuất kho batch (nhiều sản phẩm, 1 phiếu)
        /// 
        /// LUỒNG:
        /// 1. Validation: Kiểm tra danh sách không trống, không trùng lặp ID
        /// 2. Check: Kiểm tra tồn kho của tất cả sản phẩm có đủ để xuất không
        /// 3. CreateTransaction(): Tạo 1 phiếu xuất chung
        /// 4. Loop từng sản phẩm:
        ///    - Validation: Kiểm tra sản phẩm, số lượng, giá
        ///    - AddTransactionDetail(): Thêm chi tiết cho sản phẩm này
        ///    - UpdateQuantity(): Cập nhật tồn kho -= số lượng
        /// 5. LogAction(): Ghi nhật ký 1 lần cho cả batch
        /// 6. MarkAsChanged(): Đánh dấu có thay đổi
        /// 7. Return: true nếu thành công
        /// </summary>
        public bool ExportStockBatch(List<(int ProductId, int Quantity, decimal UnitPrice)> details, string note = "")
        {
            try
            {
                if (details == null || details.Count == 0)
                    throw new ArgumentException("Danh sách sản phẩm không thể rỗng");

                // Kiểm tra trùng lặp ID trong list
                var productIds = new List<int>();
                foreach (var (productId, quantity, unitPrice) in details)
                {
                    if (productIds.Contains(productId))
                    {
                        throw new ArgumentException($"Sản phẩm ID {productId} bị trùng lặp trong phiếu xuất");
                    }
                    productIds.Add(productId);
                }

                // Kiểm tra tồn kho trước
                foreach (var (productId, quantity, unitPrice) in details)
                {
                    if (!_productRepo.ProductIdExists(productId))
                    {
                        throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại trong hệ thống");
                    }

                    var product = _productRepo.GetProductById(productId);
                    if (product == null)
                        throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại");
                    if (product.Quantity < quantity)
                        throw new Exception($"Tồn kho {product.ProductName} không đủ (hiện có: {product.Quantity}, cần xuất: {quantity})");
                }

                // Tạo transaction
                var transaction = new StockTransaction
                {
                    Type = "Export",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim()
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                // Xử lý từng sản phẩm
                foreach (var (productId, quantity, unitPrice) in details)
                {
                    // Validation
                    if (productId <= 0)
                        throw new ArgumentException("ID sản phẩm không hợp lệ");
                    if (quantity <= 0)
                        throw new ArgumentException("Số lượng xuất phải lớn hơn 0");
                    if (quantity > 999999)
                        throw new ArgumentException("Số lượng quá lớn");
                    if (unitPrice < 0)
                        throw new ArgumentException("Đơn giá không được âm");
                    if (unitPrice > 999999999)
                        throw new ArgumentException("Đơn giá quá lớn");

                    var product = _productRepo.GetProductById(productId);
                    
                    // Thêm chi tiết
                    var detail = new TransactionDetail
                    {
                        TransactionID = transId,
                        ProductID = productId,
                        ProductName = product.ProductName,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    };
                    _transactionRepo.AddTransactionDetail(detail);

                    // Cập nhật tồn kho
                    int newQuantity = product.Quantity - quantity;
                    _productRepo.UpdateQuantity(productId, newQuantity);
                }

                // Cập nhật tổng giá trị của phiếu sau khi thêm tất cả chi tiết
                _transactionRepo.UpdateTransactionTotalValue(transId);

                // Ghi nhật ký
                _logRepo.LogAction("EXPORT_BATCH",
                    $"Xuất {details.Count} sản phẩm, Transaction ID {transId}",
                    "");
                ActionsService.Instance.MarkAsChanged();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xuất kho batch: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách sản phẩm cảnh báo (tồn kho thấp)
        /// </summary>
        public List<Product> GetLowStockProducts()
        {
            try
            {
                var products = _productRepo.GetAllProducts();
                return products.Where(p => p.IsLowStock).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy sản phẩm cảnh báo: " + ex.Message);
            }
        }

        /// <summary>
        /// Tính tổng giá trị tồn kho
        /// </summary>
        public decimal GetTotalInventoryValue()
        {
            try
            {
                var products = _productRepo.GetAllProducts();
                return products.Sum(p => p.Price * p.Quantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tính giá trị tồn kho: " + ex.Message);
            }
        }

        /// <summary>
        /// Hoàn tác thao tác cuối cùng (dựa trên nhật ký)
        /// Hỗ trợ tối đa 10 hành động gần nhất dùng cấu trúc Stack (LIFO)
        /// Hành động được hoàn tác sẽ bị xóa khỏi stack (set Visible=FALSE) để tránh conflict
        /// </summary>
        public bool UndoLastAction()
        {
            try
            {
                // Get the last 10 actions (LIFO stack) - only Visible=TRUE entries
                var logs = _logRepo.GetLastNLogs(10);
                if (logs == null || logs.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("UndoLastAction: No logs available");
                    return false;
                }

                // Pop the first item (most recent action)
                var lastLog = logs.First();
                if (lastLog == null)
                {
                    System.Diagnostics.Debug.WriteLine("UndoLastAction: LastLog is null");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"UndoLastAction: Processing LogID={lastLog.LogID}, ActionType={lastLog.ActionType}");
                
                // Check if data is available
                if (string.IsNullOrWhiteSpace(lastLog.DataBefore))
                {
                    // For ADD actions, we can try to delete the record
                    if (lastLog.ActionType != null && lastLog.ActionType.StartsWith("ADD_"))
                    {
                        try
                        {
                            // Remove from undo stack after processing
                            if (_logRepo != null)
                            {
                                _logRepo.RemoveFromUndoStack(lastLog.LogID);
                                _logRepo.LogAction("UNDO_ACTION", $"Hoàn tác hành động {lastLog.ActionType}");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa hành động khỏi stack: {ex.Message}");
                        }
                        return true;
                    }
                    return false;
                }

                try
                {
                    Newtonsoft.Json.Linq.JObject jsonObj = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(lastLog.DataBefore);
                    
                    if (jsonObj == null)
                        return false;

                    bool undoSuccess = false;

                    // Handle different action types
                    if (string.IsNullOrWhiteSpace(lastLog.ActionType))
                        return false;

                    switch (lastLog.ActionType)
                    {
                        case "IMPORT_STOCK":
                        case "EXPORT_STOCK":
                            // Restore product quantity
                            if (jsonObj.ContainsKey("ProductID") && jsonObj.ContainsKey("Quantity"))
                            {
                                try
                                {
                                    int productId = (int)jsonObj["ProductID"];
                                    int oldQuantity = (int)jsonObj["Quantity"];
                                    _productRepo?.UpdateQuantity(productId, oldQuantity);
                                    undoSuccess = true;
                                }
                                catch { /* Suppress error */ }
                            }
                            break;

                        case "UPDATE_PRODUCT":
                            // Restore all product fields
                            if (jsonObj.ContainsKey("ProductID"))
                            {
                                try
                                {
                                    var product = new Product
                                    {
                                        ProductID = (int)jsonObj["ProductID"],
                                        ProductName = (string)jsonObj["ProductName"],
                                        CategoryID = (int)jsonObj["CategoryID"],
                                        Price = (decimal)jsonObj["Price"],
                                        Quantity = (int)jsonObj["Quantity"],
                                        MinThreshold = (int)jsonObj["MinThreshold"]
                                    };
                                    _productRepo?.UpdateProduct(product);
                                    undoSuccess = true;
                                }
                                catch { /* Suppress error */ }
                            }
                            break;

                        case "DELETE_PRODUCT":
                            // Restore deleted product
                            if (jsonObj.ContainsKey("ProductID"))
                            {
                                try
                                {
                                    int productId = (int)jsonObj["ProductID"];
                                    string productName = (string)jsonObj["ProductName"];
                                    int categoryId = (int)jsonObj["CategoryID"];
                                    decimal price = (decimal)jsonObj["Price"];
                                    int quantity = (int)jsonObj["Quantity"];
                                    int minThreshold = (int)jsonObj["MinThreshold"];
                                    
                                    // Restore by updating product visibility
                                    var product = new Product
                                    {
                                        ProductID = productId,
                                        ProductName = productName,
                                        CategoryID = categoryId,
                                        Price = price,
                                        Quantity = quantity,
                                        MinThreshold = minThreshold
                                    };
                                    _productRepo?.UpdateProduct(product);
                                    undoSuccess = true;
                                }
                                catch { /* Suppress error */ }
                            }
                            break;

                        case "UPDATE_CATEGORY":
                            // Restore category
                            if (jsonObj.ContainsKey("CategoryID"))
                            {
                                try
                                {
                                    int categoryId = (int)jsonObj["CategoryID"];
                                    string categoryName = (string)jsonObj["CategoryName"];
                                    _categoryRepo?.UpdateCategory(new Category { CategoryID = categoryId, CategoryName = categoryName });
                                    undoSuccess = true;
                                }
                                catch { /* Suppress error */ }
                            }
                            break;

                        case "DELETE_CATEGORY":
                            // Restore deleted category
                            if (jsonObj.ContainsKey("CategoryID"))
                            {
                                try
                                {
                                    int categoryId = (int)jsonObj["CategoryID"];
                                    string categoryName = (string)jsonObj["CategoryName"];
                                    // Create a restored category
                                    _categoryRepo?.RestoreDeletedCategory(categoryId, categoryName);
                                    undoSuccess = true;
                                }
                                catch { /* Suppress error */ }
                            }
                            break;

                        default:
                            // Unknown action type
                            return false;
                    }

                    // Remove from undo stack after successful undo
                    if (undoSuccess)
                    {
                        try
                        {
                            // Ensure the action is marked as processed
                            if (_logRepo != null)
                            {
                                bool removeSuccess = _logRepo.RemoveFromUndoStack(lastLog.LogID);
                                System.Diagnostics.Debug.WriteLine($"UndoLastAction: Removed LogID={lastLog.LogID} from stack, success={removeSuccess}");
                                
                                _logRepo.LogAction("UNDO_ACTION", $"Hoàn tác hành động {lastLog.ActionType}");
                                System.Diagnostics.Debug.WriteLine($"UndoLastAction: Logged UNDO_ACTION for {lastLog.ActionType}");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa hành động khỏi stack: {ex.Message}\n{ex.StackTrace}");
                        }
                        return true;
                    }

                    return false;
                }
                catch (JsonException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi hoàn tác: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả giao dịch
        /// </summary>
        public List<StockTransaction> GetAllTransactions()
        {
            try
            {
                return _transactionRepo.GetAllTransactions();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách giao dịch: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy giao dịch theo ID (bao gồm chi tiết)
        /// </summary>
        public StockTransaction GetTransactionById(int transactionId)
        {
            try
            {
                var result = _transactionRepo.GetTransactionById(transactionId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy giao dịch ID {transactionId}: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách nhật ký hành động
        /// </summary>
        public List<Actions> GetAllLogs()
        {
            try
            {
                return _logRepo.GetAllLogs();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhật ký: " + ex.Message);
            }
        }
    }
}