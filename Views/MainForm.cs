using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views
{
    /// <summary>
    /// Form chính - Giao diện chính ứng dụng với TabControl
    /// </summary>
    public partial class MainForm : Form
    {
        private ProductController _productController;
        private InventoryController _inventoryController;
        private TabControl tabControl;
        private DataGridView dgvProducts;
        private DataGridView dgvCategories;
        private DataGridView dgvTransactions;
        private TextBox txtSearch;
        private Button btnAddProduct;
        private Button btnImport, btnExport, btnUndo, btnReport;
        private Label lblTotalValue;

        public MainForm()
        {
            InitializeComponent();
            Text = "Quản Lý Kho Hàng";
            WindowState = FormWindowState.Maximized;
            _productController = new ProductController();
            _inventoryController = new InventoryController();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 60)
            };
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Tab 1: Sản phẩm
            TabPage tabProducts = new TabPage("Sản Phẩm");
            tabProducts.Controls.Add(CreateProductsTab());
            tabControl.TabPages.Add(tabProducts);

            // Tab 1.5: Danh mục
            TabPage tabCategories = new TabPage("Danh Mục");
            tabCategories.Controls.Add(CreateCategoriesTab());
            tabControl.TabPages.Add(tabCategories);

            // Tab 2: Giao dịch
            TabPage tabTransactions = new TabPage("Giao Dịch");
            tabTransactions.Controls.Add(CreateTransactionsTab());
            tabControl.TabPages.Add(tabTransactions);

            // Tab 3: Báo cáo
            TabPage tabReport = new TabPage("Báo Cáo");
            tabReport.Controls.Add(CreateReportTab());
            tabControl.TabPages.Add(tabReport);

            // Toolbar
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnAddProduct = new Button { Text = "➕ Thêm", Left = 10, Top = 15, Width = 80, Height = 30 };
            btnImport = new Button { Text = "📥 Nhập", Left = 100, Top = 15, Width = 80, Height = 30 };
            btnExport = new Button { Text = "📤 Xuất", Left = 190, Top = 15, Width = 80, Height = 30 };
            btnUndo = new Button { Text = "↶ Hoàn tác", Left = 280, Top = 15, Width = 90, Height = 30 };
            btnReport = new Button { Text = "📊 Báo cáo", Left = 380, Top = 15, Width = 90, Height = 30 };

            btnAddProduct.Click += BtnAddProduct_Click;
            btnImport.Click += BtnImport_Click;
            btnExport.Click += BtnExport_Click;
            btnUndo.Click += BtnUndo_Click;
            btnReport.Click += BtnReport_Click;

            toolbar.Controls.Add(btnAddProduct);
            toolbar.Controls.Add(btnImport);
            toolbar.Controls.Add(btnExport);
            toolbar.Controls.Add(btnUndo);
            toolbar.Controls.Add(btnReport);

            Controls.Add(tabControl);
            Controls.Add(toolbar);

            Load += MainForm_Load;
            ResumeLayout(false);
        }

        private Control CreateProductsTab()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill };

            // Search box
            txtSearch = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                Margin = new Padding(5),
                Text = ""
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            panel.Controls.Add(txtSearch);

            // DataGridView
            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "ProductID", Width = 50 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên Sản Phẩm", DataPropertyName = "ProductName", Width = 220 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Danh Mục", DataPropertyName = "CategoryID", Width = 100 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá", DataPropertyName = "Price", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "C", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tồn Kho", DataPropertyName = "Quantity", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngưỡng Min", DataPropertyName = "MinThreshold", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            
            // Add action buttons
            DataGridViewButtonColumn editBtn = new DataGridViewButtonColumn { HeaderText = "✏️", Text = "Sửa", Width = 50, UseColumnTextForButtonValue = true };
            DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn { HeaderText = "🗑️", Text = "Xóa", Width = 50, UseColumnTextForButtonValue = true };
            dgvProducts.Columns.Add(editBtn);
            dgvProducts.Columns.Add(deleteBtn);

            dgvProducts.CellFormatting += DgvProducts_CellFormatting;
            dgvProducts.CellClick += DgvProducts_CellClick;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            panel.Controls.Add(dgvProducts);
            return panel;
        }

        private Control CreateCategoriesTab()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill };

            // DataGridView
            dgvCategories = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };

            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "CategoryID", Width = 50 });
            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên Danh Mục", DataPropertyName = "CategoryName", Width = 400 });
            
            // Add action buttons
            DataGridViewButtonColumn catEditBtn = new DataGridViewButtonColumn { HeaderText = "✏️", Text = "Sửa", Width = 50, UseColumnTextForButtonValue = true };
            DataGridViewButtonColumn catDeleteBtn = new DataGridViewButtonColumn { HeaderText = "🗑️", Text = "Xóa", Width = 50, UseColumnTextForButtonValue = true };
            dgvCategories.Columns.Add(catEditBtn);
            dgvCategories.Columns.Add(catDeleteBtn);

            dgvCategories.CellClick += DgvCategories_CellClick;
            dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            panel.Controls.Add(dgvCategories);
            return panel;
        }

        private Control CreateTransactionsTab()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill };

            dgvTransactions = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID Phiếu", DataPropertyName = "TransactionID", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Loại", DataPropertyName = "Type", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày", DataPropertyName = "DateCreated", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ghi chú", DataPropertyName = "Note", Width = 400 });
            
            // Add view button
            DataGridViewButtonColumn viewBtn = new DataGridViewButtonColumn { HeaderText = "👁️", Text = "Xem", Width = 50, UseColumnTextForButtonValue = true };
            dgvTransactions.Columns.Add(viewBtn);

            // Double-click để xem chi tiết
            dgvTransactions.CellDoubleClick += DgvTransactions_CellDoubleClick;
            dgvTransactions.CellClick += DgvTransactions_CellClick;
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            panel.Controls.Add(dgvTransactions);
            return panel;
        }

        private void DgvTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int transactionId = (int)dgvTransactions.Rows[e.RowIndex].Cells[0].Value;
            
            try
            {
                StockTransaction transaction = _inventoryController.GetTransactionById(transactionId);
                
                if (transaction != null)
                {
                    TransactionDetailForm form = new TransactionDetailForm(transaction);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy giao dịch");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải giao dịch: " + ex.Message);
            }
        }

        private Control CreateReportTab()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill };

            lblTotalValue = new Label
            {
                Dock = DockStyle.Top,
                Text = "Tổng giá trị tồn kho: 0 VNĐ",
                Height = 40,
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10)
            };

            panel.Controls.Add(lblTotalValue);
            return panel;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Kiểm tra quyền user
            if (GlobalUser.CurrentUser != null && !GlobalUser.CurrentUser.IsAdmin)
            {
                // Staff không thấy nút Báo cáo
                btnReport.Visible = false;
            }

            LoadProducts();
            LoadCategories();
            LoadTransactions();
            UpdateTotalValue();
        }

        private void LoadProducts()
        {
            try
            {
                List<Product> products = _productController.GetAllProducts();
                dgvProducts.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void LoadCategories()
        {
            try
            {
                List<Category> categories = _productController.GetAllCategories();
                dgvCategories.DataSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void LoadTransactions()
        {
            try
            {
                List<StockTransaction> transactions = _inventoryController.GetAllTransactions();
                dgvTransactions.DataSource = transactions;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải giao dịch: " + ex.Message);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            try
            {
                List<Product> allProducts = _productController.GetAllProducts();
                List<Product> filtered = allProducts.FindAll(p => p.ProductName.ToLower().Contains(searchText));
                dgvProducts.DataSource = filtered;
            }
            catch { }
        }

        private void DgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProducts.Rows[e.RowIndex].DataBoundItem is Product product)
            {
                if (product.IsLowStock)
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.DarkRed;
                }
                else
                {
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void UpdateTotalValue()
        {
            try
            {
                decimal total = _inventoryController.GetTotalInventoryValue();
                lblTotalValue.Text = $"Tổng giá trị tồn kho: {total:C}";
            }
            catch { }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0) // Sản Phẩm
            {
                ProductForm form = new ProductForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    UpdateTotalValue();
                }
            }
            else if (tabControl.SelectedIndex == 1) // Danh Mục
            {
                CategoryForm form = new CategoryForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                    LoadProducts();
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            TransactionForm form = new TransactionForm("Import");
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
                LoadTransactions();
                UpdateTotalValue();
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            TransactionForm form = new TransactionForm("Export");
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
                LoadTransactions();
                UpdateTotalValue();
            }
        }

        private void BtnUndo_Click(object sender, EventArgs e)
        {
            try
            {
                if (_inventoryController.UndoLastAction())
                {
                    MessageBox.Show("Hoàn tác thành công!");
                    LoadProducts();
                    LoadTransactions();
                    UpdateTotalValue();
                }
                else
                {
                    MessageBox.Show("Không có hành động để hoàn tác");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hoàn tác: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            ReportForm form = new ReportForm();
            form.ShowDialog();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Row selection và button click handler cho Products
        /// </summary>
        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;  // Header row
            
            // Check if button columns were clicked
            if (e.ColumnIndex == 6) // Edit button
            {
                int productId = (int)dgvProducts.Rows[e.RowIndex].Cells[0].Value;
                ProductForm form = new ProductForm(productId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    UpdateTotalValue();
                }
                return;
            }
            else if (e.ColumnIndex == 7) // Delete button
            {
                if (MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int productId = (int)dgvProducts.Rows[e.RowIndex].Cells[0].Value;
                    _productController.DeleteProduct(productId);
                    LoadProducts();
                    UpdateTotalValue();
                }
                return;
            }
            
            // Normal row selection for other columns
            dgvProducts.ClearSelection();
            dgvProducts.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// Row selection cho Transactions - click any cell để select entire row
        /// </summary>
        private void DgvTransactions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;  // Header row
            
            // Check if view button was clicked
            if (e.ColumnIndex == 4) // View button
            {
                int transactionId = (int)dgvTransactions.Rows[e.RowIndex].Cells[0].Value;
                
                try
                {
                    StockTransaction transaction = _inventoryController.GetTransactionById(transactionId);
                    if (transaction != null)
                    {
                        TransactionDetailForm form = new TransactionDetailForm(transaction);
                        form.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy giao dịch");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải giao dịch: " + ex.Message);
                }
                return;
            }
            
            // Normal row selection for other columns
            dgvTransactions.ClearSelection();
            dgvTransactions.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// Row selection và button click handler cho Categories
        /// </summary>
        private void DgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;  // Header row
            
            // Check if button columns were clicked
            if (e.ColumnIndex == 2) // Edit button
            {
                int categoryId = (int)dgvCategories.Rows[e.RowIndex].Cells[0].Value;
                CategoryForm form = new CategoryForm(categoryId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                    LoadProducts();
                }
                return;
            }
            else if (e.ColumnIndex == 3) // Delete button
            {
                if (MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int categoryId = (int)dgvCategories.Rows[e.RowIndex].Cells[0].Value;
                    _productController.DeleteCategory(categoryId);
                    LoadCategories();
                    LoadProducts();
                }
                return;
            }
            
            // Normal row selection for other columns
            dgvCategories.ClearSelection();
            dgvCategories.Rows[e.RowIndex].Selected = true;
        }
    }
}

