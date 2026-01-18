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
        private Label lblType, lblDate, lblNote;
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
                Padding = new Padding(UIConstants.Spacing.Padding.Large)
            };

            const int LEFT_MARGIN = 20;
            const int LABEL_WIDTH = 110;
            const int VALUE_WIDTH = 400;
            int currentY = 20;
            int spacing = UIConstants.Spacing.Margin.Medium;

            // Transaction Type
            Label lblTypeLabel = new Label 
            { 
                Text = $"{UIConstants.Icons.Transaction} Loại Giao Dịch:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = LABEL_WIDTH,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };
            currentY += 25;

            lblType = new Label 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = VALUE_WIDTH, 
                Height = 30,
                AutoSize = false, 
                TextAlign = ContentAlignment.MiddleLeft, 
                BorderStyle = BorderStyle.FixedSingle, 
                BackColor = ThemeManager.Instance.BackgroundLight,
                Padding = new Padding(UIConstants.Spacing.Padding.Small),
                Font = ThemeManager.Instance.FontRegular
            };
            currentY += 30 + spacing;

            // Date
            Label lblDateLabel = new Label 
            { 
                Text = $"{UIConstants.Icons.Calendar} Ngày Tạo:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = LABEL_WIDTH,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.MiddleLeft
            };
            currentY += 25;

            lblDate = new Label 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = VALUE_WIDTH, 
                Height = 30,
                AutoSize = false, 
                TextAlign = ContentAlignment.MiddleLeft, 
                BorderStyle = BorderStyle.FixedSingle, 
                BackColor = ThemeManager.Instance.BackgroundLight,
                Padding = new Padding(UIConstants.Spacing.Padding.Small),
                Font = ThemeManager.Instance.FontRegular
            };
            currentY += 30 + spacing;

            // Note
            Label lblNoteLabel = new Label 
            { 
                Text = $"{UIConstants.Icons.FileText} Ghi Chú:", 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = LABEL_WIDTH,
                Font = ThemeManager.Instance.FontRegular,
                TextAlign = ContentAlignment.TopLeft
            };
            currentY += 25;

            lblNote = new Label 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = VALUE_WIDTH, 
                Height = 60,
                AutoSize = false, 
                TextAlign = ContentAlignment.TopLeft, 
                BorderStyle = BorderStyle.FixedSingle, 
                BackColor = ThemeManager.Instance.BackgroundLight,
                Padding = new Padding(UIConstants.Spacing.Padding.Small),
                Font = ThemeManager.Instance.FontRegular
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
                Height = 200,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = ThemeManager.Instance.BackgroundDefault,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false
            };

            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sản phẩm", DataPropertyName = "ProductName", Width = 200 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", DataPropertyName = "Quantity", Width = 90 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", DataPropertyName = "UnitPrice", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành tiền", DataPropertyName = "TotalPrice", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            currentY += 200 + UIConstants.Spacing.Margin.Large;

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

            mainPanel.Controls.Add(lblTypeLabel);
            mainPanel.Controls.Add(lblType);
            mainPanel.Controls.Add(lblDateLabel);
            mainPanel.Controls.Add(lblDate);
            mainPanel.Controls.Add(lblNoteLabel);
            mainPanel.Controls.Add(lblNote);
            mainPanel.Controls.Add(lblDetailsTitle);
            mainPanel.Controls.Add(dgvDetails);
            mainPanel.Controls.Add(btnClose);

            Controls.Add(mainPanel);

            ClientSize = new Size(620, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;

            Load += TransactionDetailForm_Load;
            ResumeLayout(false);
        }

        private void TransactionDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Set các giá trị cho label
                string typeIcon = _transaction.Type == "Import" ? UIConstants.Icons.Import : UIConstants.Icons.Export;
                lblType.Text = $"{typeIcon} {_transaction.Type}";
                lblDate.Text = $"{UIConstants.Icons.Clock} {_transaction.DateCreated:dd/MM/yyyy HH:mm}";
                lblNote.Text = _transaction.Note ?? "(Không có ghi chú)";
                
                // Hiển thị chi tiết giao dịch
                if (_transaction.Details != null && _transaction.Details.Count > 0)
                {
                    dgvDetails.DataSource = _transaction.Details;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải chi tiết giao dịch: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
