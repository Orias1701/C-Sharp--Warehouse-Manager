using System;
using System.Windows.Forms;
using WarehouseManagement.Helpers;
using WarehouseManagement.Views;

namespace WarehouseManagement
{
    static class Program
    {
        /// <summary>
        /// Điểm vào chính của ứng dụng
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Khởi tạo visual styles cho ứng dụng Windows Forms
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Tự động chạy schema.sql để tạo database và bảng (nếu chưa tồn tại)
                try
                {
                    DatabaseHelper.ExecuteSchema();
                }
                catch
                {
                    // Schema có thể đã tồn tại, không cần báo lỗi
                }

                // Kiểm tra kết nối database trước khi chạy ứng dụng
                if (!DatabaseHelper.TestDatabaseConnection())
                {
                    MessageBox.Show(
                        "Không thể kết nối tới database!\n\nVui lòng kiểm tra:\n" +
                        "1. MySQL Server đang chạy\n" +
                        "2. Connection String trong App.config đúng\n\n" +
                        DatabaseHelper.GetConnectionError(),
                        "Lỗi Kết Nối Database",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Hiển thị form đăng nhập
                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng nhập thành công, chạy form chính
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khởi động ứng dụng: " + ex.Message + "\n\n" + ex.StackTrace, 
                    "Lỗi", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}
