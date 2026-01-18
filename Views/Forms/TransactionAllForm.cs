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
    /// Form Tạo phiếu Nhập/Xuất kho
    /// </summary>
    public partial class TransactionAllForm : Form
    {
        private string _transactionType; // "Import" hoặc "Export"
        private InventoryController _inventoryController;
        private ProductController _productController;
        private CustomComboBox cmbProduct;
        private CustomTextBox txtQuantity, txtUnitPrice;
        private CustomTextArea txtNote;
        private DataGridView dgvDetails;
        private CustomButton btnAddDetail, btnRemoveDetail, btnSaveTransaction, btnCancel, btnExportVoucher;
        private List<(int ProductID, int Quantity, decimal UnitPrice)> _details;

        public TransactionAllForm(string type)
        {
            InitializeComponent();
            _transactionType = type;
            _details = new List<(int, int, decimal)>();
            _inventoryController = new InventoryController();
            _productController = new ProductController();
            
            string icon = type == "Import" ? UIConstants.Icons.Import : UIConstants.Icons.Export;
            Text = type == "Import" ? $"{icon} Phiếu Nhập Kho" : $"{icon} Phiếu Xuất Kho";
            
            // Apply theme
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main container
            CustomPanel mainPanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = false,
                Padding = new Padding(UIConstants.Spacing.Padding.Large)
            };

            const int LEFT_MARGIN = 20;
            int currentY = 20;
            int spacing = UIConstants.Spacing.Margin.Medium;

            // Product
            Label lblProduct = new Label 
            { 
                Text = $"{UIConstants.Icons.Product} Sản phẩm:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 110,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };
            currentY += 25;

            cmbProduct = new CustomComboBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 520
            };
            currentY += UIConstants.Sizes.InputHeight + spacing;

            // Quantity and Price (same row)
            Label lblQuantity = new Label 
            { 
                Text = $"{UIConstants.Icons.Package} Số lượng:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblPrice = new Label 
            { 
                Text = $"{UIConstants.Icons.Money} Đơn giá:", 
                Left = LEFT_MARGIN + 250, 
                Top = currentY, 
                Width = 80,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };
            currentY += 25;

            txtQuantity = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 220,
                Placeholder = "Số lượng..."
            };

            txtUnitPrice = new CustomTextBox 
            { 
                Left = LEFT_MARGIN + 250, 
                Top = currentY, 
                Width = 270,
                Placeholder = "Đơn giá..."
            };
            currentY += UIConstants.Sizes.InputHeight + spacing;

            // Note
            Label lblNote = new Label 
            { 
                Text = $"{UIConstants.Icons.FileText} Ghi chú:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.TopLeft
            };
            currentY += 25;

            txtNote = new CustomTextArea 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 520, 
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

            // DataGridView
            dgvDetails = new DataGridView
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = 520,
                Height = 180,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = ThemeManager.Instance.BackgroundDefault,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sản phẩm", DataPropertyName = "ProductName", Width = 250 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", DataPropertyName = "Quantity", Width = 80 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", DataPropertyName = "UnitPrice", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
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

            btnExportVoucher = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Export} Xuất Phiếu", 
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
            btnExportVoucher.Click += BtnExportVoucher_Click;

            mainPanel.Controls.Add(lblProduct);
            mainPanel.Controls.Add(cmbProduct);
            mainPanel.Controls.Add(lblQuantity);
            mainPanel.Controls.Add(txtQuantity);
            mainPanel.Controls.Add(lblPrice);
            mainPanel.Controls.Add(txtUnitPrice);
            mainPanel.Controls.Add(lblNote);
            mainPanel.Controls.Add(txtNote);
            mainPanel.Controls.Add(btnAddDetail);
            mainPanel.Controls.Add(btnRemoveDetail);
            mainPanel.Controls.Add(btnSaveTransaction);
            mainPanel.Controls.Add(btnExportVoucher);
            mainPanel.Controls.Add(btnCancel);
            mainPanel.Controls.Add(dgvDetails);

            Controls.Add(mainPanel);

            ClientSize = new Size(620, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;

            Load += TransactionAllForm_Load;
            ResumeLayout(false);
        }

        private void BtnExportVoucher_Click(object sender, EventArgs e)
        {
            if (_details.Count == 0)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Vui lòng thêm ít nhất 1 sản phẩm trước khi xuất phiếu", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv",
                    DefaultExt = "txt",
                    FileName = $"Phieu_{_transactionType}_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportVoucherToFile(saveDialog.FileName);
                    MessageBox.Show($"{UIConstants.Icons.Success} Xuất phiếu thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi xuất phiếu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportVoucherToFile(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Header
                writer.WriteLine("╔════════════════════════════════════════════════════╗");
                writer.WriteLine($"║ PHIẾU {(_transactionType == "Import" ? "NHẬP KHO" : "XUẤT KHO"),-42} ║");
                writer.WriteLine("╚════════════════════════════════════════════════════╝");
                writer.WriteLine();

                // Thông tin phiếu
                writer.WriteLine($"Ngày tạo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                writer.WriteLine($"Loại phiếu: {(_transactionType == "Import" ? "Nhập kho" : "Xuất kho")}");
                if (!string.IsNullOrEmpty(txtNote.Text))
                {
                    writer.WriteLine($"Ghi chú: {txtNote.Text}");
                }
                writer.WriteLine();

                // Chi tiết
                writer.WriteLine("┌────────────────────────────────────────────────────┐");
                writer.WriteLine("│ CHI TIẾT PHIẾU                                     │");
                writer.WriteLine("├────────────────────────────────────────────────────┤");

                decimal totalAmount = 0;
                foreach (var detail in _details)
                {
                    var product = _productController.GetProductById(detail.ProductID);
                    decimal amount = detail.Quantity * detail.UnitPrice;
                    totalAmount += amount;

                    writer.WriteLine($"│ Sản phẩm: {product?.ProductName ?? "N/A",-35} │");
                    writer.WriteLine($"│   Số lượng: {detail.Quantity,-38} │");
                    writer.WriteLine($"│   Đơn giá: {detail.UnitPrice:N0} ₫{"",-31} │");
                    writer.WriteLine($"│   Thành tiền: {amount:N0} ₫{"",-25} │");
                    writer.WriteLine("├────────────────────────────────────────────────────┤");
                }

                writer.WriteLine($"│ TỔNG CỘNG: {totalAmount:N0} ₫{"",-21} │");
                writer.WriteLine("└────────────────────────────────────────────────────┘");
                writer.WriteLine();
                writer.WriteLine($"In lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
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

            _details.Add((productId, quantity, price));
            RefreshDetails();
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
        }

        private void RefreshDetails()
        {
            dgvDetails.DataSource = null;
            var displayList = new List<dynamic>();
            foreach (var (productId, qty, price) in _details)
            {
                var product = _productController.GetProductById(productId);
                displayList.Add(new { ProductName = product.ProductName, Quantity = qty, UnitPrice = price });
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
                    _inventoryController.ImportBatch(_details, txtNote.Text);
                }
                else
                {
                    _inventoryController.ExportBatch(_details, txtNote.Text);
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