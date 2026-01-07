namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lớp thực thể Chi tiết phiếu Nhập/Xuất
    /// </summary>
    public class TransactionDetail
    {
        public int DetailID { get; set; }
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } // Snapshot của tên sản phẩm
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Tính tổng giá trị của dòng chi tiết
        /// </summary>
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
