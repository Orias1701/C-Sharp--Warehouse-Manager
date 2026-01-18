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
            ShowBorder = false;
            HasShadow = true;
            ShadowSize = 5;
            BorderRadius = UIConstants.Borders.RadiusMedium;
            Padding = new Padding(
                UIConstants.Spacing.Padding.Medium,
                UIConstants.Spacing.Padding.Large,
                UIConstants.Spacing.Padding.Medium,
                UIConstants.Spacing.Padding.Large
            );

            // App name label
            lblAppName = new Label
            {
                Text = $"{UIConstants.Icons.Warehouse} MANAGER",
                Font = ThemeManager.Instance.GetFont(UIConstants.Fonts.Large, FontStyle.Bold),
                ForeColor = ThemeManager.Instance.PrimaryDefault,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Controls.Add(lblAppName);
        }
    }
}
