using System;
using WarehouseManagement.Helpers;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// Lớp thực thể Người dùng
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  // Lưu ý: Nên lưu dạng hash SHA256 trong cơ sở dữ liệu
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kiểm tra xem user có phải Admin hay không
        /// </summary>
        public bool IsAdmin => Role == "Admin";

        /// <summary>
        /// Hash mật khẩu bằng SHA256 trước khi lưu vào database
        /// </summary>
        public string HashPassword(string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword))
                throw new ArgumentNullException(nameof(plainPassword));
            
            return IdGenerator.GenerateSHA256Hash(plainPassword);
        }

        /// <summary>
        /// Xác minh mật khẩu người dùng nhập với hash đã lưu
        /// </summary>
        public bool VerifyPassword(string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(this.Password))
                return false;

            return IdGenerator.VerifySHA256Hash(plainPassword, this.Password);
        }

        /// <summary>
        /// Thiết lập mật khẩu mới (tự động hash)
        /// </summary>
        public void SetPassword(string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword))
                throw new ArgumentNullException(nameof(plainPassword));
            this.Password = HashPassword(plainPassword);
        }
    }
}