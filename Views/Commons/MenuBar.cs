using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Commons
{
    /// <summary>
    /// MenuBar component - Thanh menu điều hướng
    /// </summary>
    public class MenuBar : CustomPanel
    {
        public CustomButton BtnCategories { get; private set; }
        public CustomButton BtnProducts { get; private set; }
        public CustomButton BtnTransactions { get; private set; }
        public CustomButton BtnSettings { get; private set; }
        public CustomButton BtnAccount { get; private set; }

        public MenuBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Panel configuration
            BackColor = ThemeManager.Instance.BackgroundLight;
            ShowBorder = true;
            BorderRadius = UIConstants.Borders.RadiusMedium;
            Padding = new Padding(UIConstants.Spacing.Padding.Small);

            int btnHeight = 55;
            int btnMargin = 5;

            // Navigation buttons (Catalog)
            BtnCategories = new CustomButton
            {
                Text = $"{UIConstants.Icons.Category} Danh Mục",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.FilledNoOutline,
                Font = ThemeManager.Instance.FontMedium,
                BorderRadius = UIConstants.Borders.RadiusMedium
            };

            BtnProducts = new CustomButton
            {
                Text = $"{UIConstants.Icons.Product} Sản Phẩm",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.FilledNoOutline,
                Font = ThemeManager.Instance.FontMedium,
                BorderRadius = UIConstants.Borders.RadiusMedium
            };

            BtnTransactions = new CustomButton
            {
                Text = $"{UIConstants.Icons.Transaction} Giao Dịch",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.FilledNoOutline,
                Font = ThemeManager.Instance.FontMedium,
                BorderRadius = UIConstants.Borders.RadiusMedium
            };

            // Settings & Account buttons (Bottom)
            BtnSettings = new CustomButton
            {
                Text = $"{UIConstants.Icons.Settings} Cài Đặt",
                Dock = DockStyle.Bottom,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Outlined,
                Font = ThemeManager.Instance.FontRegular,
                BorderRadius = UIConstants.Borders.RadiusMedium
            };

            BtnAccount = new CustomButton
            {
                Text = $"{UIConstants.Icons.User} Account",
                Dock = DockStyle.Bottom,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Outlined,
                Font = ThemeManager.Instance.FontRegular,
                BorderRadius = UIConstants.Borders.RadiusMedium
            };

            Controls.Add(BtnAccount);
            Controls.Add(BtnSettings);
            Controls.Add(BtnTransactions);
            Controls.Add(BtnProducts);
            Controls.Add(BtnCategories);
        }
    }
}
