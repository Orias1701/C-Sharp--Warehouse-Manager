using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;
using WarehouseManagement.Views.Forms;

namespace WarehouseManagement.Views.Commons
{
    /// <summary>
    /// MenuBar component - Thanh menu điều hướng với logic xử lý navigation
    /// </summary>
    public class MenuBar : CustomPanel
    {
        public CustomButton BtnCategories { get; private set; }
        public CustomButton BtnProducts { get; private set; }
        public CustomButton BtnTransactions { get; private set; }
        public CustomButton BtnSuppliers { get; private set; }
        public CustomButton BtnCustomers { get; private set; }
        public CustomButton BtnInventoryChecks { get; private set; }
        public CustomButton BtnSettings { get; private set; }
        public CustomButton BtnAccount { get; private set; }

        // Events
        public event EventHandler<int> PanelChangeRequested;
        public event EventHandler SettingsRequested;
        public event EventHandler AccountMenuRequested;

        public MenuBar()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            // Panel configuration
            BackColor = ThemeManager.Instance.BackgroundLight;
            ShowBorder = false;
            HasShadow = true;
            ShadowSize = 5;
            BorderRadius = UIConstants.Borders.RadiusMedium;
            Padding = new Padding(UIConstants.Spacing.Padding.Small);

            int btnHeight = 45;
            int btnMargin = 5;

            // Navigation buttons (Catalog)
            BtnCategories = new CustomButton
            {
                Text = $"{UIConstants.Icons.Category} Danh Mục",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            BtnProducts = new CustomButton
            {
                Text = $"{UIConstants.Icons.Product} Sản Phẩm",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            BtnSuppliers = new CustomButton
            {
                Text = $"{UIConstants.Icons.Supplier} Nhà Cung Cấp",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            BtnCustomers = new CustomButton
            {
                Text = $"{UIConstants.Icons.Customer} Khách Hàng",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            BtnTransactions = new CustomButton
            {
                Text = $"{UIConstants.Icons.Transaction} Giao Dịch",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            BtnInventoryChecks = new CustomButton
            {
                Text = $"{UIConstants.Icons.Check} Kiểm Kê",
                Dock = DockStyle.Top,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            // Settings & Account buttons (Bottom)
            BtnSettings = new CustomButton
            {
                Text = $"{UIConstants.Icons.Settings} Cài Đặt",
                Dock = DockStyle.Bottom,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            BtnAccount = new CustomButton
            {
                Text = $"{UIConstants.Icons.User} Account",
                Dock = DockStyle.Bottom,
                Height = btnHeight,
                Margin = new Padding(btnMargin),
                ButtonStyleType = ButtonStyle.Menu,
                Font = ThemeManager.Instance.FontBold,
                BorderRadius = UIConstants.Borders.RadiusMedium,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            Controls.Add(BtnAccount);
            Controls.Add(BtnSettings);
            // Add in reverse order of Dock.Top
            Controls.Add(BtnInventoryChecks);
            Controls.Add(BtnTransactions);
            Controls.Add(BtnCustomers);
            Controls.Add(BtnSuppliers);
            Controls.Add(BtnProducts);
            Controls.Add(BtnCategories);
        }

        private void SetupEventHandlers()
        {
            BtnCategories.Click += (s, e) => RequestPanelChange(0);
            BtnProducts.Click += (s, e) => RequestPanelChange(1);
            BtnTransactions.Click += (s, e) => RequestPanelChange(2);
            BtnSuppliers.Click += (s, e) => RequestPanelChange(3);
            BtnCustomers.Click += (s, e) => RequestPanelChange(4);
            BtnInventoryChecks.Click += (s, e) => RequestPanelChange(5);
            
            BtnSettings.Click += (s, e) => SettingsRequested?.Invoke(this, EventArgs.Empty);
            BtnAccount.Click += (s, e) => AccountMenuRequested?.Invoke(this, EventArgs.Empty);
        }

        private void RequestPanelChange(int panelIndex)
        {
            PanelChangeRequested?.Invoke(this, panelIndex);
        }

        public void UpdateAccountButtonText()
        {
            if (GlobalUser.CurrentUser != null)
            {
                BtnAccount.Text = $"{UIConstants.Icons.User} {GlobalUser.CurrentUser.FullName}";
            }
        }

        public void SetSelectedPanel(int panelIndex)
        {
            BtnCategories.IsSelected = (panelIndex == 0);
            BtnProducts.IsSelected = (panelIndex == 1);
            BtnTransactions.IsSelected = (panelIndex == 2);
            BtnSuppliers.IsSelected = (panelIndex == 3);
            BtnCustomers.IsSelected = (panelIndex == 4);
            BtnInventoryChecks.IsSelected = (panelIndex == 5);
        }
    }
}
