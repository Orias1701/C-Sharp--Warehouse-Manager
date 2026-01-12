using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến phiếu Nhập/Xuất kho
    /// </summary>
    public class StockTransactionController
    {
        private readonly StockTransactionService _transactionService;

        public StockTransactionController()
        {
            _transactionService = new StockTransactionService();
        }

        /// <summary>
        /// Lấy danh sách tất cả phiếu
        /// </summary>
        public List<StockTransaction> GetAllTransactions()
        {
            return _transactionService.GetAllTransactions();
        }

        /// <summary>
        /// Lấy phiếu theo ID (bao gồm chi tiết)
        /// </summary>
        public StockTransaction GetTransactionById(int transactionId)
        {
            return _transactionService.GetTransactionById(transactionId);
        }

        /// <summary>
        /// Tạo phiếu nhập/xuất mới
        /// </summary>
        public int CreateTransaction(string type, string note = "")
        {
            return _transactionService.CreateTransaction(type, note);
        }

        /// <summary>
        /// Cập nhật phiếu
        /// </summary>
        public bool UpdateTransaction(int transactionId, string type, string note)
        {
            return _transactionService.UpdateTransaction(transactionId, type, note);
        }

        /// <summary>
        /// Xóa phiếu
        /// </summary>
        public bool DeleteTransaction(int transactionId)
        {
            return _transactionService.DeleteTransaction(transactionId);
        }

        /// <summary>
        /// Lấy danh sách phiếu theo loại (Import/Export)
        /// </summary>
        public List<StockTransaction> GetTransactionsByType(string type)
        {
            return _transactionService.GetTransactionsByType(type);
        }

        /// <summary>
        /// Lấy danh sách phiếu trong một khoảng thời gian
        /// </summary>
        public List<StockTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _transactionService.GetTransactionsByDateRange(startDate, endDate);
        }

        /// <summary>
        /// Tính tổng giá trị một phiếu
        /// </summary>
        public decimal GetTransactionTotalValue(int transactionId)
        {
            return _transactionService.GetTransactionTotalValue(transactionId);
        }

        /// <summary>
        /// Đếm tổng số phiếu
        /// </summary>
        public int CountTransactions()
        {
            return _transactionService.CountTransactions();
        }
    }
}
