using System;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lớp thực thể Người dùng
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kiểm tra xem user có phải Admin hay không
        /// </summary>
        public bool IsAdmin => Role == "Admin";
    }
}
