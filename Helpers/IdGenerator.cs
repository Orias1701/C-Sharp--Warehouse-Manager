using System;
using System.Security.Cryptography;
using System.Text;

namespace WarehouseManagement.Helpers
{
    /// <summary>
    /// Lớp tiện ích để tạo các ID khác nhau cho hệ thống
    /// </summary>
    public static class IdGenerator
    {
        /// <summary>
        /// Tạo một ID dạng hex từ timestamp và random bytes
        /// Format: YYYYMMDD-HHMMSS-XXXXXXXXXXX (8-6-11 ký tự hex)
        /// Example: 20250109-143025-A1B2C3D4E5F
        /// </summary>
        public static string GenerateHexId()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                
                // Tạo 6 bytes random (12 ký tự hex)
                byte[] randomBytes = new byte[6];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(randomBytes);
                }
                
                string randomHex = BitConverter.ToString(randomBytes).Replace("-", "");
                string hexId = $"{timestamp}-{randomHex}";
                return hexId;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo hex ID: " + ex.Message);
            }
        }

        /// <summary>
        /// Tạo một ID dạng UUID v4
        /// </summary>
        public static string GenerateUUID()
        {
            try
            {
                string uuid = Guid.NewGuid().ToString();
                return uuid;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo UUID: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra xem chuỗi ID có phải hex ID hợp lệ hay không
        /// </summary>
        public static bool IsValidHexId(string hexId)
        {
            try
            {
                if (string.IsNullOrEmpty(hexId))
                    return false;

                // Kiểm tra format: YYYYMMDD-HHMMSS-XXXXXXXXXXX
                if (hexId.Length != 27) // 8 + 1 + 6 + 1 + 11
                    return false;

                if (hexId[8] != '-' || hexId[15] != '-')
                    return false;

                // Kiểm tra phần timestamp có phải hex không
                string timestampPart = hexId.Substring(0, 8); // YYYYMMDD
                if (!int.TryParse(timestampPart, out _))
                    return false;

                string timePart = hexId.Substring(9, 6); // HHMMSS
                if (!int.TryParse(timePart, out _))
                    return false;

                // Kiểm tra phần random có phải hex không
                string randomPart = hexId.Substring(16, 11);
                try
                {
                    Convert.ToInt32(randomPart, 16);
                }
                catch
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Tạo một hash SHA-256 từ chuỗi input (Base64 format)
        /// </summary>
        public static string GenerateSHA256Hash(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                    throw new ArgumentNullException(nameof(input));

                using (var sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                    // Use HEX format (lowercase) for compatibility with database
                    string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                    return hash;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo SHA256 hash: " + ex.Message);
            }
        }

        /// <summary>
        /// Xác minh chuỗi input với hash đã lưu
        /// </summary>
        public static bool VerifySHA256Hash(string input, string hash)
        {
            try
            {
                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(hash))
                    return false;

                string inputHash = GenerateSHA256Hash(input);
                bool isMatch = inputHash == hash;
                return isMatch;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}