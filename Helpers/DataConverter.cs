using System;
using Newtonsoft.Json;

namespace WarehouseManagement.Helpers
{
    /// <summary>
    /// Helper chuyển đổi dữ liệu và định dạng tiền tệ
    /// </summary>
    public class DataConverter
    {
        /// <summary>
        /// Chuyển đổi số thành định dạng tiền tệ VNĐ
        /// </summary>
        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("N0") + " ₫";
        }

        /// <summary>
        /// Chuyển đổi chuỗi JSON thành object
        /// </summary>
        public static T DeserializeJson<T>(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return default(T);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Chuyển đổi object thành JSON
        /// </summary>
        public static string SerializeJson<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Định dạng ngày tháng
        /// </summary>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Định dạng ngày tháng ngắn
        /// </summary>
        public static string FormatDateShort(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Định dạng số lượng (với dấu phân cách hàng nghìn)
        /// </summary>
        public static string FormatQuantity(int quantity)
        {
            return quantity.ToString("N0");
        }

        /// <summary>
        /// Kiểm tra xem chuỗi có phải JSON hợp lệ không
        /// </summary>
        public static bool IsValidJson(string input)
        {
            try
            {
                JsonConvert.DeserializeObject(input);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}