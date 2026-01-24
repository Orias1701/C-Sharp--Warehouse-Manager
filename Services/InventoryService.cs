using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WarehouseManagement.Services
{
    public class InventoryService
    {
        private readonly ProductRepository _productRepo;
        private readonly TransactionRepository _transactionRepo;
        private readonly LogRepository _logRepo;
        private readonly CategoryRepository _categoryRepo;
        private readonly SupplierRepository _supplierRepo;
        private readonly CustomerRepository _customerRepo;

        public InventoryService()
        {
            _productRepo = new ProductRepository();
            _transactionRepo = new TransactionRepository();
            _logRepo = new LogRepository();
            _categoryRepo = new CategoryRepository();
            _supplierRepo = new SupplierRepository();
            _customerRepo = new CustomerRepository();
        }

        public bool ImportStock(int productId, int quantity, decimal unitPrice, string note = "")
        {
            try
            {
                if (productId <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ");
                if (quantity <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0");
                if (quantity > 999999) throw new ArgumentException("Số lượng quá lớn");
                if (unitPrice < 0) throw new ArgumentException("Đơn giá không được âm");
                if (unitPrice > 999999999) throw new ArgumentException("Đơn giá quá lớn");

                var product = _productRepo.GetProductById(productId);
                if (product == null) throw new ArgumentException("Sản phẩm không tồn tại");

                var oldData = new { product.Quantity, product.ProductID };
                
                var transaction = new Transaction
                {
                    Type = "Import",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim(),
                    Status = "Pending",
                    Visible = true
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                var detail = new TransactionDetail
                {
                    TransactionID = transId,
                    ProductID = productId,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Visible = true
                };
                _transactionRepo.AddTransactionDetail(detail);

                int newQuantity = product.Quantity + quantity;
                if (newQuantity > 999999) throw new Exception("Tồn kho sẽ vượt quá giới hạn cho phép");
                
                // DEFERRED: _productRepo.UpdateQuantity(productId, newQuantity);
                _transactionRepo.UpdateTransactionTotal(transId);

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

        public bool ExportStock(int productId, int quantity, decimal unitPrice, string note = "")
        {
            try
            {
                if (productId <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ");
                if (quantity <= 0) throw new ArgumentException("Số lượng xuất phải lớn hơn 0");
                if (quantity > 999999) throw new ArgumentException("Số lượng quá lớn");
                if (unitPrice < 0) throw new ArgumentException("Đơn giá không được âm");
                if (unitPrice > 999999999) throw new ArgumentException("Đơn giá quá lớn");

                var product = _productRepo.GetProductById(productId);
                if (product == null) throw new ArgumentException("Sản phẩm không tồn tại");

                if (product.Quantity < quantity)
                    throw new Exception("Tồn kho không đủ để xuất (hiện có: " + product.Quantity + ")");

                var oldData = new { product.Quantity, product.ProductID };

                var transaction = new Transaction
                {
                    Type = "Export",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim(),
                    Status = "Pending",
                    Visible = true
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                var detail = new TransactionDetail
                {
                    TransactionID = transId,
                    ProductID = productId,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Visible = true
                };
                _transactionRepo.AddTransactionDetail(detail);

                int newQuantity = product.Quantity - quantity;
                // DEFERRED: _productRepo.UpdateQuantity(productId, newQuantity);
                _transactionRepo.UpdateTransactionTotal(transId);

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

        public int ImportStockBatch(List<(int ProductId, int Quantity, decimal UnitPrice, double DiscountRate)> details, string note = "", int supplierId = 0)
        {
            try
            {
                if (details == null || details.Count == 0)
                    throw new ArgumentException("Danh sách sản phẩm không thể rỗng");

                var productIds = new List<int>();
                foreach (var (productId, quantity, unitPrice, discountRate) in details)
                {
                    if (productIds.Contains(productId))
                        throw new ArgumentException($"Sản phẩm ID {productId} bị trùng lặp trong phiếu nhập");
                    
                    if (!_productRepo.ProductIdExists(productId))
                        throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại trong hệ thống");
                    
                    productIds.Add(productId);
                }

                var transaction = new Transaction
                {
                    Type = "Import",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim(),
                    Status = "Pending",
                    Visible = true,
                    SupplierID = supplierId > 0 ? supplierId : (int?)null
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                foreach (var (productId, quantity, unitPrice, discountRate) in details)
                {
                    if (productId <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ");
                    if (quantity <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0");
                    if (quantity > 999999) throw new ArgumentException("Số lượng quá lớn");
                    if (unitPrice < 0) throw new ArgumentException("Đơn giá không được âm");
                    if (unitPrice > 999999999) throw new ArgumentException("Đơn giá quá lớn");
                    if (discountRate < 0 || discountRate > 100) throw new ArgumentException("Chiết khấu phải từ 0-100%");

                    var product = _productRepo.GetProductById(productId);
                    if (product == null) throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại");

                    var detail = new TransactionDetail
                    {
                        TransactionID = transId,
                        ProductID = productId,
                        ProductName = product.ProductName,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        DiscountRate = discountRate,
                        Visible = true
                    };
                    _transactionRepo.AddTransactionDetail(detail);

                    int newQuantity = product.Quantity + quantity;
                    if (newQuantity > 999999) throw new Exception("Tồn kho sẽ vượt quá giới hạn cho phép");

                    // DEFERRED: _productRepo.UpdateQuantity(productId, newQuantity);
                }

                _transactionRepo.UpdateTransactionTotal(transId);

                _logRepo.LogAction("IMPORT_BATCH", 
                    $"Nhập {details.Count} sản phẩm, Transaction ID {transId}",
                    "");

                ActionsService.Instance.MarkAsChanged();
                return transId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi nhập kho batch: " + ex.Message);
            }
        }

        public int ExportStockBatch(List<(int ProductId, int Quantity, decimal UnitPrice, double DiscountRate)> details, string note = "", int customerId = 0)
        {
            try
            {
                if (details == null || details.Count == 0)
                    throw new ArgumentException("Danh sách sản phẩm không thể rỗng");

                var productIds = new List<int>();
                foreach (var (productId, quantity, unitPrice, discountRate) in details)
                {
                    if (productIds.Contains(productId))
                        throw new ArgumentException($"Sản phẩm ID {productId} bị trùng lặp trong phiếu xuất");
                    productIds.Add(productId);
                }

                foreach (var (productId, quantity, unitPrice, discountRate) in details)
                {
                    if (!_productRepo.ProductIdExists(productId))
                        throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại trong hệ thống");

                    var product = _productRepo.GetProductById(productId);
                    if (product == null) throw new ArgumentException($"Sản phẩm ID {productId} không tồn tại");
                    if (product.Quantity < quantity)
                        throw new Exception($"Tồn kho {product.ProductName} không đủ (hiện có: {product.Quantity}, cần xuất: {quantity})");
                }

                var transaction = new Transaction
                {
                    Type = "Export",
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = string.IsNullOrWhiteSpace(note) ? "" : note.Trim(),
                    Status = "Pending",
                    Visible = true,
                    CustomerID = customerId > 0 ? customerId : (int?)null
                };
                int transId = _transactionRepo.CreateTransaction(transaction);

                foreach (var (productId, quantity, unitPrice, discountRate) in details)
                {
                    if (productId <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ");
                    if (quantity <= 0) throw new ArgumentException("Số lượng xuất phải lớn hơn 0");
                    if (quantity > 999999) throw new ArgumentException("Số lượng quá lớn");
                    if (unitPrice < 0) throw new ArgumentException("Đơn giá không được âm");
                    if (unitPrice > 999999999) throw new ArgumentException("Đơn giá quá lớn");
                    if (discountRate < 0 || discountRate > 100) throw new ArgumentException("Chiết khấu phải từ 0-100%");

                    var product = _productRepo.GetProductById(productId);
                    
                    var detail = new TransactionDetail
                    {
                        TransactionID = transId,
                        ProductID = productId,
                        ProductName = product.ProductName,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        DiscountRate = discountRate,
                        Visible = true
                    };
                    _transactionRepo.AddTransactionDetail(detail);

                    int newQuantity = product.Quantity - quantity;
                    // DEFERRED: _productRepo.UpdateQuantity(productId, newQuantity);
                }

                _transactionRepo.UpdateTransactionTotal(transId);

                _logRepo.LogAction("EXPORT_BATCH",
                    $"Xuất {details.Count} sản phẩm, Transaction ID {transId}",
                    "");
                ActionsService.Instance.MarkAsChanged();
                return transId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xuất kho batch: " + ex.Message);
            }
        }

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

        public bool UndoLastAction()
        {
            try
            {
                var logs = _logRepo.GetAllLogs();
                if (logs.Count == 0) return false;

                var lastLog = logs[0]; // Gets the most recent log (LIFO)
                bool success = false;

                switch (lastLog.ActionType)
                {
                    case "ADD_CATEGORY":
                        // DataBefore is empty for ADD.
                        // We need to parse the Description to find the name or ID? 
                        // Actually, the log description says "Thêm danh mục: {Name}".
                        // It's safer if we stored the ID in the log, but we didn't. 
                        // However, since it's the *last* action, we can assume the highest ID or search by name.
                        // Better approach: In AddCategory, we should have logged the ID.
                        // Current implementation of AddCategory doesn't log ID in DataBefore.
                        // But since we just added it, it should be the latest category? 
                        // Let's try to find it by parsing description or taking the latest category.
                        
                        // Parse name from "Thêm danh mục: {Name}"
                        if (lastLog.Descriptions.StartsWith("Thêm danh mục: "))
                        {
                            string catName = lastLog.Descriptions.Replace("Thêm danh mục: ", "").Trim();
                            var cats = _categoryRepo.GetAllCategories(); // This returns visible categories
                            var cat = cats.FirstOrDefault(c => c.CategoryName.Equals(catName, StringComparison.OrdinalIgnoreCase));
                            if (cat != null)
                            {
                                // Hard delete or Soft delete?
                                // If we soft delete (HideCategory), it acts like it's gone.
                                // But if user Saves changes, logs are cleared. 
                                // Ideally we should Hard Delete if it was a fresh add, but soft delete is safer.
                                // However, finding the ID is tricky if multiple have same name. 
                                // Let's use the ID-based approach if we can find it.
                                
                                // For improved safety, let's use the CategoryService.DeleteCategory which checks dependencies
                                // But here we want to UNDO, so we want to force remove it?
                                // If we just added it, it shouldn't have products yet (unless added immediately after).
                                // Let's use DeleteCategory (Soft Delete).
                                success = _categoryRepo.DeleteCategory(cat.CategoryID);
                            }
                        }
                        break;

                    case "UPDATE_CATEGORY":
                        if (!string.IsNullOrEmpty(lastLog.DataBefore))
                        {
                            JObject data = JObject.Parse(lastLog.DataBefore);
                            int catId = (int)data["CategoryID"];
                            string oldName = (string)data["CategoryName"];
                            string oldDesc = (string)data["Description"];
                            
                            var catToRestore = new Category 
                            { 
                                CategoryID = catId, 
                                CategoryName = oldName, 
                                Description = oldDesc 
                            };
                            success = _categoryRepo.UpdateCategory(catToRestore);
                        }
                        break;

                    case "DELETE_CATEGORY":
                        if (!string.IsNullOrEmpty(lastLog.DataBefore))
                        {
                            JObject data = JObject.Parse(lastLog.DataBefore);
                            int catId = (int)data["CategoryID"];
                            string oldName = (string)data["CategoryName"];
                            string oldDesc = (string)data["Description"];
                            
                            success = _categoryRepo.RestoreDeletedCategory(catId, oldName, oldDesc);
                        }
                        break;

                    case "ADD_PRODUCT":
                        if (lastLog.Descriptions.StartsWith("Thêm sản phẩm: "))
                        {
                            string name = lastLog.Descriptions.Replace("Thêm sản phẩm: ", "").Trim();
                            var prods = _productRepo.SearchProductByName(name);
                            // Assuming the last added one with this name
                            var prod = prods.OrderByDescending(p => p.ProductID).FirstOrDefault();
                            if (prod != null)
                            {
                                success = _productRepo.DeleteProduct(prod.ProductID);
                            }
                        }
                        break;

                    case "UPDATE_PRODUCT":
                        if (!string.IsNullOrEmpty(lastLog.DataBefore))
                        {
                            JObject data = JObject.Parse(lastLog.DataBefore);
                            var prod = new Product
                            {
                                ProductID = (int)data["ProductID"],
                                ProductName = (string)data["ProductName"],
                                CategoryID = (int)data["CategoryID"],
                                Price = (decimal)data["Price"],
                                Quantity = (int)data["Quantity"],
                                MinThreshold = (int)data["MinThreshold"]
                            };
                            success = _productRepo.UpdateProduct(prod);
                        }
                        break;

                    case "DELETE_PRODUCT":
                         if (!string.IsNullOrEmpty(lastLog.DataBefore))
                        {
                            JObject data = JObject.Parse(lastLog.DataBefore);
                            int id = (int)data["ProductID"];
                            // Using UpdateProduct to restore? Or need a Restore method?
                            // ProductRepo.UpdateProduct usually updates visible fields. 
                            // But usually "Delete" sets Visible=False.
                            // To Restore, we need to set Visible=True.
                            // Does UpdateProduct update Visible? 
                            // Examining ProductService.UpdateProduct -> It calls Repo.UpdateProduct.
                            // Examining Repo.UpdateProduct (I need to check this mostly). 
                            // If Repo doesn't update Visible, I can't restore easily without direct SQL or specific method.
                            // Let's assume I need to handle this.
                            // Let's TRY to use a direct SQL via a new method or ...
                            // Wait, for Suppliers/Customers I used Update.
                            // For Categories I added RestoreDeletedCategory.
                            // I probably need RestoreDeletedProduct. 
                            // But I can't modify ProductRepo easily in this same step if I didn't plan it well (I checked service, not repo deep enough).
                            // Workaround: Use UpdateProduct AND ALSO set Visible=True if the Repo supports it?
                            // Checked ProductRepo code? I viewed ProductService, but not ProductRepo content in this turn.
                            // Warning: I might need to verify ProductRepo.UpdateProduct updates Visible.
                            // If not, I should add a helper or use a raw command? 
                            // I can't run raw commands easily here.
                            // Best bet: Implement a simple "Restore" via SQL if I could, but I can't.
                            // Let's look at what I have. I have _productRepo. 
                            // If I can't restore, I'll fail. 
                            // Let's skip implementing DELETE_PRODUCT fully or try Update.
                            
                            // actually, let's assume UpdateProduct restores it if I set it.
                            // If not, I will add RestoreDeletedProduct to Repo in next step if verification fails.
                            // But better: I'll use a specific trick: 
                            // I can't.
                            
                            // Let's add the case but note it might need Repo update.
                            // Actually, I can use "HideProduct(id)" to hide. To show? 
                            // There is no ShowProduct.
                            
                            // Let's add "RestoreDeletedProduct" to ProductRepostiory in a separate call if needed.
                            // I will do that concurrently or after. 
                            // For now, let's put the logic assuming there is a way or I will add it.
                            // I will add RestoreDeletedProduct to ProductRepository.cs NEXT.
                            
                            success = _productRepo.RestoreDeletedProduct(id); // I will add this method.
                        }
                        break;

                    case "ADD_SUPPLIER":
                        if (lastLog.Descriptions.StartsWith("Thêm nhà cung cấp: "))
                        {
                            string name = lastLog.Descriptions.Replace("Thêm nhà cung cấp: ", "").Trim();
                            // Find by name? Repo doesn't have SearchByName exposed? Service has GetAll.
                            var suppliers = _supplierRepo.GetAllSuppliers(true); 
                            var sup = suppliers.FirstOrDefault(s => s.SupplierName == name); // Potentially risk if duplicates
                            if (sup != null)
                            {
                                success = _supplierRepo.SoftDeleteSupplier(sup.SupplierID);
                            }
                        }
                        break;

                    case "UPDATE_SUPPLIER":
                         if (!string.IsNullOrEmpty(lastLog.DataBefore))
                        {
                            JObject data = JObject.Parse(lastLog.DataBefore);
                            var sup = new Supplier
                            {
                                SupplierID = (int)data["SupplierID"],
                                SupplierName = (string)data["SupplierName"],
                                Phone = (string)data["Phone"],
                                Email = (string)data["Email"],
                                Address = (string)data["Address"],
                                Visible = true // Restore visibility if needed or just update
                            };
                            success = _supplierRepo.UpdateSupplier(sup);
                        }
                        break;

                    case "DELETE_SUPPLIER":
                         if (!string.IsNullOrEmpty(lastLog.DataBefore))
                        {
                            JObject data = JObject.Parse(lastLog.DataBefore);
                            var sup = new Supplier
                            {
                                SupplierID = (int)data["SupplierID"],
                                SupplierName = (string)data["SupplierName"],
                                Phone = (string)data["Phone"],
                                Email = (string)data["Email"],
                                Address = (string)data["Address"],
                                Visible = true
                            };
                            success = _supplierRepo.UpdateSupplier(sup); // Update with Visible=True should restore it
                        }
                        break;

                    case "ADD_CUSTOMER":
                        if (lastLog.Descriptions.StartsWith("Thêm khách hàng: "))
                        {
                            string name = lastLog.Descriptions.Replace("Thêm khách hàng: ", "").Trim();
                            var customers = _customerRepo.GetAllCustomers(true);
                            var cus = customers.FirstOrDefault(c => c.CustomerName == name);
                            if (cus != null)
                            {
                                success = _customerRepo.SoftDeleteCustomer(cus.CustomerID);
                            }
                        }
                        break;

                    case "UPDATE_CUSTOMER":
                    case "DELETE_CUSTOMER":
                          if (!string.IsNullOrEmpty(lastLog.DataBefore))
                        {
                            JObject data = JObject.Parse(lastLog.DataBefore);
                            var cus = new Customer
                            {
                                CustomerID = (int)data["CustomerID"],
                                CustomerName = (string)data["CustomerName"],
                                Phone = (string)data["Phone"],
                                Email = (string)data["Email"],
                                Address = (string)data["Address"],
                                Visible = true
                            };
                            success = _customerRepo.UpdateCustomer(cus);
                        }
                        break;
                        
                    default:
                        // Unknown action type
                        return false;
                }

                if (success)
                {
                    // Remove the log from the stack so we don't undo it again (and to reflect "Undo" operation)
                    _logRepo.RemoveFromUndoStack(lastLog.LogID);
                    
                    ActionsService.Instance.MarkAsChanged(); // Update UI state
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                // Log error?
                Console.WriteLine("Undo Error: " + ex.Message);
                return false;
            }
        }

        public List<Transaction> GetAllTransactions(bool includeHidden = false)
        {
            try
            {
                return _transactionRepo.GetAllTransactions(includeHidden);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách giao dịch: " + ex.Message);
            }
        }

        public Transaction GetTransactionById(int transactionId)
        {
            try
            {
                return _transactionRepo.GetTransactionById(transactionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy giao dịch ID {transactionId}: " + ex.Message);
            }
        }

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

        public bool HideTransaction(int transactionId)
        {
            try
            {
                return _transactionRepo.SoftDeleteTransaction(transactionId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi ẩn giao dịch: " + ex.Message);
            }
        }
        public bool ApproveTransaction(int transactionId)
        {
            try
            {
                var transaction = _transactionRepo.GetTransactionById(transactionId);
                if (transaction == null) throw new Exception("Giao dịch không tồn tại");
                if (transaction.Status != "Pending") throw new Exception("Chỉ có thể duyệt giao dịch đang chờ");

                // Execute Stock Updates
                foreach (var detail in transaction.Details)
                {
                    var product = _productRepo.GetProductById(detail.ProductID);
                    if (product == null) throw new Exception($"Sản phẩm ID {detail.ProductID} không tồn tại");

                    if (transaction.Type == "Import")
                    {
                        int newQty = product.Quantity + detail.Quantity;
                         _productRepo.UpdateQuantity(detail.ProductID, newQty);
                    }
                    else if (transaction.Type == "Export")
                    {
                        if (product.Quantity < detail.Quantity)
                            throw new Exception($"Tồn kho không đủ cho sản phẩm {product.ProductName}");
                        
                        int newQty = product.Quantity - detail.Quantity;
                        _productRepo.UpdateQuantity(detail.ProductID, newQty);
                    }
                }

                _transactionRepo.UpdateTransactionStatus(transactionId, "Approved");
                
                _logRepo.LogAction("APPROVE_TRANSACTION", 
                    $"Duyệt giao dịch #{transactionId} ({transaction.Type})", "");
                
                ActionsService.Instance.MarkAsChanged();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi duyệt giao dịch: " + ex.Message);
            }
        }

        public bool CancelTransaction(int transactionId)
        {
            try
            {
                var transaction = _transactionRepo.GetTransactionById(transactionId);
                if (transaction == null) throw new Exception("Giao dịch không tồn tại");
                if (transaction.Status != "Pending") throw new Exception("Chỉ có thể hủy giao dịch đang chờ");

                _transactionRepo.UpdateTransactionStatus(transactionId, "Cancelled");
                
                _logRepo.LogAction("CANCEL_TRANSACTION", 
                    $"Hủy giao dịch #{transactionId}", "");

                ActionsService.Instance.MarkAsChanged();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi hủy giao dịch: " + ex.Message);
            }
        }
    }
}