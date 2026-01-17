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
    public class LogRepository : BaseRepository
    {
        /// <summary>
        /// Lấy danh sách nhật ký (chỉ visible records)
        /// </summary>
        public List<Actions> GetAllLogs()
        {
            var logs = new List<Actions>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Actions WHERE Visible=TRUE ORDER BY CreatedAt DESC", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new Actions
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
        /// Ghi lại nhật ký hành động
        /// </summary>
        public bool LogAction(string actionType, string descriptions, string dataBefore = "")
        {
            if (string.IsNullOrEmpty(dataBefore)) dataBefore = "{}";
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO Actions (ActionType, Descriptions, DataBefore, Visible, CreatedAt) " +
                        "VALUES (@type, @desc, @data, TRUE, @created)", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", actionType);
                        cmd.Parameters.AddWithValue("@desc", descriptions);
                        cmd.Parameters.AddWithValue("@data", dataBefore);
                        cmd.Parameters.AddWithValue("@created", DateTime.Now);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi ghi nhật ký: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy nhật ký theo loại hành động (chỉ visible records)
        /// </summary>
        public List<Actions> GetLogsByActionType(string actionType)
        {
            var logs = new List<Actions>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM Actions WHERE ActionType=@type AND Visible=TRUE ORDER BY CreatedAt DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@type", actionType);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new Actions
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
                throw new Exception($"Lỗi khi lấy nhật ký loại {actionType}: " + ex.Message);
            }
            return logs;
        }

        /// <summary>
        /// Lấy N hành động gần nhất (LIFO stack - Last In First Out)
        /// </summary>
        public List<Actions> GetLastNLogs(int count = 10)
        {
            var logs = new List<Actions>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        $"SELECT * FROM Actions WHERE Visible=TRUE AND ActionType != 'UNDO_ACTION' ORDER BY CreatedAt DESC LIMIT {Math.Min(count, 10)}", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new Actions
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
                throw new Exception("Lỗi khi lấy nhật ký gần nhất: " + ex.Message);
            }
            return logs;
        }

        /// <summary>
        /// Xóa mềm hành động từ undo stack (set Visible=FALSE)
        /// </summary>
        public bool RemoveFromUndoStack(int logId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE Actions SET Visible=FALSE WHERE LogID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", logId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa hành động khỏi undo stack: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa nhật ký cũ (hơn N ngày)
        /// </summary>
        public bool DeleteOldLogs(int daysOld)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "DELETE FROM Actions WHERE CreatedAt < DATE_SUB(NOW(), INTERVAL @days DAY)", conn))
                    {
                        cmd.Parameters.AddWithValue("@days", daysOld);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa nhật ký cũ: " + ex.Message);
            }
        }
    }
}