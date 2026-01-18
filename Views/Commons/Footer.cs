using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Commons
{
    /// <summary>
    /// Footer component - Hiển thị thông tin phiên bản và thời gian
    /// </summary>
    public class Footer : CustomPanel
    {
        private Label lblVersion;
        public Label LblFooterTime { get; private set; }

        public Footer()
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
            ShadowOffsetY = -4; // Shadow hắt lên trên
            BorderRadius = 0;
            Padding = new Padding(UIConstants.Spacing.Padding.Small);
            Height = 45;

            // Version label (Left)
            lblVersion = new Label
            {
                Text = "v1.0.0",
                Dock = DockStyle.Left,
                Width = 100,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(UIConstants.Spacing.Padding.Large, 0, 0, 0),
                Font = ThemeManager.Instance.FontRegular,
                ForeColor = ThemeManager.Instance.TextSecondary
            };

            // Time label (Right)
            LblFooterTime = new Label
            {
                Dock = DockStyle.Right,
                Width = 250,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, UIConstants.Spacing.Padding.Large, 0),
                Font = ThemeManager.Instance.FontRegular,
                ForeColor = ThemeManager.Instance.TextSecondary
            };

            Controls.Add(lblVersion);
            Controls.Add(LblFooterTime);
        }
    }
}
