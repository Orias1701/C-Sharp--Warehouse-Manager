using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views
{
    /// <summary>
    /// Form hiển thị chi tiết giao dịch (Nhập/Xuất kho) - Chế độ xem
    /// </summary>
    public partial class TransactionDetailForm : Form
    {
        private StockTransaction _transaction;
        private Label lblType, lblDate, lblNote;
        private DataGridView dgvDetails;
        private Button btnClose;

        public TransactionDetailForm(StockTransaction transaction)
        {
            InitializeComponent();
            _transaction = transaction;
            Text = $"Chi Tiết Giao Dịch #{transaction.TransactionID}";
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Layout standard: Label 100px, Input 300px, spacing 20px
            const int LABEL_WIDTH = 100;
            const int INPUT_WIDTH = 300;
            const int LABEL_LEFT = 20;
            const int INPUT_LEFT = 130;
            const int ITEM_SPACING = 40;
            const int BUTTON_WIDTH = 100;
            const int BUTTON_HEIGHT = 35;

            // Labels và controls (Read-only)
            Label lblTypeLabel = new Label { Text = "Loại Giao Dịch:", Left = LABEL_LEFT, Top = 20, Width = LABEL_WIDTH, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            lblType = new Label { Left = INPUT_LEFT, Top = 20, Width = INPUT_WIDTH, Height = 25, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, BorderStyle = BorderStyle.FixedSingle, BackColor = System.Drawing.Color.White };

            Label lblDateLabel = new Label { Text = "Ngày Tạo:", Left = LABEL_LEFT, Top = 20 + ITEM_SPACING, Width = LABEL_WIDTH, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            lblDate = new Label { Left = INPUT_LEFT, Top = 20 + ITEM_SPACING, Width = INPUT_WIDTH, Height = 25, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, BorderStyle = BorderStyle.FixedSingle, BackColor = System.Drawing.Color.White };

            Label lblNoteLabel = new Label { Text = "Ghi Chú:", Left = LABEL_LEFT, Top = 20 + ITEM_SPACING * 2, Width = LABEL_WIDTH, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.TopLeft };
            lblNote = new Label { Left = INPUT_LEFT, Top = 20 + ITEM_SPACING * 2, Width = INPUT_WIDTH, Height = 50, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.TopLeft, BorderStyle = BorderStyle.FixedSingle, BackColor = System.Drawing.Color.White, Padding = new Padding(5) };

            // DataGridView - Read-only
            dgvDetails = new DataGridView
            {
                Left = LABEL_LEFT,
                Top = 20 + ITEM_SPACING * 3 + 30,
                Width = 520,
                Height = 180,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sản phẩm", DataPropertyName = "ProductName", Width = 250 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số lượng", DataPropertyName = "Quantity", Width = 80 });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn giá", DataPropertyName = "UnitPrice", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "C" } });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thành tiền", DataPropertyName = "TotalPrice", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "C" } });

            // Nút đóng
            btnClose = new Button { Text = "✖️ Đóng", Left = INPUT_LEFT, Top = 20 + ITEM_SPACING * 4 + 220, Width = BUTTON_WIDTH, Height = BUTTON_HEIGHT, DialogResult = DialogResult.OK };
            btnClose.Click += (s, e) => Close();

            Controls.Add(lblTypeLabel);
            Controls.Add(lblType);
            Controls.Add(lblDateLabel);
            Controls.Add(lblDate);
            Controls.Add(lblNoteLabel);
            Controls.Add(lblNote);
            Controls.Add(btnClose);
            Controls.Add(dgvDetails);

            Width = 600;
            Height = 580;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Load += TransactionDetailForm_Load;
            ResumeLayout(false);
        }

        private void TransactionDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Set các giá trị cho label
                lblType.Text = _transaction.Type;
                lblDate.Text = _transaction.DateCreated.ToString("dd/MM/yyyy HH:mm");
                lblNote.Text = _transaction.Note ?? "";
                
                // Hiển thị chi tiết giao dịch
                if (_transaction.Details != null && _transaction.Details.Count > 0)
                {
                    dgvDetails.DataSource = _transaction.Details;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết giao dịch: " + ex.Message);
            }
        }
    }
}


