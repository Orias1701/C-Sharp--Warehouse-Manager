using System;
using System.Collections.Generic;
using WarehouseManagement.Services;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller điều hướng các thao tác liên quan đến người dùng
    /// </summary>
    public class UserController
    {
        private readonly UserService _userService;

        public UserController()
        {
            _userService = new UserService();
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        public User Login(string username, string password)
        {
            return _userService.Login(username, password);
        }

        /// <summary>
        /// Lấy người dùng theo ID
        /// </summary>
        public User GetUserById(int userId)
        {
            return _userService.GetUserById(userId);
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        public List<User> GetAllUsers()
        {
            return _userService.GetAllUsers();
        }

        /// <summary>
        /// Kiểm tra tên đăng nhập có tồn tại hay không
        /// </summary>
        public bool UsernameExists(string username)
        {
            return _userService.UsernameExists(username);
        }

        /// <summary>
        /// Kiểm tra người dùng có hoạt động hay không
        /// </summary>
        public bool IsUserActive(int userId)
        {
            return _userService.IsUserActive(userId);
        }

        /// <summary>
        /// Đếm tổng số người dùng
        /// </summary>
        public int CountUsers()
        {
            return _userService.CountUsers();
        }

        /// <summary>
        /// Lấy danh sách người dùng hoạt động
        /// </summary>
        public List<User> GetActiveUsers()
        {
            return _userService.GetActiveUsers();
        }
    }
}
