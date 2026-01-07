using System;
using System.Windows.Forms;
using WarehouseManagement.Helpers;
using WarehouseManagement.Views;
using WarehouseManagement.Models;

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
                // Tự động chạy schema.sql lần đầu
                try
                {
                    DatabaseHelper.ExecuteSchema();
                }
                catch
                {
                    // Schema có thể đã tồn tại, không cần lỗi
                }

                // Kiểm tra kết nối database
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

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Hiển thị LoginForm
                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK && GlobalUser.CurrentUser != null)
                {
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi động ứng dụng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
