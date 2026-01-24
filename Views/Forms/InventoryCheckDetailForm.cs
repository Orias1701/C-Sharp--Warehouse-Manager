using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;
using WarehouseManagement.Controllers;
using WarehouseManagement.Services;
using System.Linq; // Added for Data processing

namespace WarehouseManagement.Views.Forms
{
    public partial class InventoryCheckDetailForm : Form
    {
        private InventoryCheck _check;
        private DataGridView dgvDetails;
        private CustomButton btnClose;
        private CustomButton btnPrint;
        private CustomButton btnApprove; // Extra button for Pending checks
        private UserController _userController;
        private InventoryCheckController _checkController;
        private ProductController _productController;
        
        public InventoryCheckDetailForm(InventoryCheck check)
        {
            _check = check;
            _userController = new UserController();
            _checkController = new InventoryCheckController();
            _productController = new ProductController();
            InitializeComponent();
            
            Text = $"{UIConstants.Icons.Check} Chi Tiết Kiểm Kê #{check.CheckID}";
            
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
            int inputSpacing = 20;

            // Fetch Data for Labels
            string typeIcon = UIConstants.Icons.Check;
            string typeName = "Phiếu Kiểm Kê";
            string typeText = $"{typeIcon} {typeName}";
            string dateText = _check.CheckDate.ToString("dd/MM/yyyy HH:mm");
            
            // Status Text
            string statusText = _check.Status;
            if (statusText == "Pending") statusText = "Đang chờ duyệt";
            else if (statusText == "Approved" || statusText == "Completed") statusText = "Đã duyệt";
            else if (statusText == "Cancelled") statusText = "Đã hủy";

            // Creator
            string creatorName = $"User #{_check.CreatedByUserID}";
            try 
            {
                var user = _userController.GetUserById(_check.CreatedByUserID);
                if (user != null) creatorName = $"{user.FullName} ({user.Username})";
            } catch {}

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
            mainPanel.Controls.Add(CreateValueLabel($"Mã Phiếu: #{_check.CheckID}", LEFT_MARGIN, currentY, INPUT_WIDTH_HALF));
            mainPanel.Controls.Add(CreateValueLabel($"Loại Phiếu: {typeText}", LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, currentY, INPUT_WIDTH_HALF));
            currentY += 25;

            // Row 2: Creator | Date
            mainPanel.Controls.Add(CreateValueLabel($"Người Tạo: {creatorName}", LEFT_MARGIN, currentY, INPUT_WIDTH_HALF));
            mainPanel.Controls.Add(CreateValueLabel($"Ngày Tạo: {dateText}", LEFT_MARGIN + INPUT_WIDTH_HALF + COLUMN_GAP, currentY, INPUT_WIDTH_HALF));
            currentY += 25;

            // Row 3: Status | (Empty or Note Preview)
            mainPanel.Controls.Add(CreateValueLabel($"Trạng Thái: {statusText}", LEFT_MARGIN, currentY, INPUT_WIDTH_HALF));
            // Placeholder for 4th item if needed
            currentY += 25 + 10; // Extra spacing

            // Row 4: Note
            string noteContent = string.IsNullOrEmpty(_check.Note) ? "(Không có)" : _check.Note;
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

            // Columns - Adapted for InventoryCheck
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "ProductID", 
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(10, 4, 10, 4) }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Sản phẩm", 
                Name = "ProductName", // Managed manually via loaded products
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 4, 10, 4) }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tồn Máy", 
                DataPropertyName = "SystemQuantity", 
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 30, 4)
                }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tồn Thực", 
                DataPropertyName = "ActualQuantity", 
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 30, 4)
                } 
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Chênh Lệch", 
                DataPropertyName = "Difference", 
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
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
            
            // Format Cell Event
            dgvDetails.CellFormatting += DgvDetails_CellFormatting;

            // Apply Hover Effect
            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvDetails);
            // Apply Selection Effect
            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvDetails);

            mainPanel.Controls.Add(dgvDetails);
            currentY += dgvDetails.Height + 30;

            // Buttons Logic
            // If Pending, show Approve button.
            // Alignment: Center? Or Right? TransactionDetail uses Center-ish.
            
            btnPrint = new CustomButton
            {
                Text = $"{UIConstants.Icons.Print} In Phiếu",
                Width = 120,
                ButtonStyleType = ButtonStyle.Outlined,
                Top = currentY
            };
            btnPrint.Click += (s, e) => {
                 PrintService ps = new PrintService();
                 ps.PrintInventoryCheck(_check);
            };

            btnClose = new CustomButton 
            { 
                Text = $"{UIConstants.Icons.Close} Đóng",
                Width = 120,
                ButtonStyleType = ButtonStyle.FilledNoOutline,
                Top = currentY
            };
            btnClose.Click += (s, e) => {
                DialogResult = DialogResult.OK;
                Close();
            };

            if (_check.Status == "Pending")
            {
                btnApprove = new CustomButton 
                { 
                    Text = $"{UIConstants.Icons.Check} Duyệt",
                    Width = 120,
                    ButtonStyleType = ButtonStyle.FilledNoOutline, // Highlight action
                    Top = currentY
                };
                btnApprove.Click += BtnApprove_Click;
                
                // Change Close to Outlined so Approve stands out
                btnClose.ButtonStyleType = ButtonStyle.Outlined;

                // Layout: Print | Close | Approve
                int totalW = 120 + 20 + 120 + 20 + 120;
                int startX = LEFT_MARGIN + (INPUT_WIDTH_FULL - totalW) / 2;
                
                btnPrint.Left = startX;
                btnClose.Left = startX + 140;
                btnApprove.Left = startX + 280;

                mainPanel.Controls.Add(btnPrint);
                mainPanel.Controls.Add(btnClose);
                mainPanel.Controls.Add(btnApprove);
            }
            else
            {
                // Layout: Print | Close
                // Match TransactionDetailForm spacing
                // TransactionDetailForm: Print (Left) | Close (Right) center-ish
                
                 // TransactionDetailForm Logic:
                 // btnPrint.Left = LEFT_MARGIN + (INPUT_WIDTH_FULL - 250) / 2;
                 // btnClose.Left = btnPrint.Left + 130;
                 
                 btnPrint.Left = LEFT_MARGIN + (INPUT_WIDTH_FULL - 250) / 2;
                 btnClose.Left = btnPrint.Left + 130;

                 mainPanel.Controls.Add(btnPrint);
                 mainPanel.Controls.Add(btnClose);
            }

            currentY += 35 + 30; // Button height + padding

            Controls.Add(mainPanel);

            // Size
            ClientSize = new Size(1024, 700);
            AutoScroll = true;
            
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;
            
            AcceptButton = btnClose;
            CancelButton = btnClose;

            Load += InventoryCheckDetailForm_Load;
            ResumeLayout(false);
        }

        private void InventoryCheckDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (_check.Details != null && _check.Details.Count > 0)
                {
                    // Pre-load product names is better, but cell formatting handles it too if efficient
                    dgvDetails.DataSource = _check.Details;
                }
                ActiveControl = btnClose;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải chi tiết: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
             if (e.RowIndex < 0) return;
             
             // Lookup ProductName
             if (dgvDetails.Columns[e.ColumnIndex].Name == "ProductName")
             {
                 var detail = dgvDetails.Rows[e.RowIndex].DataBoundItem as InventoryCheckDetail;
                 if (detail != null)
                 {
                      var p = _productController.GetProductById(detail.ProductID); // Could be slow if many rows, but safe
                      e.Value = p?.ProductName ?? "Unknown";
                      e.FormattingApplied = true;
                 }
             }
             // Colorize Difference
             else if (dgvDetails.Columns[e.ColumnIndex].DataPropertyName == "Difference")
             {
                 if (e.Value is int diff)
                 {
                     if (diff > 0) e.CellStyle.ForeColor = UIConstants.SemanticColors.Success;
                     else if (diff < 0) e.CellStyle.ForeColor = UIConstants.SemanticColors.Error;
                 }
             }
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
             if (MessageBox.Show("Bạn có chắc chắn muốn duyệt phiếu kiểm kê này?", 
                 "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
             {
                 try
                 {
                    _checkController.ApproveCheck(_check.CheckID, GlobalUser.CurrentUser?.UserID ?? 0);
                    MessageBox.Show($"{UIConstants.Icons.Success} Duyệt phiếu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                 }
                 catch(Exception ex)
                 {
                      MessageBox.Show($"{UIConstants.Icons.Error} Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
             }
        }
    }
}
