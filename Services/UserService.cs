using System;
using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý logic người dùng
    /// 
    /// CHỨC NĂNG:
    /// - Quản lý người dùng (CRUD): Thêm, sửa, xóa
    /// - Xác thực: Đăng nhập, kiểm tra mật khẩu
    /// - Tìm kiếm người dùng: Theo tên, ID
    /// 
    /// LUỒNG:
    /// 1. Validation: Kiểm tra đầu vào (ID, tên, mật khẩu, v.v...)
    /// 2. Repository call: Gọi DB để thực hiện thao tác
    /// 3. Return: Trả về kết quả
    /// </summary>
    public class UserService
    {
        private readonly UserRepository _userRepo;

        public UserService()
        {
            _userRepo = new UserRepository();
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        public User Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Tên đăng nhập không được trống");
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Mật khẩu không được trống");

                var user = _userRepo.Login(username, password);
                if (user == null)
                    throw new Exception("Tên đăng nhập hoặc mật khẩu không đúng");

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đăng nhập: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy người dùng theo ID
        /// </summary>
        public User GetUserById(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ");
                return _userRepo.GetUserById(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy người dùng: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        public List<User> GetAllUsers()
        {
            try
            {
                return _userRepo.GetAllUsers();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách người dùng: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra tên đăng nhập có tồn tại hay không
        /// </summary>
        public bool UsernameExists(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Tên đăng nhập không được trống");

                var users = GetAllUsers();
                foreach (var user in users)
                {
                    if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi kiểm tra tên đăng nhập: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra người dùng có hoạt động hay không
        /// </summary>
        public bool IsUserActive(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("ID người dùng không hợp lệ");

                var user = _userRepo.GetUserById(userId);
                return user != null && user.IsActive;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi kiểm tra người dùng: " + ex.Message);
            }
        }

        /// <summary>
        /// Đếm tổng số người dùng
        /// </summary>
        public int CountUsers()
        {
            try
            {
                return _userRepo.GetAllUsers().Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đếm người dùng: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng hoạt động
        /// </summary>
        public List<User> GetActiveUsers()
        {
            try
            {
                var users = _userRepo.GetAllUsers();
                var activeUsers = new List<User>();
                foreach (var user in users)
                {
                    if (user.IsActive)
                        activeUsers.Add(user);
                }
                return activeUsers;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy người dùng hoạt động: " + ex.Message);
            }
        }
    }
}
