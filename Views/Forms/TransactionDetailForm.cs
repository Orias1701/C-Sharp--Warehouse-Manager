using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;
using WarehouseManagement.Controllers;

namespace WarehouseManagement.Views.Forms
{
    public partial class TransactionDetailForm : Form
    {
        private Transaction _transaction;
        private CustomTextBox txtTransactionID, txtType, txtDate, txtTotalValue, txtCreatedBy;
        private CustomTextArea txtNote;
        private DataGridView dgvDetails;
        private CustomButton btnClose;
        private SupplierController _supplierController;
        private CustomerController _customerController;
        private CustomTextBox txtPartner;

        public TransactionDetailForm(Transaction transaction)
        {
            InitializeComponent();
            _transaction = transaction;
            _supplierController = new SupplierController();
            _customerController = new CustomerController();
            Text = $"{UIConstants.Icons.FileText} Chi Tiết Giao Dịch #{transaction.TransactionID}";
            
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            CustomPanel mainPanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = false,
                Padding = new Padding(0)
            };

            const int LEFT_MARGIN = 40;
            const int COLUMN_GAP = 20;
            const int INPUT_WIDTH_HALF = 250; // Total width approx 40 + 250 + 20 + 250 + 40 = 600
            const int INPUT_WIDTH_FULL = 520; // 250 + 20 + 250
            int currentY = 30;
            int inputSpacing = 20;

            // Helper to create styled labels
            Label CreateStyledLabel(string text, int x, int y)
            {
                return new Label
                {
                    Text = text,
                    Left = x,
                    Top = y,
                    AutoSize = true,
                    Font = ThemeManager.Instance.FontSmall,
                    ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                              UIConstants.PrimaryColor.Default.G, 
                                              UIConstants.PrimaryColor.Default.B),
                    TabStop = false
                };
            }

            // Row 1: ID | Type
            mainPanel.Controls.Add(CreateStyledLabel("Mã Giao Dịch", LEFT_MARGIN, currentY));
            mainPanel.Controls.Add(CreateStyledLabel("Loại Giao Dịch", LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, currentY));
            currentY += 20;

            txtTransactionID = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = INPUT_WIDTH_HALF,
                ReadOnly = true,
                TabStop = false
            };
            mainPanel.Controls.Add(txtTransactionID);

            txtType = new CustomTextBox 
            { 
                Left = LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, 
                Top = currentY, 
                Width = INPUT_WIDTH_HALF,
                ReadOnly = true,
                TabStop = false
            };
            mainPanel.Controls.Add(txtType);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;


            // Row 2: Partner (Full width)
            mainPanel.Controls.Add(CreateStyledLabel("Đối tác", LEFT_MARGIN, currentY));
            currentY += 20;

            txtPartner = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = INPUT_WIDTH_FULL,
                ReadOnly = true,
                TabStop = false
            };
            mainPanel.Controls.Add(txtPartner);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Row 3: Date | User
            mainPanel.Controls.Add(CreateStyledLabel("Ngày Tạo", LEFT_MARGIN, currentY));
            mainPanel.Controls.Add(CreateStyledLabel("Người Tạo", LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, currentY));
            currentY += 20;

            txtDate = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = INPUT_WIDTH_HALF,
                ReadOnly = true,
                TabStop = false
            };
            mainPanel.Controls.Add(txtDate);

            txtCreatedBy = new CustomTextBox 
            { 
                Left = LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, 
                Top = currentY, 
                Width = INPUT_WIDTH_HALF,
                ReadOnly = true,
                TabStop = false
            };
            mainPanel.Controls.Add(txtCreatedBy);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Row 4: Total Value (Full width for emphasis)
            mainPanel.Controls.Add(CreateStyledLabel("Tổng Giá Trị", LEFT_MARGIN, currentY));
            currentY += 20;

            txtTotalValue = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = INPUT_WIDTH_FULL,
                ReadOnly = true,
                TabStop = false
            };
            mainPanel.Controls.Add(txtTotalValue);
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Row 5: Note
            mainPanel.Controls.Add(CreateStyledLabel("Ghi chú", LEFT_MARGIN, currentY));
            currentY += 20;

            txtNote = new CustomTextArea 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = INPUT_WIDTH_FULL, 
                Height = 60,
                ReadOnly = true,
                TabStop = false
            };
            mainPanel.Controls.Add(txtNote);
            currentY += 60 + inputSpacing; // Extra spacing before grid

            // Grid Title
            Label lblDetailsTitle = new Label
            {
                Text = $"{UIConstants.Icons.List} Chi Tiết Sản Phẩm",
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = 200,
                Font = ThemeManager.Instance.FontBold,
                ForeColor = ThemeManager.Instance.TextPrimary
            };
            mainPanel.Controls.Add(lblDetailsTitle);
            currentY += 25;

            // Grid
            dgvDetails = new DataGridView
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH_FULL,
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
                Width = 210,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 4, 10, 4) }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Số lượng", 
                DataPropertyName = "Quantity", 
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 30, 4)
                }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Đơn giá", 
                DataPropertyName = "UnitPrice", 
                Width = 110, 
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 30, 4)
                } 
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Thành tiền", 
                DataPropertyName = "SubTotal", 
                Width = 110, 
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 30, 4)
                } 
            });

            // Sync Header Alignment
            foreach (DataGridViewColumn col in dgvDetails.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 4, 10, 4);
            }
            
            // Apply Hover Effect
            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvDetails);
            // Apply Selection Effect
            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvDetails);

            mainPanel.Controls.Add(dgvDetails);
            currentY += 180 + 30;

            // Button Close
            btnClose = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Close} Đóng", 
                // Center button
                Left = LEFT_MARGIN + (INPUT_WIDTH_FULL - 120) / 2, 
                Top = currentY, 
                Width = 120,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            btnClose.Click += (s, e) => {
                DialogResult = DialogResult.OK;
                Close();
            };
            mainPanel.Controls.Add(btnClose);
            currentY += 35 + 30; // Button height + padding

            Controls.Add(mainPanel);

            // Size
            int calculatedWidth = LEFT_MARGIN + INPUT_WIDTH_FULL + LEFT_MARGIN; // 40 + 520 + 40 = 600
            ClientSize = new Size(calculatedWidth, currentY);
            
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;
            
            AcceptButton = btnClose;
            CancelButton = btnClose;

            Load += TransactionDetailForm_Load;
            ResumeLayout(false);
        }

        private void TransactionDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                txtTransactionID.Text = $"#{_transaction.TransactionID}";
                
                string typeIcon = _transaction.Type == "Import" ? UIConstants.Icons.Import : UIConstants.Icons.Export;
                string typeName = _transaction.Type == "Import" ? "Nhập" : "Xuất";
                txtType.Text = $"{typeIcon} {typeName}";
                
                txtDate.Text = _transaction.DateCreated.ToString("dd/MM/yyyy HH:mm");
                
                // Using TotalAmount or FinalAmount. Let's use FinalAmount (after discount) if available, 
                // or TotalAmount. The user requirement said FinalAmount is in Transaction.
                txtTotalValue.Text = $"{_transaction.FinalAmount:N0} ₫"; 
                
                txtCreatedBy.Text = $"User ID: {_transaction.CreatedByUserID}";
                txtCreatedBy.Text = $"User ID: {_transaction.CreatedByUserID}";
                
                // Load Partner Name
                string partnerName = "N/A";
                if (_transaction.Type == "Import" && _transaction.SupplierID.HasValue)
                {
                    var supplier = _supplierController.GetSupplierById(_transaction.SupplierID.Value);
                    partnerName = supplier != null ? $"Nhà cung cấp: {supplier.SupplierName}" : "N/A";
                }
                else if (_transaction.Type == "Export" && _transaction.CustomerID.HasValue)
                {
                    var customer = _customerController.GetCustomerById(_transaction.CustomerID.Value);
                    partnerName = customer != null ? $"Khách hàng: {customer.CustomerName}" : "N/A";
                }
                txtPartner.Text = partnerName;

                txtNote.Text = _transaction.Note ?? "(Không có ghi chú)";
                
                if (_transaction.Details != null && _transaction.Details.Count > 0)
                {
                    dgvDetails.DataSource = _transaction.Details;
                }
                
                ActiveControl = btnClose;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải chi tiết giao dịch: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
