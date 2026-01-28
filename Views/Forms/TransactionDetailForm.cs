using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;
using WarehouseManagement.Controllers;
using WarehouseManagement.Services;

namespace WarehouseManagement.Views.Forms
{
    public partial class TransactionDetailForm : Form
    {
        private Transaction _transaction;
        private DataGridView dgvDetails;
        private CustomButton btnClose;
        private SupplierController _supplierController;
        private CustomerController _customerController;
        
        public TransactionDetailForm(Transaction transaction)
        {
            _transaction = transaction;
            _supplierController = new SupplierController();
            _customerController = new CustomerController();
            InitializeComponent();
            
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
            const int INPUT_WIDTH_HALF = 462; // (944 - 20) / 2
            const int INPUT_WIDTH_FULL = 944; // 1024 - 40 - 40
            int currentY = 30;
            // int inputSpacing = 20;

            // Fetch Data for Labels
            string typeIcon = _transaction.Type == "Import" ? UIConstants.Icons.Import : UIConstants.Icons.Export;
            string typeName = _transaction.Type == "Import" ? "Nhập" : "Xuất";
            string typeText = $"{typeIcon} {typeName}";
            string dateText = _transaction.DateCreated.ToString("dd/MM/yyyy HH:mm");
            string totalText = $"{_transaction.FinalAmount:N0} ₫";
            
            // Creator
            string creatorName = $"User #{_transaction.CreatedByUserID}";
            try 
            {
                UserController userCnt = new UserController();
                var user = userCnt.GetUserById(_transaction.CreatedByUserID);
                if (user != null) creatorName = $"{user.FullName} ({user.Username})";
            } catch {}

            // Partner
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

            // Define Label Helper
            Label CreateValueLabel(string text, int x, int y, int width)
            {
                return new Label
                {
                    Text = text,
                    Left = x,
                    Top = y,
                    Width = width,
                    AutoSize = false,
                    Font = ThemeManager.Instance.FontRegular,
                    ForeColor = ThemeManager.Instance.TextPrimary
                };
            }

            // Row 1: ID | Type
            mainPanel.Controls.Add(CreateValueLabel($"Mã Giao Dịch: #{_transaction.TransactionID}", LEFT_MARGIN, currentY, INPUT_WIDTH_HALF));
            mainPanel.Controls.Add(CreateValueLabel($"Loại Giao Dịch: {typeText}", LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, currentY, INPUT_WIDTH_HALF));
            currentY += 25;

            // Row 2: Partner | Date
            mainPanel.Controls.Add(CreateValueLabel($"Đối tác: {partnerName}", LEFT_MARGIN, currentY, INPUT_WIDTH_HALF));
            mainPanel.Controls.Add(CreateValueLabel($"Ngày Tạo: {dateText}", LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, currentY, INPUT_WIDTH_HALF));
            currentY += 25;

            // Row 3: Creator | Value
            mainPanel.Controls.Add(CreateValueLabel($"Người Tạo: {creatorName}", LEFT_MARGIN, currentY, INPUT_WIDTH_HALF));
            mainPanel.Controls.Add(CreateValueLabel($"Tổng Giá Trị: {totalText}", LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, currentY, INPUT_WIDTH_HALF));
            currentY += 25 + 10; // Extra spacing

            // Row 5: Note
            string noteContent = string.IsNullOrEmpty(_transaction.Note) ? "(Không có)" : _transaction.Note;
            Label lblNoteDisplay = new Label
            {
                Text = $"Ghi chú: {noteContent}",
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH_FULL,
                AutoSize = false,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = ThemeManager.Instance.TextPrimary
            };
            mainPanel.Controls.Add(lblNoteDisplay);
            currentY += 25 + 10; // Extra spacing before grid

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

            // Calculate Grid Height
            int gridHeight = 650 - currentY - 80;
            if (gridHeight < 200) gridHeight = 200;

            // Grid
            dgvDetails = new DataGridView
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH_FULL,
                Height = gridHeight,
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
                HeaderText = "Số lượng", 
                DataPropertyName = "Quantity", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
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
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
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
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
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
            currentY += dgvDetails.Height + 30;



            // Button Print
            CustomButton btnPrint = new CustomButton
            {
                Text = $"{UIConstants.Icons.Print} In Phiếu",
                Left = LEFT_MARGIN + (INPUT_WIDTH_FULL - 250) / 2, // Spacing logic
                Top = currentY,
                Width = 120,
                ButtonStyleType = ButtonStyle.Outlined
            };
            btnPrint.Click += (s, e) => {
                 PrintService ps = new PrintService();
                 ps.PrintTransaction(_transaction);
            };
            mainPanel.Controls.Add(btnPrint);

            // Button Close
            btnClose = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Close} Đóng",
                Left = btnPrint.Left + 130, // Spacing 
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
            // int calculatedWidth = 800;
            // ClientSize = new Size(calculatedWidth, currentY);
            ClientSize = new Size(1024, 700);
            AutoScroll = true;
            
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
