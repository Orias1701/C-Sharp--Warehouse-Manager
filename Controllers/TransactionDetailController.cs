using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến chi tiết phiếu Nhập/Xuất kho
    /// </summary>
    public class TransactionDetailController
    {
        private readonly TransactionDetailService _detailService;

        public TransactionDetailController()
        {
            _detailService = new TransactionDetailService();
        }

        /// <summary>
        /// Lấy danh sách chi tiết theo Transaction ID
        /// </summary>
        public List<TransactionDetail> GetDetailsByTransactionId(int transactionId)
        {
            return _detailService.GetDetailsByTransactionId(transactionId);
        }

        /// <summary>
        /// Lấy chi tiết theo ID
        /// </summary>
        public TransactionDetail GetDetailById(int detailId)
        {
            return _detailService.GetDetailById(detailId);
        }

        /// <summary>
        /// Thêm chi tiết vào phiếu
        /// </summary>
        public int AddTransactionDetail(int transactionId, int productId, string productName, int quantity, decimal unitPrice)
        {
            return _detailService.AddTransactionDetail(transactionId, productId, productName, quantity, unitPrice);
        }

        /// <summary>
        /// Cập nhật chi tiết phiếu
        /// </summary>
        public bool UpdateTransactionDetail(int detailId, int quantity, decimal unitPrice)
        {
            return _detailService.UpdateTransactionDetail(detailId, quantity, unitPrice);
        }

        /// <summary>
        /// Xóa chi tiết phiếu
        /// </summary>
        public bool DeleteTransactionDetail(int detailId)
        {
            return _detailService.DeleteTransactionDetail(detailId);
        }

        /// <summary>
        /// Xóa tất cả chi tiết của một phiếu
        /// </summary>
        public bool DeleteAllDetails(int transactionId)
        {
            return _detailService.DeleteAllDetails(transactionId);
        }

        /// <summary>
        /// Tính tổng số lượng trong phiếu
        /// </summary>
        public int GetTotalQuantity(int transactionId)
        {
            return _detailService.GetTotalQuantity(transactionId);
        }

        /// <summary>
        /// Tính tổng giá trị phiếu chi tiết
        /// </summary>
        public decimal GetTotalValue(int transactionId)
        {
            return _detailService.GetTotalValue(transactionId);
        }

        /// <summary>
        /// Đếm tổng số chi tiết trong phiếu
        /// </summary>
        public int CountDetails(int transactionId)
        {
            return _detailService.CountDetails(transactionId);
        }
    }
}