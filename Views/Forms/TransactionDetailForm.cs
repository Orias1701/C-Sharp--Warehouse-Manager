using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Forms
{
    /// <summary>
    /// Form hiển thị chi tiết giao dịch (Nhập/Xuất kho) - Chế độ xem
    /// </summary>
    public partial class TransactionDetailForm : Form
    {
        private StockTransaction _transaction;
        private CustomTextBox txtTransactionID, txtType, txtDate, txtTotalValue, txtCreatedBy;
        private CustomTextArea txtNote;
        private DataGridView dgvDetails;
        private CustomButton btnClose;

        public TransactionDetailForm(StockTransaction transaction)
        {
            InitializeComponent();
            _transaction = transaction;
            Text = $"{UIConstants.Icons.FileText} Chi Tiết Giao Dịch #{transaction.TransactionID}";
            
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
                Padding = new Padding(0)
            };

            const int LEFT_MARGIN = 40;
            int currentY = 20;
            int spacing = UIConstants.Spacing.Margin.Medium;

            // Mã Giao Dịch và Loại Giao Dịch (cùng hàng)
            Label lblTransactionIDLabel = new Label 
            { 
                Text = "Mã Giao Dịch:",
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B)
            };

            Label lblTypeLabel = new Label 
            { 
                Text = "Loại Giao Dịch:",
                Left = LEFT_MARGIN + 250, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B)
            };
            currentY += 20;

            txtTransactionID = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 220,
                ReadOnly = true,
                TabStop = false
            };

            txtType = new CustomTextBox 
            { 
                Left = LEFT_MARGIN + 250, 
                Top = currentY, 
                Width = 270,
                ReadOnly = true,
                TabStop = false
            };
            currentY += UIConstants.Sizes.InputHeight + spacing;

            // Ngày Tạo và Người Tạo (cùng hàng)
            Label lblDateLabel = new Label 
            { 
                Text = "Ngày Tạo:",
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B)
            };

            Label lblCreatedByLabel = new Label 
            { 
                Text = "Người Tạo:",
                Left = LEFT_MARGIN + 250, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B)
            };
            currentY += 20;

            txtDate = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 220,
                ReadOnly = true,
                TabStop = false
            };

            txtCreatedBy = new CustomTextBox 
            { 
                Left = LEFT_MARGIN + 250, 
                Top = currentY, 
                Width = 270,
                ReadOnly = true,
                TabStop = false
            };
            currentY += UIConstants.Sizes.InputHeight + spacing;

            // Tổng Giá Trị
            Label lblTotalValueLabel = new Label 
            { 
                Text = "Tổng Giá Trị:",
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B)
            };
            currentY += 20;

            txtTotalValue = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 520,
                ReadOnly = true,
                TabStop = false
            };
            currentY += UIConstants.Sizes.InputHeight + spacing;

            // Note
            Label lblNoteLabel = new Label 
            { 
                Text = "Ghi chú:",
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 100,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B)
            };
            currentY += 20;

            txtNote = new CustomTextArea 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 520, 
                Height = 60,
                ReadOnly = true,
                TabStop = false
            };
            currentY += 60 + spacing + 10;

            // DataGridView title
            Label lblDetailsTitle = new Label
            {
                Text = $"{UIConstants.Icons.List} Chi Tiết Sản Phẩm:",
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = 200,
                Font = ThemeManager.Instance.FontBold
            };
            currentY += 25;

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

            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sản phẩm", DataPropertyName = "ProductName", Width = 200 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", DataPropertyName = "Quantity", Width = 90 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", DataPropertyName = "UnitPrice", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành tiền", DataPropertyName = "TotalPrice", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            currentY += 180 + spacing;

            // Close button
            btnClose = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Close} Đóng", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = 120,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            btnClose.Click += (s, e) => {
                DialogResult = DialogResult.OK;
                Close();
            };

            mainPanel.Controls.Add(lblTransactionIDLabel);
            mainPanel.Controls.Add(txtTransactionID);
            mainPanel.Controls.Add(lblTypeLabel);
            mainPanel.Controls.Add(txtType);
            mainPanel.Controls.Add(lblDateLabel);
            mainPanel.Controls.Add(txtDate);
            mainPanel.Controls.Add(lblTotalValueLabel);
            mainPanel.Controls.Add(txtTotalValue);
            mainPanel.Controls.Add(lblCreatedByLabel);
            mainPanel.Controls.Add(txtCreatedBy);
            mainPanel.Controls.Add(lblNoteLabel);
            mainPanel.Controls.Add(txtNote);
            mainPanel.Controls.Add(lblDetailsTitle);
            mainPanel.Controls.Add(dgvDetails);
            mainPanel.Controls.Add(btnClose);

            Controls.Add(mainPanel);

            ClientSize = new Size(620, 635);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;
            
            // Set nút Đóng là default button để focus vào đó
            AcceptButton = btnClose;
            CancelButton = btnClose;

            Load += TransactionDetailForm_Load;
            ResumeLayout(false);
        }

        private void TransactionDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Set các giá trị
                txtTransactionID.Text = $"#{_transaction.TransactionID}";
                
                string typeIcon = _transaction.Type == "Import" ? UIConstants.Icons.Import : UIConstants.Icons.Export;
                string typeName = _transaction.Type == "Import" ? "Nhập" : "Xuất";
                txtType.Text = $"{typeIcon} {typeName}";
                
                txtDate.Text = _transaction.DateCreated.ToString("dd/MM/yyyy HH:mm");
                txtTotalValue.Text = $"{_transaction.TotalValue:N0} ₫";
                txtCreatedBy.Text = $"User ID: {_transaction.CreatedByUserID}";
                txtNote.Text = _transaction.Note ?? "(Không có ghi chú)";
                
                // Hiển thị chi tiết giao dịch
                if (_transaction.Details != null && _transaction.Details.Count > 0)
                {
                    dgvDetails.DataSource = _transaction.Details;
                }
                
                // Force focus vào nút Đóng - dùng ActiveControl
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
