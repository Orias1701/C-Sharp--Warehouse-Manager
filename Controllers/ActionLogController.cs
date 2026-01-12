using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến nhật ký hành động
    /// </summary>
    public class ActionLogController
    {
        private readonly ActionLogService _logService;

        public ActionLogController()
        {
            _logService = new ActionLogService();
        }

        /// <summary>
        /// Lấy danh sách tất cả nhật ký
        /// </summary>
        public List<ActionLog> GetAllLogs()
        {
            return _logService.GetAllLogs();
        }

        /// <summary>
        /// Lấy nhật ký theo ID
        /// </summary>
        public ActionLog GetLogById(int logId)
        {
            return _logService.GetLogById(logId);
        }

        /// <summary>
        /// Ghi nhật ký hành động mới
        /// </summary>
        public int LogAction(string actionType, string descriptions, string dataBefore = "")
        {
            return _logService.LogAction(actionType, descriptions, dataBefore);
        }

        /// <summary>
        /// Xóa nhật ký
        /// </summary>
        public bool DeleteLog(int logId)
        {
            return _logService.DeleteLog(logId);
        }

        /// <summary>
        /// Lấy nhật ký theo loại hành động
        /// </summary>
        public List<ActionLog> GetLogsByActionType(string actionType)
        {
            return _logService.GetLogsByActionType(actionType);
        }

        /// <summary>
        /// Lấy nhật ký trong một khoảng thời gian
        /// </summary>
        public List<ActionLog> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _logService.GetLogsByDateRange(startDate, endDate);
        }

        /// <summary>
        /// Lấy nhật ký gần nhất của một loại hành động
        /// </summary>
        public ActionLog GetLatestLog(string actionType)
        {
            return _logService.GetLatestLog(actionType);
        }

        /// <summary>
        /// Kiểm tra có nhật ký nào không
        /// </summary>
        public bool HasLogs()
        {
            return _logService.HasLogs();
        }

        /// <summary>
        /// Đếm tổng số nhật ký
        /// </summary>
        public int CountLogs()
        {
            return _logService.CountLogs();
        }
    }
}
