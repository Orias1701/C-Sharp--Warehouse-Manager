using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Forms
{
    /// <summary>
    /// Form Cài Đặt - Quản lý tùy chọn hiển thị và theme
    /// </summary>
    public class SettingsForm : Form
    {
        private CheckBox chkShowHidden;
        private CheckBox chkDarkMode;
        private Button btnViewComponents;
        private Button btnSave, btnCancel;

        // Static property to share settings across the app
        public static bool ShowHiddenItems { get; set; } = false;
        
        // Event to notify when settings change
        public static event EventHandler SettingsChanged;

        public SettingsForm()
        {
            InitializeComponent();
            Text = "Cài Đặt";
            
            // Subscribe to theme changes
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // === DISPLAY SETTINGS ===
            Label lblDisplaySettings = new Label
            {
                Text = "HIỂN THỊ",
                Left = 20,
                Top = 20,
                Width = 300,
                Height = 25,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // CheckBox Show Hidden
            chkShowHidden = new CheckBox
            {
                Text = "Hiển thị tất cả các bản ghi đã ẩn",
                Left = 40,
                Top = 55,
                Width = 300,
                Height = 25,
                Checked = ShowHiddenItems
            };

            // === THEME SETTINGS ===
            Label lblThemeSettings = new Label
            {
                Text = "GIAO DIỆN",
                Left = 20,
                Top = 100,
                Width = 300,
                Height = 25,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // CheckBox Dark Mode
            chkDarkMode = new CheckBox
            {
                Text = $"{UIConstants.Icons.Moon} Chế độ tối (Dark Mode)",
                Left = 40,
                Top = 135,
                Width = 300,
                Height = 25,
                Checked = ThemeManager.Instance.IsDarkMode
            };
            chkDarkMode.CheckedChanged += ChkDarkMode_CheckedChanged;

            // Button View Components
            btnViewComponents = new Button
            {
                Text = $"{UIConstants.Icons.Eye} Xem Components",
                Left = 40,
                Top = 170,
                Width = 200,
                Height = 35,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            btnViewComponents.Click += BtnViewComponents_Click;

            // === ACTION BUTTONS ===
            btnSave = new Button
            {
                Text = "💾 Lưu",
                Left = 100,
                Top = 230,
                Width = 100,
                Height = 35
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "❌ Hủy",
                Left = 210,
                Top = 230,
                Width = 100,
                Height = 35,
                CausesValidation = false
            };
            btnCancel.Click += BtnCancel_Click;

            Controls.Add(lblDisplaySettings);
            Controls.Add(chkShowHidden);
            Controls.Add(lblThemeSettings);
            Controls.Add(chkDarkMode);
            Controls.Add(btnViewComponents);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            Width = 400;
            Height = 320;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            ResumeLayout(false);
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            ForeColor = ThemeManager.Instance.TextPrimary;
            
            // Update icon in dark mode checkbox
            chkDarkMode.Text = ThemeManager.Instance.IsDarkMode 
                ? $"{UIConstants.Icons.Sun} Chế độ sáng (Light Mode)" 
                : $"{UIConstants.Icons.Moon} Chế độ tối (Dark Mode)";
        }

        private void ChkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.Instance.IsDarkMode = chkDarkMode.Checked;
        }

        private void BtnViewComponents_Click(object sender, EventArgs e)
        {
            // Tạo form mới để hiển thị ComponentsTestPanel
            Form componentsForm = new Form
            {
                Text = "Components Preview",
                Width = 1400,
                Height = 800,
                StartPosition = FormStartPosition.CenterParent,
                WindowState = FormWindowState.Maximized
            };

            ComponentsTestPanel testPanel = new ComponentsTestPanel();
            componentsForm.Controls.Add(testPanel);

            // Apply theme to form
            ThemeManager.Instance.ApplyThemeToForm(componentsForm);

            componentsForm.ShowDialog();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Update static property
            ShowHiddenItems = chkShowHidden.Checked;

            // Notify all listeners
            SettingsChanged?.Invoke(this, EventArgs.Empty);

            MessageBox.Show("Cài đặt đã được lưu.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Revert dark mode if changed
            chkDarkMode.Checked = ThemeManager.Instance.IsDarkMode;
            
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.Instance.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }
    }
}
