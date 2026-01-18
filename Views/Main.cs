using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.Services;
using WarehouseManagement.Views.Panels;
using WarehouseManagement.Views.Forms;
using WarehouseManagement.Views.Commons;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views
{
    /// <summary>
    /// Main - Khung giao diện chính với Grid Layout 2x2
    /// Layout: 
    ///   - Footer (Bottom)
    ///   - MainZone (Fill): Grid 2x2
    ///     Row 0: AppName | ToolsBar
    ///     Row 1: MenuBar | Content
    /// </summary>
    public partial class Main : Form
    {
        private ProductController _productController;
        private CategoryController _categoryController;
        private InventoryController _inventoryController;
        private ActionsController _logController;
        private ActionsService _actionsService;
        private Actions _actions;

        // Main components
        private TableLayoutPanel mainZone;
        private Footer footer;
        private AppName appName;
        private ToolsBar toolsBar;
        private MenuBar menuBar;
        private Content content;

        // Content panels
        private ProductsPanel productsPanel;
        private CategoriesPanel categoriesPanel;
        private TransactionsPanel transactionsPanel;

        private System.Windows.Forms.Timer statusUpdateTimer, timeUpdateTimer;

        public Main()
        {
            _productController = new ProductController();
            _categoryController = new CategoryController();
            _inventoryController = new InventoryController();
            _logController = new ActionsController();
            _actionsService = ActionsService.Instance;

            InitializeComponent();
            Text = $"{UIConstants.Icons.Warehouse} Quản Lý Kho Hàng";
            WindowState = FormWindowState.Maximized;

            // Apply theme
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // 1. FOOTER (Bottom)
            footer = new Footer
            {
                Dock = DockStyle.Bottom
            };

            // 2. MAINZONE - TableLayoutPanel với Grid 2x2
            mainZone = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = ThemeManager.Instance.BackgroundDefault,
                Padding = new Padding(20),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Cấu hình columns: Col0 (220px) | Col1 (Fill)
            mainZone.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            mainZone.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Cấu hình rows: Row0 (70px) | Row1 (Fill)
            mainZone.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            mainZone.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // 3. CREATE COMPONENTS
            appName = new AppName { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 10) };
            toolsBar = new ToolsBar { Dock = DockStyle.Fill, Margin = new Padding(10, 0, 0, 10) };
            menuBar = new MenuBar { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 10, 0) };
            content = new Content { Dock = DockStyle.Fill, Margin = new Padding(10, 10, 0, 0) };

            // 4. ADD COMPONENTS TO GRID
            // Row 0, Col 0: AppName
            mainZone.Controls.Add(appName, 0, 0);
            // Row 0, Col 1: ToolsBar
            mainZone.Controls.Add(toolsBar, 1, 0);
            // Row 1, Col 0: MenuBar
            mainZone.Controls.Add(menuBar, 0, 1);
            // Row 1, Col 1: Content
            mainZone.Controls.Add(content, 1, 1);

            // 5. CREATE CONTENT PANELS
            CreatePanels();
            content.Controls.Add(transactionsPanel);
            content.Controls.Add(productsPanel);
            content.Controls.Add(categoriesPanel);

            // 6. ADD TO FORM
            Controls.Add(mainZone);
            Controls.Add(footer);

            // 7. SETUP EVENT HANDLERS
            SetupEventHandlers();

            // 8. INITIALIZE TIMERS
            InitializeTimers();

            Load += Main_Load;
            FormClosing += Main_FormClosing;
            ResumeLayout(false);
        }

        private void CreatePanels()
        {
            // Panel 0: Categories
            categoriesPanel = new CategoriesPanel
            {
                Dock = DockStyle.Fill,
                Visible = true
            };

            // Panel 1: Products
            productsPanel = new ProductsPanel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };

            // Panel 2: Transactions
            transactionsPanel = new TransactionsPanel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
        }

        private void SetupEventHandlers()
        {
            // Initialize Actions handler
            _actions = new Actions(
                _actionsService,
                _inventoryController,
                toolsBar.LblChangeStatus,
                toolsBar.BtnSave,
                RefreshAllData
            );

            // ToolsBar events
            toolsBar.TxtSearch.TextChanged += TxtSearch_TextChanged;
            toolsBar.BtnSearch.Click += (s, e) => TxtSearch_TextChanged(null, null);
            toolsBar.BtnAddRecord.Click += BtnAddRecord_Click;
            toolsBar.BtnImport.Click += BtnImport_Click;
            toolsBar.BtnExport.Click += BtnExport_Click;
            toolsBar.BtnUndo.Click += BtnUndo_Click;
            toolsBar.BtnSave.Click += BtnSave_Click;
            toolsBar.BtnReport.Click += BtnReport_Click;

            // MenuBar events
            menuBar.BtnCategories.Click += (s, e) => ShowPanel(0);
            menuBar.BtnProducts.Click += (s, e) => ShowPanel(1);
            menuBar.BtnTransactions.Click += (s, e) => ShowPanel(2);
            menuBar.BtnSettings.Click += BtnSettings_Click;
            menuBar.BtnAccount.Click += BtnAccount_Click;

            // Update account button text
            if (GlobalUser.CurrentUser != null)
            {
                menuBar.BtnAccount.Text = $"{UIConstants.Icons.User} {GlobalUser.CurrentUser.FullName}";
            }
        }

        private void InitializeTimers()
        {
            statusUpdateTimer = new System.Windows.Forms.Timer
            {
                Interval = 500
            };
            statusUpdateTimer.Tick += (s, e) => _actions?.UpdateChangeStatus();

            timeUpdateTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            timeUpdateTimer.Tick += (s, e) => footer.LblFooterTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void ShowPanel(int index)
        {
            ResetSearch();

            // Hide all panels
            categoriesPanel.Visible = false;
            productsPanel.Visible = false;
            transactionsPanel.Visible = false;

            // Show selected panel
            switch (index)
            {
                case 0:
                    categoriesPanel.Visible = true;
                    categoriesPanel.BringToFront();
                    break;
                case 1:
                    productsPanel.Visible = true;
                    productsPanel.BringToFront();
                    break;
                case 2:
                    transactionsPanel.Visible = true;
                    transactionsPanel.BringToFront();
                    break;
            }
        }

        private void BtnAddRecord_Click(object sender, EventArgs e)
        {
            // Determine which form to open based on currently visible panel
            if (categoriesPanel.Visible)
            {
                CategoryForm form = new CategoryForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    categoriesPanel.LoadData();
                    productsPanel.LoadData();
                    _actions?.UpdateChangeStatus();
                }
            }
            else if (productsPanel.Visible)
            {
                ProductForm form = new ProductForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    productsPanel.LoadData();
                    _actions?.UpdateChangeStatus();
                }
            }
            else if (transactionsPanel.Visible)
            {
                ProductForm form = new ProductForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    productsPanel.LoadData();
                    transactionsPanel.LoadData();
                    _actions?.UpdateChangeStatus();
                }
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void BtnAccount_Click(object sender, EventArgs e)
        {
            // Show popup with Switch Account and Quit options
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("🔄 Chuyển Account", null, (s, e) =>
            {
                this.Hide();
                Login loginForm = new Login();
                loginForm.ShowDialog();
                if (GlobalUser.CurrentUser != null)
                {
                    menuBar.BtnAccount.Text = "👤 " + GlobalUser.CurrentUser.FullName;
                    this.Show();
                }
                else
                {
                    Application.Exit();
                }
            });
            menu.Items.Add("❌ Thoát", null, (s, e) => Application.Exit());
            menu.Show(menuBar.BtnAccount, 0, menuBar.BtnAccount.Height);
        }

        private void ResetSearch()
        {
            toolsBar.TxtSearch.Text = "Tìm kiếm...";
            toolsBar.TxtSearch.ForeColor = Color.Gray;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (toolsBar.TxtSearch.Text == "Tìm kiếm...") return;

            // Determine which panel is visible and search
            Control panel = null;

            if (categoriesPanel.Visible) panel = categoriesPanel;
            else if (productsPanel.Visible) panel = productsPanel;
            else if (transactionsPanel.Visible) panel = transactionsPanel;

            if (panel is ISearchable searchable)
            {
                searchable.Search(toolsBar.TxtSearch.Text);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            TransactionAllForm form = new TransactionAllForm("Import");
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshAllData();
                _actions?.UpdateChangeStatus();
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            TransactionAllForm form = new TransactionAllForm("Export");
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshAllData();
                _actions?.UpdateChangeStatus();
            }
        }

        private void BtnUndo_Click(object sender, EventArgs e)
        {
            if (_actions != null)
            {
                _actions.Undo();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_actions != null)
            {
                _actions.Save();
            }
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            TransactionReportForm form = new TransactionReportForm();
            form.ShowDialog();
        }

        private void RefreshAllData()
        {
            productsPanel.LoadData();
            categoriesPanel.LoadData();
            transactionsPanel.LoadData();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser != null && !GlobalUser.CurrentUser.IsAdmin)
            {
                // Staff restrictions here if needed
            }

            statusUpdateTimer?.Start();
            timeUpdateTimer?.Start();

            productsPanel.LoadData();
            categoriesPanel.LoadData();
            transactionsPanel.LoadData();

            footer.LblFooterTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                statusUpdateTimer?.Stop();
                statusUpdateTimer?.Dispose();
                timeUpdateTimer?.Stop();
                timeUpdateTimer?.Dispose();

                if (_actionsService.HasUnsavedChanges)
                {
                    DialogResult result = MessageBox.Show(
                        $"Có {_actionsService.ChangeCount} thay đổi chưa được lưu.\n\nBạn muốn lưu trước khi thoát?",
                        "Xác nhận thoát",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (result == DialogResult.Yes)
                    {
                        _actionsService.CommitChanges();
                        MessageBox.Show("Đã lưu thay đổi.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == DialogResult.No)
                    {
                        _actionsService.RollbackChanges();
                        MessageBox.Show("Đã hủy bỏ tất cả thay đổi từ lần lưu cuối.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                try
                {
                    if (_logController != null)
                    {
                        _logController.ClearAllLogs();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi xóa logs: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thoát: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}