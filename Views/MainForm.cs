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
        private Button btnAddProduct, btnEditProduct, btnDeleteProduct;
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
            btnEditProduct = new Button { Text = "✏️ Sửa", Left = 100, Top = 15, Width = 80, Height = 30 };
            btnDeleteProduct = new Button { Text = "🗑️ Xóa", Left = 190, Top = 15, Width = 80, Height = 30 };
            btnImport = new Button { Text = "📥 Nhập", Left = 280, Top = 15, Width = 80, Height = 30 };
            btnExport = new Button { Text = "📤 Xuất", Left = 370, Top = 15, Width = 80, Height = 30 };
            btnUndo = new Button { Text = "↶ Hoàn tác", Left = 460, Top = 15, Width = 90, Height = 30 };
            btnReport = new Button { Text = "📊 Báo cáo", Left = 560, Top = 15, Width = 90, Height = 30 };

            btnAddProduct.Click += BtnAddProduct_Click;
            btnEditProduct.Click += BtnEditProduct_Click;
            btnDeleteProduct.Click += BtnDeleteProduct_Click;
            btnImport.Click += BtnImport_Click;
            btnExport.Click += BtnExport_Click;
            btnUndo.Click += BtnUndo_Click;
            btnReport.Click += BtnReport_Click;

            toolbar.Controls.Add(btnAddProduct);
            toolbar.Controls.Add(btnEditProduct);
            toolbar.Controls.Add(btnDeleteProduct);
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

            dgvProducts.CellFormatting += DgvProducts_CellFormatting;

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

            panel.Controls.Add(dgvTransactions);
            return panel;
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

        private void BtnEditProduct_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0) // Sản Phẩm
            {
                if (dgvProducts.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm");
                    return;
                }

                int productId = (int)dgvProducts.SelectedRows[0].Cells[0].Value;
                ProductForm form = new ProductForm(productId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    UpdateTotalValue();
                }
            }
            else if (tabControl.SelectedIndex == 1) // Danh Mục
            {
                if (dgvCategories.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn danh mục");
                    return;
                }

                int categoryId = (int)dgvCategories.SelectedRows[0].Cells[0].Value;
                CategoryForm form = new CategoryForm(categoryId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                    LoadProducts();
                }
            }
        }

        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0) // Sản Phẩm
            {
                if (dgvProducts.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm");
                    return;
                }

                if (MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int productId = (int)dgvProducts.SelectedRows[0].Cells[0].Value;
                    _productController.DeleteProduct(productId);
                    LoadProducts();
                    UpdateTotalValue();
                }
            }
            else if (tabControl.SelectedIndex == 1) // Danh Mục
            {
                if (dgvCategories.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn danh mục");
                    return;
                }

                if (MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int categoryId = (int)dgvCategories.SelectedRows[0].Cells[0].Value;
                    _productController.DeleteCategory(categoryId);
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

        private void BtnReport_Click(object sender, EventArgs e)
        {
            ReportForm form = new ReportForm();
            form.ShowDialog();
        }
    }
}
