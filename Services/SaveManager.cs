using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Quản lý trạng thái Save/Commit của ứng dụng
    /// 
    /// LUỒNG HOẠT ĐỘNG:
    /// 1. User thực hiện hành động (Thêm/Sửa/Xóa) 
    ///    → Service gọi SaveManager.MarkAsChanged()
    ///    → UI cập nhật thay đổi chưa lưu
    /// 
    /// 2. User click "Lưu" → CommitChanges()
    ///    → Cập nhật _lastSaveTime
    ///    → Reset trạng thái
    ///    → Database đã được cập nhật qua các Service
    /// 
    /// 3. User thoát app (có thay đổi chưa lưu)
    ///    → MainForm_FormClosing hỏi Yes/No/Cancel
    ///    → Nếu Yes: CommitChanges() rồi tắt
    ///    → Nếu No: RollbackChanges() rồi tắt
    ///    → Nếu Cancel: không tắt
    /// 
    /// RollbackChanges: Xóa tất cả log từ lần save cuối
    ///                  bằng cách set Visible=FALSE trong ActionLogs
    /// 
    /// ClearUndoStack: Xóa tất cả undo stack khi app đóng
    /// </summary>
    public class SaveManager
    {
        private bool _hasUnsavedChanges = false;
        private DateTime _lastSaveTime = DateTime.Now;
        private int _changeCount = 0;

        // Singleton pattern
        private static SaveManager _instance;

        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SaveManager();
                return _instance;
            }
        }

        private SaveManager()
        {
            _lastSaveTime = DateTime.Now;
        }

        /// <summary>
        /// Đánh dấu có thay đổi chưa lưu
        /// Được gọi từ các Service methods (AddProduct, ImportStock, v.v...)
        /// </summary>
        public void MarkAsChanged()
        {
            _hasUnsavedChanges = true;
            _changeCount++;
        }

        /// <summary>
        /// Kiểm tra có thay đổi chưa lưu hay không
        /// </summary>
        public bool HasUnsavedChanges => _hasUnsavedChanges;

        /// <summary>
        /// Lấy số lượng thay đổi từ lần save cuối cùng
        /// </summary>
        public int ChangeCount => _changeCount;

        /// <summary>
        /// Lấy thời gian Save cuối cùng
        /// </summary>
        public DateTime LastSaveTime => _lastSaveTime;

        /// <summary>
        /// Lưu các thay đổi vào database (CommitChanges)
        /// 
        /// LUỒNG:
        /// 1. Tất cả thay đổi đã được thực hiện qua các Service methods
        /// 2. Đã được ghi vào ActionLogs với CreatedAt = now
        /// 3. Chỉ cần update lại _lastSaveTime
        /// 4. Reset trạng thái HasUnsavedChanges và ChangeCount
        /// 
        /// Được gọi khi:
        /// - User click nút "Lưu" (💾)
        /// - User chọn "Có" (Yes) khi thoát app
        /// </summary>
        public void CommitChanges()
        {
            try
            {
                // Cập nhật thời gian save cuối cùng
                // Tất cả thay đổi từ lần save trước đến now đều đã được lưu
                _lastSaveTime = DateTime.Now;
                
                // Reset trạng thái
                _hasUnsavedChanges = false;
                _changeCount = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lưu thay đổi: " + ex.Message);
            }
        }

        /// <summary>
        /// Khôi phục tất cả thay đổi từ lần save cuối cùng
        /// 
        /// LUỒNG:
        /// 1. Truy vấn ActionLogs
        /// 2. Tìm tất cả hành động từ _lastSaveTime trở đi (CreatedAt >= _lastSaveTime)
        /// 3. Set Visible=FALSE để "ẩn" những hành động đó
        /// 4. Không xóa vật lý, chỉ ẩn để giữ nguyên tính lịch sử
        /// 
        /// Được gọi khi:
        /// - User chọn "Không" (No) khi thoát app
        /// - System cần revert các thay đổi chưa lưu
        /// </summary>
        public void RollbackChanges()
        {
            try
            {
                // Lấy connection string từ App.config
                string connString = ConfigurationManager.ConnectionStrings["WarehouseDB"].ConnectionString;

                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    
                    // Xóa (ẩn) tất cả hành động từ lần save cuối
                    // Loại trừ hành động Undo để không ảnh hưởng đến undo stack
                    using (var cmd = new MySqlCommand(
                        "UPDATE ActionLogs SET Visible=FALSE " +
                        "WHERE CreatedAt >= @lastSaveTime AND ActionType != 'UNDO_ACTION'", 
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@lastSaveTime", _lastSaveTime);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reset trạng thái
                _hasUnsavedChanges = false;
                _changeCount = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi khôi phục thay đổi: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa toàn bộ undo stack
        /// 
        /// LUỒNG:
        /// 1. Xóa tất cả hành động trong LIFO undo stack
        /// 2. Set Visible=FALSE cho tất cả ActionLogs (trừ UNDO_ACTION)
        /// 3. App sẽ khởi động lại với trạng thái sạch sẽ
        /// 
        /// Được gọi khi:
        /// - App sắp đóng (sau CommitChanges hoặc RollbackChanges)
        /// - Reset trạng thái toàn bộ
        /// </summary>
        public void ClearUndoStack()
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["WarehouseDB"].ConnectionString;

                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    
                    // Xóa (ẩn) tất cả undo stack entry
                    using (var cmd = new MySqlCommand(
                        "UPDATE ActionLogs SET Visible=FALSE WHERE ActionType != 'UNDO_ACTION'", 
                        conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa undo stack: " + ex.Message);
            }
        }

        /// <summary>
        /// Reset trạng thái SaveManager
        /// Sử dụng khi app khởi động lại hoặc cần reset toàn bộ
        /// </summary>
        public void Reset()
        {
            _hasUnsavedChanges = false;
            _changeCount = 0;
            _lastSaveTime = DateTime.Now;
        }
    }
}
