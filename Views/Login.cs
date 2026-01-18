using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.Helpers;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views
{
    public partial class Login : Form
    {
        private UserController _userController;

        public Login()
        {
            InitializeComponent();
            _userController = new UserController();
            
            // Apply theme
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void Login_Load(object sender, EventArgs e)
        {
            Text = "Đăng Nhập Hệ Thống";
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = ThemeManager.Instance.BackgroundLight;
            
            txtUsername.Focus();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Focus();
                return;
            }

            // Try login - pass raw password, UserController will hash it
            User user = _userController.Login(txtUsername.Text, txtPassword.Text);
            
            if (user != null && user.IsActive)
            {
                GlobalUser.CurrentUser = user;
                MessageBox.Show($"Đăng nhập thành công!\nChào mừng {user.Username}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác", "Lỗi Đăng Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Text = "";
                txtPassword.Focus();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.SuppressKeyPress = true;
                BtnLogin_Click(null, null);
            }
        }

        private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.SuppressKeyPress = true;
                txtPassword.Focus();
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main container panel
            CustomPanel mainPanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = false,
                Padding = new Padding(UIConstants.Spacing.Padding.XXLarge)
            };

            // Title Label
            Label lblTitle = new Label
            {
                Text = $"{UIConstants.Icons.Lock} ĐĂNG NHẬP HỆ THỐNG",
                Font = ThemeManager.Instance.FontXLarge,
                ForeColor = ThemeManager.Instance.PrimaryDefault,
                AutoSize = true,
                Location = new Point(40, 30)
            };

            // Username Label
            lblUsername = new Label
            {
                Text = $"{UIConstants.Icons.User} Tên đăng nhập:",
                Font = ThemeManager.Instance.FontRegular,
                AutoSize = true,
                Location = new Point(40, 90),
                TabIndex = 0
            };

            // Username TextBox
            txtUsername = new CustomTextBox
            {
                Location = new Point(40, 115),
                Width = 380,
                Placeholder = "Nhập tên đăng nhập...",
                TabIndex = 1
            };
            txtUsername.KeyDown += TxtUsername_KeyDown;

            // Password Label
            lblPassword = new Label
            {
                Text = $"{UIConstants.Icons.Password} Mật khẩu:",
                Font = ThemeManager.Instance.FontRegular,
                AutoSize = true,
                Location = new Point(40, 170),
                TabIndex = 2
            };

            // Password TextBox
            txtPassword = new CustomTextBox
            {
                Location = new Point(40, 195),
                Width = 380,
                Placeholder = "Nhập mật khẩu...",
                IsPassword = true,
                TabIndex = 3
            };
            txtPassword.KeyDown += TxtPassword_KeyDown;

            // Login Button
            btnLogin = new CustomButton
            {
                Text = $"{UIConstants.Icons.Login} Đăng Nhập",
                Location = new Point(140, 270),
                Width = 140,
                Height = UIConstants.Sizes.ButtonHeight,
                ButtonStyleType = ButtonStyle.FilledNoOutline,
                TabIndex = 4
            };
            btnLogin.Click += BtnLogin_Click;

            // Cancel Button
            btnCancel = new CustomButton
            {
                Text = $"{UIConstants.Icons.Close} Thoát",
                Location = new Point(290, 270),
                Width = 130,
                Height = UIConstants.Sizes.ButtonHeight,
                ButtonStyleType = ButtonStyle.Outlined,
                TabIndex = 5
            };
            btnCancel.Click += BtnCancel_Click;

            // Add controls to main panel
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblUsername);
            mainPanel.Controls.Add(txtUsername);
            mainPanel.Controls.Add(lblPassword);
            mainPanel.Controls.Add(txtPassword);
            mainPanel.Controls.Add(btnLogin);
            mainPanel.Controls.Add(btnCancel);

            // Form settings
            ClientSize = new Size(480, 360);
            Controls.Add(mainPanel);
            Name = "Login";
            Text = "Login";
            Load += Login_Load;
            
            ResumeLayout(false);
        }

        private Label lblUsername;
        private Label lblPassword;
        private CustomTextBox txtUsername;
        private CustomTextBox txtPassword;
        private CustomButton btnLogin;
        private CustomButton btnCancel;
    }
}