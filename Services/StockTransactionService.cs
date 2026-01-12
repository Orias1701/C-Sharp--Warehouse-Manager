using System;
using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;
using Newtonsoft.Json;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic phiếu Nhập/Xuất kho
    /// 
    /// CHỨC NĂNG:
    /// - Quản lý phiếu (CRUD): Thêm, sửa, xóa
    /// - Tìm kiếm phiếu: Theo loại, ngày tháng
    /// - Tính toán: Tính tổng giá trị, tổng số lượng
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra đầu vào
    /// 2. Repository call: Gọi DB để thực hiện thao tác
    /// 3. Logging: Ghi nhật ký ActionLogs
    /// 4. Change tracking: Gọi SaveManager.MarkAsChanged()
    /// 5. Return: Trả về kết quả
    /// </summary>
    public class StockTransactionService
    {
        private readonly StockTransactionRepository _transactionRepo;
        private readonly ActionLogRepository _logRepo;

        public StockTransactionService()
        {
            _transactionRepo = new StockTransactionRepository();
            _logRepo = new ActionLogRepository();
        }

        /// <summary>
        /// Lấy danh sách tất cả phiếu
        /// </summary>
        public List<StockTransaction> GetAllTransactions()
        {
            try
            {
                return _transactionRepo.GetAllTransactions();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy phiếu theo ID (bao gồm chi tiết)
        /// </summary>
        public StockTransaction GetTransactionById(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");
                return _transactionRepo.GetTransactionById(transactionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy phiếu ID {transactionId}: " + ex.Message);
            }
        }

        /// <summary>
        /// Tạo phiếu nhập/xuất mới
        /// </summary>
        public int CreateTransaction(string type, string note = "")
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(type))
                    throw new ArgumentException("Loại phiếu không được trống");
                if (type != "Import" && type != "Export")
                    throw new ArgumentException("Loại phiếu phải là Import hoặc Export");

                var transaction = new StockTransaction
                {
                    Type = type.Trim(),
                    DateCreated = DateTime.Now,
                    CreatedByUserID = GlobalUser.CurrentUser?.UserID ?? 0,
                    Note = note ?? ""
                };

                int transId = _transactionRepo.CreateTransaction(transaction);

                // Ghi nhật ký
                var log = new ActionLog
                {
                    ActionType = "CREATE_TRANSACTION",
                    Descriptions = $"Tạo phiếu {type}: {note}",
                    DataBefore = "",
                    CreatedAt = DateTime.Now
                };
                _logRepo.LogAction(log);

                SaveManager.Instance.MarkAsChanged();
                return transId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật phiếu
        /// </summary>
        public bool UpdateTransaction(int transactionId, string type, string note)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");
                if (string.IsNullOrWhiteSpace(type))
                    throw new ArgumentException("Loại phiếu không được trống");
                if (type != "Import" && type != "Export")
                    throw new ArgumentException("Loại phiếu phải là Import hoặc Export");

                var oldTransaction = _transactionRepo.GetTransactionById(transactionId);
                if (oldTransaction == null)
                    throw new ArgumentException("Phiếu không tồn tại");

                var beforeData = new
                {
                    TransactionID = oldTransaction.TransactionID,
                    Type = oldTransaction.Type,
                    Note = oldTransaction.Note
                };

                var transaction = new StockTransaction
                {
                    TransactionID = transactionId,
                    Type = type.Trim(),
                    Note = note ?? ""
                };

                bool result = _transactionRepo.UpdateTransaction(transaction);

                if (result)
                {
                    var log = new ActionLog
                    {
                        ActionType = "UPDATE_TRANSACTION",
                        Descriptions = $"Cập nhật phiếu ID {transactionId}",
                        DataBefore = JsonConvert.SerializeObject(beforeData),
                        CreatedAt = DateTime.Now
                    };
                    _logRepo.LogAction(log);

                    SaveManager.Instance.MarkAsChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa phiếu
        /// </summary>
        public bool DeleteTransaction(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");

                var transaction = _transactionRepo.GetTransactionById(transactionId);
                if (transaction == null)
                    throw new ArgumentException("Phiếu không tồn tại");

                var beforeData = new
                {
                    TransactionID = transaction.TransactionID,
                    Type = transaction.Type,
                    DateCreated = transaction.DateCreated,
                    Note = transaction.Note,
                    DetailCount = transaction.Details.Count
                };

                bool result = _transactionRepo.DeleteTransaction(transactionId);

                if (result)
                {
                    var log = new ActionLog
                    {
                        ActionType = "DELETE_TRANSACTION",
                        Descriptions = $"Xóa phiếu ID {transactionId}",
                        DataBefore = JsonConvert.SerializeObject(beforeData),
                        CreatedAt = DateTime.Now
                    };
                    _logRepo.LogAction(log);

                    SaveManager.Instance.MarkAsChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách phiếu theo loại (Import/Export)
        /// </summary>
        public List<StockTransaction> GetTransactionsByType(string type)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                    throw new ArgumentException("Loại phiếu không được trống");
                if (type != "Import" && type != "Export")
                    throw new ArgumentException("Loại phiếu phải là Import hoặc Export");

                return _transactionRepo.GetTransactionsByType(type.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy phiếu theo loại: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách phiếu trong một khoảng thời gian
        /// </summary>
        public List<StockTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc");

                return _transactionRepo.GetTransactionsByDateRange(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy phiếu theo ngày: " + ex.Message);
            }
        }

        /// <summary>
        /// Tính tổng giá trị một phiếu
        /// </summary>
        public decimal GetTransactionTotalValue(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");

                var transaction = _transactionRepo.GetTransactionById(transactionId);
                if (transaction == null)
                    throw new ArgumentException("Phiếu không tồn tại");

                decimal total = 0;
                foreach (var detail in transaction.Details)
                {
                    total += detail.Quantity * detail.UnitPrice;
                }
                return total;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tính tổng giá trị phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Đếm tổng số phiếu
        /// </summary>
        public int CountTransactions()
        {
            try
            {
                return _transactionRepo.GetAllTransactions().Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đếm phiếu: " + ex.Message);
            }
        }
    }
}
