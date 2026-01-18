using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Commons
{
    /// <summary>
    /// AppName component - Hiển thị tên ứng dụng
    /// </summary>
    public class AppName : CustomPanel
    {
        private Label lblAppName;

        public AppName()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Panel configuration
            BackColor = ThemeManager.Instance.BackgroundLight;
            ShowBorder = true;
            BorderRadius = UIConstants.Borders.RadiusMedium;
            Padding = new Padding(UIConstants.Spacing.Padding.Large);

            // App name label
            lblAppName = new Label
            {
                Text = $"{UIConstants.Icons.Warehouse} Quản Lý Kho Hàng",
                Font = ThemeManager.Instance.GetFont(UIConstants.Fonts.XLarge, FontStyle.Bold),
                ForeColor = ThemeManager.Instance.PrimaryDefault,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Controls.Add(lblAppName);
        }
    }
}
