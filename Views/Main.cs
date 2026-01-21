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
        private SuppliersPanel suppliersPanel;
        private CustomersPanel customersPanel;
        private InventoryChecksPanel inventoryChecksPanel;

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
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 20, 0, 0) // Top 20 để cách phần trên
            };

            // 2. MAINZONE - TableLayoutPanel với Grid 2x2
            mainZone = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = ThemeManager.Instance.BackgroundDefault,
                Padding = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Cấu hình columns: Col0 (200px) | Col1 (Fill)
            mainZone.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            mainZone.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Cấu hình rows: Row0 (90px) | Row1 (Fill) | Row2 (45px)
            mainZone.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            mainZone.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainZone.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F)); // 45 + 20 margin

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
            
            // Row 2, Col 0+1: Footer (Span 2 columns)
            mainZone.Controls.Add(footer, 0, 2);
            mainZone.SetColumnSpan(footer, 2);

            // 5. CREATE CONTENT PANELS
            CreatePanels();
            content.Controls.Add(inventoryChecksPanel);
            content.Controls.Add(customersPanel);
            content.Controls.Add(suppliersPanel);
            content.Controls.Add(transactionsPanel);
            content.Controls.Add(productsPanel);
            content.Controls.Add(categoriesPanel);

            // 6. ADD TO FORM
            Controls.Add(mainZone);
             // Footer now inside mainZone, no need to add separately
            
            // Ensure footer is on top for shadow to be visible over mainZone
            // footer.BringToFront(); // No longer needed inside grid? Or maybe needed for Z-order inside cell?

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

            // Panel 3: Suppliers
            suppliersPanel = new SuppliersPanel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };

            // Panel 4: Customers
            customersPanel = new CustomersPanel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };

            // Panel 5: InventoryChecks
            inventoryChecksPanel = new InventoryChecksPanel
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
            toolsBar.SearchRequested += OnSearchRequested;
            toolsBar.AddRequested += OnAddRequested;
            toolsBar.TransactionRequested += OnTransactionRequested;
            toolsBar.InventoryRequested += OnInventoryRequested;
            toolsBar.UndoRequested += OnUndoRequested;
            toolsBar.SaveRequested += OnSaveRequested;
            toolsBar.ReportRequested += OnReportRequested;

            // MenuBar events
            menuBar.PanelChangeRequested += OnPanelChangeRequested;
            menuBar.SettingsRequested += OnSettingsRequested;
            menuBar.AccountMenuRequested += OnAccountMenuRequested;

            // Update account button text
            menuBar.UpdateAccountButtonText();
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

        // Event handlers for MenuBar
        private void OnPanelChangeRequested(object sender, int panelIndex)
        {
            toolsBar.ResetSearch();

            // Hide all panels
            categoriesPanel.Visible = false;
            productsPanel.Visible = false;
            transactionsPanel.Visible = false;
            suppliersPanel.Visible = false;
            customersPanel.Visible = false;
            inventoryChecksPanel.Visible = false;

            // Update menu button states
            menuBar.SetSelectedPanel(panelIndex);
            
            // Show selected panel
            switch (panelIndex)
            {
                case 0: // Categories
                    categoriesPanel.Visible = true;
                    categoriesPanel.BringToFront();
                    break;
                case 1: // Products
                    productsPanel.Visible = true;
                    productsPanel.BringToFront();
                    break;
                case 2: // Transactions
                    transactionsPanel.Visible = true;
                    transactionsPanel.BringToFront();
                    break;
                case 3: // Suppliers
                    suppliersPanel.Visible = true;
                    suppliersPanel.BringToFront();
                    break;
                case 4: // Customers
                    customersPanel.Visible = true;
                    customersPanel.BringToFront();
                    break;
                case 5: // Inventory Checks
                    inventoryChecksPanel.Visible = true;
                    inventoryChecksPanel.BringToFront();
                    break;
            }
            
            // Update toolbar state based on panel
            SetToolbarStateForPanel(panelIndex);
        }
        
        /// <summary>
        /// Set trạng thái toolbar (enable/disable và text) dựa trên panel đang hiển thị
        /// </summary>
        private void SetToolbarStateForPanel(int panelIndex)
        {
            switch (panelIndex)
            {
                case 0: // Categories
                    toolsBar.BtnAdd.Enabled = true;
                    toolsBar.BtnAdd.Text = $"{UIConstants.Icons.Add} Thêm Danh mục";
                    break;
                case 1: // Products
                    toolsBar.BtnAdd.Enabled = true;
                    toolsBar.BtnAdd.Text = $"{UIConstants.Icons.Add} Thêm Sản phẩm";
                    break;
                case 2: // Transactions
                    toolsBar.BtnAdd.Enabled = false;
                    toolsBar.BtnAdd.Text = $"{UIConstants.Icons.Add} Thêm";
                    break;
                case 3: // Suppliers
                    toolsBar.BtnAdd.Enabled = true;
                    toolsBar.BtnAdd.Text = $"{UIConstants.Icons.Add} Thêm NCC";
                    break;
                case 4: // Customers
                    toolsBar.BtnAdd.Enabled = true;
                    toolsBar.BtnAdd.Text = $"{UIConstants.Icons.Add} Thêm KH";
                    break;
                case 5: // Inventory Checks
                    toolsBar.BtnAdd.Enabled = false; 
                    toolsBar.BtnAdd.Text = $"{UIConstants.Icons.Add} Thêm Phiếu";
                    break;
            }
        }

        private void OnSettingsRequested(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void OnAccountMenuRequested(object sender, EventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("🔄 Chuyển Account", null, (s, e) =>
            {
                this.Hide();
                Login loginForm = new Login();
                loginForm.ShowDialog();
                if (GlobalUser.CurrentUser != null)
                {
                    menuBar.UpdateAccountButtonText();
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

        // Event handlers for ToolsBar
        private void OnSearchRequested(object sender, EventArgs e)
        {
            string searchText = toolsBar.GetSearchText();

            Control panel = null;
            if (categoriesPanel.Visible) panel = categoriesPanel;
            else if (productsPanel.Visible) panel = productsPanel;
            else if (transactionsPanel.Visible) panel = transactionsPanel;
            else if (suppliersPanel.Visible) panel = suppliersPanel;
            else if (customersPanel.Visible) panel = customersPanel;
            else if (inventoryChecksPanel.Visible) panel = inventoryChecksPanel;

            if (panel is ISearchable searchable)
            {
                // Nếu searchText rỗng, sẽ hiển thị tất cả (Contains("") = true)
                searchable.Search(searchText);
            }
        }

        private void OnAddRequested(object sender, EventArgs e)
        {
            // Xác định panel hiện tại và mở form tương ứng
            if (categoriesPanel.Visible)
            {
                CategoryForm form = new CategoryForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllData();
                    _actions?.UpdateChangeStatus();
                }
            }
            else if (productsPanel.Visible)
            {
                ProductForm form = new ProductForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllData();
                    _actions?.UpdateChangeStatus();
                }
            }
            else if (suppliersPanel.Visible)
            {
                SupplierForm form = new SupplierForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllData();
                    _actions?.UpdateChangeStatus();
                }
            }
            else if (customersPanel.Visible)
            {
                CustomerForm form = new CustomerForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllData();
                    _actions?.UpdateChangeStatus();
                }
            }

            // Transactions add buttons might function differently or be disabled
        }

        private void OnTransactionRequested(object sender, EventArgs e)
        {
            TransactionAllForm form = new TransactionAllForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshAllData();
                _actions?.UpdateChangeStatus();
            }
        }

        private void OnInventoryRequested(object sender, EventArgs e)
        {
            InventoryCheckForm form = new InventoryCheckForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshAllData();
                _actions?.UpdateChangeStatus();
            }
        }

        private void OnUndoRequested(object sender, EventArgs e)
        {
            _actions?.Undo();
        }

        private void OnSaveRequested(object sender, EventArgs e)
        {
            _actions?.Save();
        }

        private void OnReportRequested(object sender, EventArgs e)
        {
            TransactionReportForm form = new TransactionReportForm();
            form.ShowDialog();
        }

        private void RefreshAllData()
        {
            productsPanel.LoadData();
            categoriesPanel.LoadData();
            transactionsPanel.LoadData();
            suppliersPanel.LoadData();
            customersPanel.LoadData();
            inventoryChecksPanel.LoadData();
            
            // Re-apply search if needed
            // OnSearchRequested(this, EventArgs.Empty);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser != null && !GlobalUser.CurrentUser.IsAdmin)
            {
                // Staff restrictions here if needed
            }

            // Set initial panel state (Categories is default)
            int initialPanel = 0; // Categories
            // Logic checked against visible state
            
            // Set menu button state và toolbar state
            menuBar.SetSelectedPanel(initialPanel);
            SetToolbarStateForPanel(initialPanel);

            statusUpdateTimer?.Start();
            timeUpdateTimer?.Start();

            // Load all data
            RefreshAllData();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                statusUpdateTimer?.Stop();
                statusUpdateTimer?.Dispose();
                timeUpdateTimer?.Stop();
                timeUpdateTimer?.Dispose();

                int actionCount = _actionsService.CountLogs();
                if (actionCount > 0)
                {
                    string changeText = actionCount == 1 ? "1 thay đổi" : $"{actionCount} thay đổi";
                    DialogResult result = MessageBox.Show(
                        $"Có {changeText} chưa được lưu.\n\nBạn muốn lưu trước khi thoát?",
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
                        _actionsService.ClearAllLogs();
                        _actionsService.Reset();
                        MessageBox.Show("Đã lưu thay đổi.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == DialogResult.No)
                    {
                        _actionsService.ClearAllLogs();
                        MessageBox.Show("Đã hủy bỏ tất cả thay đổi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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