using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến nhật ký hành động
    /// </summary>
    public class ActionsController
    {
        private readonly ActionsService _logService;

        public ActionsController()
        {
            _logService = ActionsService.Instance;
        }

        /// <summary>
        /// Lấy danh sách tất cả nhật ký
        /// </summary>
        public List<Actions> GetAllLogs()
        {
            return _logService.GetAllLogs();
        }

        /// <summary>
        /// Lấy nhật ký theo ID
        /// </summary>
        public Actions GetLogById(int logId)
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
        public List<Actions> GetLogsByActionType(string actionType)
        {
            return _logService.GetLogsByActionType(actionType);
        }

        /// <summary>
        /// Lấy nhật ký trong một khoảng thời gian
        /// </summary>
        public List<Actions> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _logService.GetLogsByDateRange(startDate, endDate);
        }

        /// <summary>
        /// Lấy nhật ký gần nhất của một loại hành động
        /// </summary>
        public Actions GetLatestLog(string actionType)
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

        /// <summary>
        /// Xóa tất cả nhật ký khi kết thúc phiên
        /// </summary>
        public bool ClearAllLogs()
        {
            return _logService.ClearAllLogs();
        }
    }
}