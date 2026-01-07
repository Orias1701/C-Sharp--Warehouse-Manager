using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến kho (Nhập/Xuất)
    /// </summary>
    public class InventoryController
    {
        private readonly InventoryService _inventoryService;

        public InventoryController()
        {
            _inventoryService = new InventoryService();
        }

        /// <summary>
        /// Thực hiện phiếu nhập kho
        /// </summary>
        public bool Import(int productId, int quantity, decimal unitPrice, string note = "")
        {
            return _inventoryService.ImportStock(productId, quantity, unitPrice, note);
        }

        /// <summary>
        /// Thực hiện phiếu xuất kho
        /// </summary>
        public bool Export(int productId, int quantity, decimal unitPrice, string note = "")
        {
            return _inventoryService.ExportStock(productId, quantity, unitPrice, note);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm cảnh báo (tồn kho thấp)
        /// </summary>
        public List<Product> GetLowStockProducts()
        {
            return _inventoryService.GetLowStockProducts();
        }

        /// <summary>
        /// Tính tổng giá trị tồn kho
        /// </summary>
        public decimal GetTotalInventoryValue()
        {
            return _inventoryService.GetTotalInventoryValue();
        }

        /// <summary>
        /// Hoàn tác thao tác cuối cùng
        /// </summary>
        public bool UndoLastAction()
        {
            return _inventoryService.UndoLastAction();
        }

        /// <summary>
        /// Lấy danh sách tất cả giao dịch
        /// </summary>
        public List<StockTransaction> GetAllTransactions()
        {
            return _inventoryService.GetAllTransactions();
        }

        /// <summary>
        /// Lấy danh sách nhật ký hành động
        /// </summary>
        public List<ActionLog> GetAllLogs()
        {
            return _inventoryService.GetAllLogs();
        }
    }
}
