namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lớp thực thể Sản phẩm (Hàng hóa)
    /// </summary>
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int MinThreshold { get; set; }
        public decimal InventoryValue { get; set; } // Tổng giá trị tồn kho (Quantity * Price)

        /// <summary>
        /// Kiểm tra xem sản phẩm có cảnh báo tồn kho hay không
        /// </summary>
        public bool IsLowStock => Quantity < MinThreshold;
    }
}