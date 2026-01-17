using System;
using System.Collections.Generic;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lớp thực thể Phiếu Nhập/Xuất kho
    /// </summary>
    public class StockTransaction
    {
        public int TransactionID { get; set; }
        public string Type { get; set; } // 'Import' hoặc 'Export'
        public DateTime DateCreated { get; set; }
        public int CreatedByUserID { get; set; } // ID người tạo phiếu
        public string Note { get; set; }
        public decimal TotalValue { get; set; } // Tổng giá trị của đơn hàng

        /// <summary>
        /// Danh sách chi tiết sản phẩm trong phiếu
        /// </summary>
        public List<TransactionDetail> Details { get; set; } = new List<TransactionDetail>();

        /// <summary>
        /// Kiểm tra phiếu là phiếu nhập hay xuất
        /// </summary>
        public bool IsImport => Type == "Import";
    }
}