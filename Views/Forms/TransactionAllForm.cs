using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Forms
{
    /// <summary>
    /// Form Tạo phiếu Nhập/Xuất kho với TabControl
    /// </summary>
    public partial class TransactionAllForm : Form
    {
        private string _transactionType; // "Import" hoặc "Export"
        private InventoryController _inventoryController;
        private ProductController _productController;
        private SupplierController _supplierController;
        private CustomerController _customerController;
        private TabControl tabControl;
        private CustomComboBox cmbProduct;
        private CustomComboBox cmbSubject; // For Supplier or Customer
        private CustomTextBox txtQuantity, txtUnitPrice, txtDiscount;
        private CustomTextArea txtNote;
        private DataGridView dgvDetails;
        private CustomButton btnAddDetail, btnRemoveDetail, btnSaveTransaction, btnCancel, btnPrint;
        private List<(int ProductID, int Quantity, decimal UnitPrice, double DiscountRate)> _details;
        private Panel contentPanel;

        public TransactionAllForm()
        {
            InitializeComponent();
            _transactionType = "Import"; // Mặc định là Import
            _details = new List<(int, int, decimal, double)>();
            _inventoryController = new InventoryController();
            _productController = new ProductController();
            _supplierController = new SupplierController();
            _customerController = new CustomerController();
            
            Text = $"{UIConstants.Icons.Transaction} Giao dịch";
            
            // Apply theme
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Top,
                Height = 35,
                Font = ThemeManager.Instance.FontMedium
            };

            // Import Tab
            TabPage importTab = new TabPage($"{UIConstants.Icons.Import} Nhập kho");
            tabControl.TabPages.Add(importTab);

            // Export Tab
            TabPage exportTab = new TabPage($"{UIConstants.Icons.Export} Xuất kho");
            tabControl.TabPages.Add(exportTab);

            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Content Panel (chứa form content)
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.Instance.BackgroundLight,
                Padding = new Padding(0)
            };

            InitializeFormContent();

            Controls.Add(contentPanel);
            Controls.Add(tabControl);

            ClientSize = new Size(800, 635); // Tăng height để chứa TabControl
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;

            Load += TransactionAllForm_Load;
            ResumeLayout(false);
        }

        private void InitializeFormContent()
        {
            const int LEFT_MARGIN = 40;
            const int COL_GAP = 20;
            const int FULL_WIDTH = 720; // 800 - 40 - 40
            
            // 3 Columns Layout for Row 2
            const int COL_WIDTH_3 = (FULL_WIDTH - (COL_GAP * 2)) / 3;
             
            // 2 Columns Layout for Row 1
            const int COL_WIDTH_2 = (FULL_WIDTH - COL_GAP) / 2;

            int currentY = 20;
            int spacing = UIConstants.Spacing.Margin.Medium;

            // Row 1: Subject (Left) | Product (Right)
            // Subject (Supplier/Customer)
            Label lblSubject = new Label 
            { 
                Name = "lblSubject",
                Text = $"{UIConstants.Icons.User} Nhà cung cấp:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = COL_WIDTH_2,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Product Label
            Label lblProduct = new Label 
            { 
                Text = $"{UIConstants.Icons.Product} Sản phẩm:", 
                Left = LEFT_MARGIN + COL_WIDTH_2 + COL_GAP, 
                Top = currentY, 
                Width = COL_WIDTH_2,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };
            currentY += 25;

            cmbSubject = new CustomComboBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = COL_WIDTH_2
            };

            cmbProduct = new CustomComboBox 
            { 
                Left = LEFT_MARGIN + COL_WIDTH_2 + COL_GAP, 
                Top = currentY, 
                Width = COL_WIDTH_2
            };
            currentY += UIConstants.Sizes.InputHeight + spacing;

            // Row 2: Quantity | Price | Discount
            Label lblQuantity = new Label 
            { 
                Text = $"{UIConstants.Icons.Package} Số lượng:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = COL_WIDTH_3,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblPrice = new Label 
            { 
                Text = $"{UIConstants.Icons.Money} Đơn giá:", 
                Left = LEFT_MARGIN + COL_WIDTH_3 + COL_GAP, 
                Top = currentY, 
                Width = COL_WIDTH_3,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblDiscount = new Label 
            { 
                Text = $"{UIConstants.Icons.Product} Chiết khấu (%):", 
                Left = LEFT_MARGIN + (COL_WIDTH_3 + COL_GAP) * 2, 
                Top = currentY, 
                Width = COL_WIDTH_3,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };
            currentY += 25;

            txtQuantity = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = COL_WIDTH_3,
                Placeholder = "Số lượng..."
            };

            txtUnitPrice = new CustomTextBox 
            { 
                Left = LEFT_MARGIN + COL_WIDTH_3 + COL_GAP, 
                Top = currentY, 
                Width = COL_WIDTH_3,
                Placeholder = "Đơn giá..."
            };

            txtDiscount = new CustomTextBox 
            { 
                Left = LEFT_MARGIN + (COL_WIDTH_3 + COL_GAP) * 2, 
                Top = currentY, 
                Width = COL_WIDTH_3,
                Placeholder = "0"
            };
            currentY += UIConstants.Sizes.InputHeight + spacing;

            // Note (Full Width)
            Label lblNote = new Label 
            { 
                Text = $"{UIConstants.Icons.FileText} Ghi chú:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = FULL_WIDTH,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.TopLeft
            };
            currentY += 25;

            txtNote = new CustomTextArea 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = FULL_WIDTH, 
                Height = 60,
                Placeholder = "Ghi chú (không bắt buộc)..."
            };
            currentY += 60 + spacing;

            // Add/Remove buttons
            btnAddDetail = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Add} Thêm", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 110,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };

            btnRemoveDetail = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Delete} Xóa", 
                Left = LEFT_MARGIN + 110 + spacing, 
                Top = currentY, 
                Width = 110,
                ButtonStyleType = ButtonStyle.Outlined
            };

            btnAddDetail.Click += BtnAddDetail_Click;
            btnRemoveDetail.Click += BtnRemoveDetail_Click;
            currentY += UIConstants.Sizes.ButtonHeight + spacing;

            // DataGridView (Full Width)
            dgvDetails = new DataGridView
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = FULL_WIDTH,
                Height = 180,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = ThemeManager.Instance.BackgroundDefault,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = UIConstants.PrimaryColor.Default,
                    ForeColor = Color.White,
                    Font = ThemeManager.Instance.FontBold
                },
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToResizeRows = false,
                Font = ThemeManager.Instance.FontRegular,
                RowTemplate = { Height = 32 }
            };

            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Sản phẩm", 
                DataPropertyName = "ProductName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 4, 10, 4) }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "SL", 
                DataPropertyName = "Quantity", 
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(5, 4, 5, 4)
                }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Đơn giá", 
                DataPropertyName = "UnitPrice", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(5, 4, 5, 4)
                } 
            });
             dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "CK(%)", 
                DataPropertyName = "DiscountRate", 
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N1",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(5, 4, 5, 4)
                } 
            });
             dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Thành tiền", // Should be Net Amount
                DataPropertyName = "TotalAmount", 
                Width = 120,
                 DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(5, 4, 5, 4)
                } 
            });

            // Sync Header Alignment
            foreach (DataGridViewColumn col in dgvDetails.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(5, 4, 5, 4);
            }

            // Apply Hover Effect
            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvDetails);
            // Apply Selection Effect
            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvDetails);
            currentY += 180 + spacing;

            // Bottom buttons
            btnSaveTransaction = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Save} Lưu Phiếu", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 120,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };

            btnPrint = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Print} In Phiếu", 
                Left = LEFT_MARGIN + 120 + spacing, 
                Top = currentY, 
                Width = 120,
                ButtonStyleType = ButtonStyle.Outlined
            };

            btnCancel = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Cancel} Hủy", 
                Left = LEFT_MARGIN + 240 + spacing * 2, 
                Top = currentY, 
                Width = 100,
                ButtonStyleType = ButtonStyle.Text,
                CausesValidation = false
            };

            btnSaveTransaction.Click += BtnSaveTransaction_Click;
            btnCancel.Click += BtnCancel_Click;
            btnPrint.Click += BtnPrint_Click;

            contentPanel.Controls.Add(lblSubject);
            contentPanel.Controls.Add(cmbSubject);
            contentPanel.Controls.Add(lblProduct);
            contentPanel.Controls.Add(cmbProduct);
            contentPanel.Controls.Add(lblQuantity);
            contentPanel.Controls.Add(txtQuantity);
            contentPanel.Controls.Add(lblPrice);
            contentPanel.Controls.Add(txtUnitPrice);
             contentPanel.Controls.Add(lblDiscount);
            contentPanel.Controls.Add(txtDiscount);
            contentPanel.Controls.Add(lblNote);
            contentPanel.Controls.Add(txtNote);
            contentPanel.Controls.Add(btnAddDetail);
            contentPanel.Controls.Add(btnRemoveDetail);
            contentPanel.Controls.Add(btnSaveTransaction);
            contentPanel.Controls.Add(btnPrint);
            contentPanel.Controls.Add(btnCancel);
            contentPanel.Controls.Add(dgvDetails);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Đổi transaction type khi chuyển tab
            _transactionType = tabControl.SelectedIndex == 0 ? "Import" : "Export";
            
            // Update Label
            var lblSubject = contentPanel.Controls.Find("lblSubject", true).FirstOrDefault() as Label;
            if (lblSubject != null)
            {
                lblSubject.Text = _transactionType == "Import" 
                    ? $"{UIConstants.Icons.User} Nhà cung cấp:" 
                    : $"{UIConstants.Icons.User} Khách hàng:";
            }

            LoadSubjects();

            // Clear current details when switching tabs
            _details.Clear();
            RefreshDetails();
            
            // Clear inputs
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
            txtDiscount.Text = "";
            txtNote.Text = "";
        }

        private void LoadSubjects()
        {
            try 
            {
                if (_transactionType == "Import")
                {
                    var suppliers = _supplierController.GetAllSuppliers().Where(s => s.Visible).ToList();
                    cmbSubject.DataSource = suppliers;
                    cmbSubject.DisplayMember = "SupplierName";
                    cmbSubject.ValueMember = "SupplierID";
                }
                else
                {
                    var customers = _customerController.GetAllCustomers().Where(c => c.Visible).ToList();
                    cmbSubject.DataSource = customers;
                    cmbSubject.DisplayMember = "CustomerName";
                    cmbSubject.ValueMember = "CustomerID";
                }
                cmbSubject.SelectedIndex = -1; // Default to no selection
            }
            catch (Exception ex)
            {
                 MessageBox.Show("Lỗi tải danh sách đối tác: " + ex.Message);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (_details.Count == 0)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Vui lòng thêm ít nhất 1 sản phẩm trước khi in", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                 int subjectId = cmbSubject.SelectedValue != null ? (int)cmbSubject.SelectedValue : 0;
                 int userId = GlobalUser.CurrentUser?.UserID ?? 0;
                 
                 // Convert _details to List<TransactionDetail>
                 var detailList = new System.Collections.Generic.List<WarehouseManagement.Models.TransactionDetail>();
                 ProductController pc = new ProductController();
                 foreach (var d in _details)
                 {
                     var p = pc.GetProductById(d.ProductID);
                     decimal sub = d.Quantity * d.UnitPrice * (decimal)(1 - d.DiscountRate/100.0);
                     detailList.Add(new WarehouseManagement.Models.TransactionDetail 
                     {
                        ProductID = d.ProductID,
                        ProductName = p?.ProductName ?? "Unknown",
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        SubTotal = sub
                     });
                 }

                 Services.PrintService ps = new Services.PrintService();
                 // Call PrintPendingTransaction
                 ps.PrintPendingTransaction(_transactionType, detailList, txtNote.Text, 
                     _transactionType == "Import" ? subjectId : 0, 
                     _transactionType == "Export" ? subjectId : 0, 
                     userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi in phiếu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TransactionAllForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<Product> products = _productController.GetAllProducts();
            
                cmbProduct.DataSource = products;
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";

                if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
                
                LoadSubjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải sản phẩm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddDetail_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex < 0)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Vui lòng chọn sản phẩm", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProduct.Focus();
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity))
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Số lượng phải là số nguyên", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return;
            }

            if (quantity <= 0)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Số lượng phải lớn hơn 0", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return;
            }

            if (quantity > 999999)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Số lượng quá lớn (tối đa: 999,999)", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal price))
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Đơn giá phải là số", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return;
            }

            if (price < 0)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Đơn giá không được âm", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return;
            }

            if (price > 999999999)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Đơn giá quá lớn (tối đa: 999,999,999)", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return;
            }

            double discountRate = 0;
            if (!string.IsNullOrWhiteSpace(txtDiscount.Text))
            {
                 if (!double.TryParse(txtDiscount.Text, out discountRate))
                {
                    MessageBox.Show($"{UIConstants.Icons.Warning} Chiết khấu phải là số", "Cảnh báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscount.Focus();
                    return;
                }
                if (discountRate < 0 || discountRate > 100)
                {
                    MessageBox.Show($"{UIConstants.Icons.Warning} Chiết khấu phải từ 0 đến 100%", "Cảnh báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscount.Focus();
                    return;
                }
            }

            if (cmbProduct.SelectedValue == null)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Vui lòng chọn sản phẩm hợp lệ từ danh sách", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProduct.Focus();
                return;
            }

            int productId = (int)cmbProduct.SelectedValue;
            
            // Kiểm tra tồn kho nếu là Xuất
            if (_transactionType == "Export")
            {
                Product product = _productController.GetProductById(productId);
                if (product == null)
                {
                    MessageBox.Show($"{UIConstants.Icons.Error} Không tìm thấy thông tin sản phẩm", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (product.Quantity < quantity)
                {
                    MessageBox.Show($"{UIConstants.Icons.Warning} Tồn kho không đủ (hiện có: {product.Quantity})", "Cảnh báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQuantity.Focus();
                    return;
                }
            }

            _details.Add((productId, quantity, price, discountRate));
            RefreshDetails();
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
            txtDiscount.Text = "";
        }

        private void RefreshDetails()
        {
            dgvDetails.DataSource = null;
            var displayList = new List<dynamic>();
            foreach (var (productId, qty, price, discount) in _details)
            {
                var product = _productController.GetProductById(productId);
                decimal amount = qty * price * (decimal)(1 - discount / 100.0);
                displayList.Add(new { 
                    ProductName = product.ProductName, 
                    Quantity = qty, 
                    UnitPrice = price, 
                    DiscountRate = discount,
                    TotalAmount = amount
                });
            }
            dgvDetails.DataSource = displayList;
        }

        private void BtnRemoveDetail_Click(object sender, EventArgs e)
        {
            if (dgvDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Vui lòng chọn dòng để xóa", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = dgvDetails.SelectedRows[0].Index;
            if (index >= 0 && index < _details.Count)
            {
                _details.RemoveAt(index);
                RefreshDetails();
            }
        }

        private void BtnSaveTransaction_Click(object sender, EventArgs e)
        {
            if (_details.Count == 0)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Vui lòng thêm ít nhất một sản phẩm", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_transactionType == "Import")
                {
                    int supplierId = cmbSubject.SelectedValue != null ? (int)cmbSubject.SelectedValue : 0;
                    _inventoryController.ImportBatch(_details, txtNote.Text, supplierId);
                }
                else
                {
                    int customerId = cmbSubject.SelectedValue != null ? (int)cmbSubject.SelectedValue : 0;
                    _inventoryController.ExportBatch(_details, txtNote.Text, customerId);
                }
                MessageBox.Show($"{UIConstants.Icons.Success} Lưu phiếu thành công!", "Thành công", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
