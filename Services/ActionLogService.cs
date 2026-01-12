using System;
using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic nhật ký hành động (hỗ trợ Undo)
    /// 
    /// CHỨC NĂNG:
    /// - Quản lý nhật ký (CRUD): Thêm, xem, xóa
    /// - Tìm kiếm nhật ký: Theo loại hành động, ngày tháng
    /// - Undo: Khôi phục dữ liệu trước khi thay đổi
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra đầu vào
    /// 2. Repository call: Gọi DB để thực hiện thao tác
    /// 3. Return: Trả về kết quả
    /// </summary>
    public class ActionLogService
    {
        private readonly ActionLogRepository _logRepo;

        public ActionLogService()
        {
            _logRepo = new ActionLogRepository();
        }

        /// <summary>
        /// Lấy danh sách tất cả nhật ký
        /// </summary>
        public List<ActionLog> GetAllLogs()
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
        public ActionLog GetLogById(int logId)
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

                var log = new ActionLog
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
        public List<ActionLog> GetLogsByActionType(string actionType)
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
        public List<ActionLog> GetLogsByDateRange(DateTime startDate, DateTime endDate)
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
        public ActionLog GetLatestLog(string actionType)
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
    }
}
