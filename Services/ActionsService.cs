using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic nhật ký hành động (hỗ trợ Undo) + Quản lý Save/Commit
    /// 
    /// CHỨC NĂNG:
    /// - Quản lý nhật ký (CRUD): Thêm, xem, xóa
    /// - Tìm kiếm nhật ký: Theo loại hành động, ngày tháng
    /// - Undo: Khôi phục dữ liệu trước khi thay đổi
    /// - Save state tracking: Đánh dấu có thay đổi chưa lưu
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra đầu vào
    /// 2. Repository call: Gọi DB để thực hiện thao tác
    /// 3. Return: Trả về kết quả
    /// </summary>
    public class ActionsService
    {
        private readonly ActionsRepository _logRepo;
        
        // Save state tracking (merged from SaveManager)
        private bool _hasUnsavedChanges = false;
        private DateTime _lastSaveTime = DateTime.Now;
        private int _changeCount = 0;

        // Singleton pattern
        private static ActionsService _instance;

        public static ActionsService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ActionsService();
                return _instance;
            }
        }

        private ActionsService()
        {
            _logRepo = new ActionsRepository();
            _lastSaveTime = DateTime.Now;
        }

        #region Action Logging Methods

        /// <summary>
        /// Lấy danh sách tất cả nhật ký
        /// </summary>
        public List<Actions> GetAllLogs()
        {
            try
            {
                return _logRepo.GetAllLogs();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy nhật ký theo ID
        /// </summary>
        public Actions GetLogById(int logId)
        {
            try
            {
                if (logId <= 0)
                    throw new ArgumentException("ID nhật ký không hợp lệ");
                return _logRepo.GetLogById(logId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Ghi nhật ký hành động mới
        /// </summary>
        public int LogAction(string actionType, string descriptions, string dataBefore = "")
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(actionType))
                    throw new ArgumentException("Loại hành động không được trống");

                var log = new Actions
                {
                    ActionType = actionType.Trim(),
                    Descriptions = descriptions ?? "",
                    DataBefore = dataBefore ?? "",
                    CreatedAt = DateTime.Now
                };

                return _logRepo.LogAction(log);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi ghi nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa nhật ký (soft delete)
        /// </summary>
        public bool DeleteLog(int logId)
        {
            try
            {
                if (logId <= 0)
                    throw new ArgumentException("ID nhật ký không hợp lệ");

                return _logRepo.DeleteLog(logId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy nhật ký theo loại hành động
        /// </summary>
        public List<Actions> GetLogsByActionType(string actionType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionType))
                    throw new ArgumentException("Loại hành động không được trống");

                return _logRepo.GetLogsByActionType(actionType.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký theo loại: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy nhật ký trong một khoảng thời gian
        /// </summary>
        public List<Actions> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc");

                return _logRepo.GetLogsByDateRange(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký theo ngày: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy nhật ký gần nhất của một loại hành động
        /// </summary>
        public Actions GetLatestLog(string actionType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionType))
                    throw new ArgumentException("Loại hành động không được trống");

                var logs = _logRepo.GetLogsByActionType(actionType.Trim());
                if (logs.Count > 0)
                    return logs[0]; // Mới nhất được sort first
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký gần nhất: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra có nhật ký nào không
        /// </summary>
        public bool HasLogs()
        {
            try
            {
                return _logRepo.GetAllLogs().Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi kiểm tra nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Đếm tổng số nhật ký
        /// </summary>
        public int CountLogs()
        {
            try
            {
                return _logRepo.GetAllLogs().Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đếm nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa tất cả nhật ký khi kết thúc phiên
        /// </summary>
        public bool ClearAllLogs()
        {
            try
            {
                return _logRepo.ClearAllLogs();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa tất cả nhật ký: " + ex.Message);
            }
        }

        #endregion

        #region Save State Management (merged from SaveManager)

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
        /// Giảm số lượng thay đổi khi hoàn tác hành động
        /// Được gọi từ Undo functionality
        /// </summary>
        public void DecrementChangeCount()
        {
            if (_changeCount > 0)
            {
                _changeCount--;
            }
            
            // Nếu không còn thay đổi nào, reset trạng thái
            if (_changeCount == 0)
            {
                _hasUnsavedChanges = false;
            }
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
        /// 2. Đã được ghi vào Actions với CreatedAt = now
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
        /// 1. Truy vấn Actions
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
                        "UPDATE Actions SET Visible=FALSE " +
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
        /// 2. Set Visible=FALSE cho tất cả Actions (trừ UNDO_ACTION)
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
                        "UPDATE Actions SET Visible=FALSE WHERE ActionType != 'UNDO_ACTION'", 
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
        /// Reset trạng thái ActionsService
        /// Sử dụng khi app khởi động lại hoặc cần reset toàn bộ
        /// </summary>
        public void Reset()
        {
            _hasUnsavedChanges = false;
            _changeCount = 0;
            _lastSaveTime = DateTime.Now;
        }

        #endregion
    }
}