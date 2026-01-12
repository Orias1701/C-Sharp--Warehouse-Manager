using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using WarehouseManagement.Models;

namespace WarehouseManagement.Repositories
{
    /// <summary>
    /// Repository để quản lý nhật ký hành động (hỗ trợ Undo)
    /// </summary>
    public class ActionLogRepository : BaseRepository
    {
        /// <summary>
        /// Lấy danh sách nhật ký (chỉ visible records)
        /// </summary>
        public List<ActionLog> GetAllLogs()
        {
            var logs = new List<ActionLog>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM ActionLogs WHERE Visible=TRUE ORDER BY CreatedAt DESC", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new ActionLog
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhật ký: " + ex.Message);
            }
            return logs;
        }

        /// <summary>
        /// Lấy nhật ký theo ID
        /// </summary>
        public ActionLog GetLogById(int logId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM ActionLogs WHERE LogID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", logId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ActionLog
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy nhật ký ID {logId}: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Thêm nhật ký hành động mới
        /// </summary>
        public int LogAction(ActionLog log)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO ActionLogs (ActionType, Descriptions, DataBefore, CreatedAt, Visible) " +
                        "VALUES (@type, @desc, @dataBefore, @createdAt, TRUE); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", log.ActionType);
                        cmd.Parameters.AddWithValue("@desc", log.Descriptions ?? "");
                        
                        // Xử lý DataBefore - nếu rỗng hoặc không hợp lệ JSON, lưu NULL hoặc "{}"
                        string dataBefore = log.DataBefore ?? "";
                        if (string.IsNullOrWhiteSpace(dataBefore) || dataBefore.Trim() == "")
                        {
                            dataBefore = "{}";
                        }
                        cmd.Parameters.AddWithValue("@dataBefore", dataBefore);
                        
                        cmd.Parameters.AddWithValue("@createdAt", log.CreatedAt);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi ghi nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Thêm nhật ký hành động mới (overload với parameters)
        /// </summary>
        public int LogAction(string actionType, string descriptions, string dataBefore = "")
        {
            var log = new ActionLog
            {
                ActionType = actionType,
                Descriptions = descriptions,
                DataBefore = dataBefore,
                CreatedAt = DateTime.Now
            };
            return LogAction(log);
        }

        /// <summary>
        /// Xóa nhật ký (soft delete)
        /// </summary>
        public bool DeleteLog(int logId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE ActionLogs SET Visible=FALSE WHERE LogID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", logId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
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
            var logs = new List<ActionLog>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT * FROM ActionLogs WHERE ActionType=@type AND Visible=TRUE ORDER BY CreatedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", actionType);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new ActionLog
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký theo loại: " + ex.Message);
            }
            return logs;
        }

        /// <summary>
        /// Lấy nhật ký trong một khoảng thời gian
        /// </summary>
        public List<ActionLog> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            var logs = new List<ActionLog>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT * FROM ActionLogs WHERE CreatedAt BETWEEN @start AND @end AND Visible=TRUE ORDER BY CreatedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@start", startDate);
                        cmd.Parameters.AddWithValue("@end", endDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new ActionLog
                                {
                                    LogID = reader.GetInt32("LogID"),
                                    ActionType = reader.GetString("ActionType"),
                                    Descriptions = reader.IsDBNull(reader.GetOrdinal("Descriptions")) ? "" : reader.GetString("Descriptions"),
                                    DataBefore = reader.IsDBNull(reader.GetOrdinal("DataBefore")) ? "" : reader.GetString("DataBefore"),
                                    CreatedAt = reader.GetDateTime("CreatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy nhật ký theo ngày: " + ex.Message);
            }
            return logs;
        }
    }
}
