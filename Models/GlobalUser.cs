namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lưu thông tin user hiện tại
    /// </summary>
    public static class GlobalUser
    {
        public static User CurrentUser { get; set; }
    }
}