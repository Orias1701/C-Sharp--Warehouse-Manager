namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lớp thực thể Nhà cung cấp
    /// </summary>
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public bool Visible { get; set; } = true;
    }
}
