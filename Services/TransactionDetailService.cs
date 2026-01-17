using System;
using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;
using Newtonsoft.Json;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic chi tiết phiếu Nhập/Xuất kho
    /// 
    /// CHỨC NĂNG:
    /// - Quản lý chi tiết phiếu (CRUD): Thêm, sửa, xóa
    /// - Tìm kiếm chi tiết: Theo phiếu, sản phẩm
    /// - Tính toán: Tính tổng số lượng, tổng giá trị
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra đầu vào
    /// 2. Repository call: Gọi DB để thực hiện thao tác
    /// 3. Logging: Ghi nhật ký Actions
    /// 4. Change tracking: Gọi ActionsService.MarkAsChanged()
    /// 5. Return: Trả về kết quả
    /// </summary>
    public class TransactionDetailService
    {
        private readonly TransactionDetailRepository _detailRepo;
        private readonly ActionsRepository _logRepo;

        public TransactionDetailService()
        {
            _detailRepo = new TransactionDetailRepository();
            _logRepo = new ActionsRepository();
        }

        /// <summary>
        /// Lấy danh sách chi tiết theo Transaction ID
        /// </summary>
        public List<TransactionDetail> GetDetailsByTransactionId(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");
                return _detailRepo.GetDetailsByTransactionId(transactionId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy chi tiết theo ID
        /// </summary>
        public TransactionDetail GetDetailById(int detailId)
        {
            try
            {
                if (detailId <= 0)
                    throw new ArgumentException("ID chi tiết không hợp lệ");
                return _detailRepo.GetDetailById(detailId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy chi tiết: " + ex.Message);
            }
        }

        /// <summary>
        /// Thêm chi tiết vào phiếu
        /// </summary>
        public int AddTransactionDetail(int transactionId, int productId, string productName, int quantity, decimal unitPrice)
        {
            try
            {
                // Validation
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");
                if (productId <= 0)
                    throw new ArgumentException("ID sản phẩm không hợp lệ");
                if (quantity <= 0)
                    throw new ArgumentException("Số lượng phải lớn hơn 0");
                if (quantity > 999999)
                    throw new ArgumentException("Số lượng quá lớn");
                if (unitPrice < 0)
                    throw new ArgumentException("Đơn giá không được âm");
                if (unitPrice > 999999999)
                    throw new ArgumentException("Đơn giá quá lớn");
                if (string.IsNullOrWhiteSpace(productName))
                    throw new ArgumentException("Tên sản phẩm không được trống");

                var detail = new TransactionDetail
                {
                    TransactionID = transactionId,
                    ProductID = productId,
                    ProductName = productName.Trim(),
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };

                int detailId = _detailRepo.AddTransactionDetail(detail);

                // Ghi nhật ký
                var log = new Actions
                {
                    ActionType = "ADD_DETAIL",
                    Descriptions = $"Thêm chi tiết sản phẩm ID {productId} vào phiếu ID {transactionId}",
                    DataBefore = "",
                    CreatedAt = DateTime.Now
                };
                _logRepo.LogAction(log);

                ActionsService.Instance.MarkAsChanged();
                return detailId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật chi tiết phiếu
        /// </summary>
        public bool UpdateTransactionDetail(int detailId, int quantity, decimal unitPrice)
        {
            try
            {
                if (detailId <= 0)
                    throw new ArgumentException("ID chi tiết không hợp lệ");
                if (quantity <= 0)
                    throw new ArgumentException("Số lượng phải lớn hơn 0");
                if (quantity > 999999)
                    throw new ArgumentException("Số lượng quá lớn");
                if (unitPrice < 0)
                    throw new ArgumentException("Đơn giá không được âm");
                if (unitPrice > 999999999)
                    throw new ArgumentException("Đơn giá quá lớn");

                var oldDetail = _detailRepo.GetDetailById(detailId);
                if (oldDetail == null)
                    throw new ArgumentException("Chi tiết không tồn tại");

                var beforeData = new
                {
                    DetailID = oldDetail.DetailID,
                    Quantity = oldDetail.Quantity,
                    UnitPrice = oldDetail.UnitPrice
                };

                var detail = new TransactionDetail
                {
                    DetailID = detailId,
                    TransactionID = oldDetail.TransactionID,
                    ProductID = oldDetail.ProductID,
                    ProductName = oldDetail.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };

                bool result = _detailRepo.UpdateTransactionDetail(detail);

                if (result)
                {
                    var log = new Actions
                    {
                        ActionType = "UPDATE_DETAIL",
                        Descriptions = $"Cập nhật chi tiết ID {detailId}",
                        DataBefore = JsonConvert.SerializeObject(beforeData),
                        CreatedAt = DateTime.Now
                    };
                    _logRepo.LogAction(log);

                    ActionsService.Instance.MarkAsChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa chi tiết phiếu
        /// </summary>
        public bool DeleteTransactionDetail(int detailId)
        {
            try
            {
                if (detailId <= 0)
                    throw new ArgumentException("ID chi tiết không hợp lệ");

                var detail = _detailRepo.GetDetailById(detailId);
                if (detail == null)
                    throw new ArgumentException("Chi tiết không tồn tại");

                var beforeData = new
                {
                    DetailID = detail.DetailID,
                    TransactionID = detail.TransactionID,
                    ProductID = detail.ProductID,
                    ProductName = detail.ProductName,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice
                };

                bool result = _detailRepo.DeleteTransactionDetail(detailId);

                if (result)
                {
                    var log = new Actions
                    {
                        ActionType = "DELETE_DETAIL",
                        Descriptions = $"Xóa chi tiết ID {detailId}",
                        DataBefore = JsonConvert.SerializeObject(beforeData),
                        CreatedAt = DateTime.Now
                    };
                    _logRepo.LogAction(log);

                    ActionsService.Instance.MarkAsChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa tất cả chi tiết của một phiếu
        /// </summary>
        public bool DeleteAllDetails(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");

                bool result = _detailRepo.DeleteAllDetails(transactionId);

                if (result)
                {
                    var log = new Actions
                    {
                        ActionType = "DELETE_ALL_DETAILS",
                        Descriptions = $"Xóa tất cả chi tiết của phiếu ID {transactionId}",
                        DataBefore = "",
                        CreatedAt = DateTime.Now
                    };
                    _logRepo.LogAction(log);

                    ActionsService.Instance.MarkAsChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa chi tiết phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Tính tổng số lượng trong phiếu
        /// </summary>
        public int GetTotalQuantity(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");

                return _detailRepo.GetTotalQuantity(transactionId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tính tổng số lượng: " + ex.Message);
            }
        }

        /// <summary>
        /// Tính tổng giá trị phiếu chi tiết
        /// </summary>
        public decimal GetTotalValue(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");

                var details = _detailRepo.GetDetailsByTransactionId(transactionId);
                decimal total = 0;
                foreach (var detail in details)
                {
                    total += detail.Quantity * detail.UnitPrice;
                }
                return total;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tính tổng giá trị: " + ex.Message);
            }
        }

        /// <summary>
        /// Đếm tổng số chi tiết trong phiếu
        /// </summary>
        public int CountDetails(int transactionId)
        {
            try
            {
                if (transactionId <= 0)
                    throw new ArgumentException("ID phiếu không hợp lệ");

                return _detailRepo.GetDetailsByTransactionId(transactionId).Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đếm chi tiết: " + ex.Message);
            }
        }
    }
}