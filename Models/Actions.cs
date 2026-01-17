using System;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lớp thực thể Nhật ký hành động (Hỗ trợ Undo)
    /// </summary>
    public class Actions
    {
        public int LogID { get; set; }
        public string ActionType { get; set; } // 'UPDATE_STOCK', 'CREATE_TRANSACTION', etc.
        public string Descriptions { get; set; }
        public string DataBefore { get; set; } // JSON
        public DateTime CreatedAt { get; set; }
    }
}