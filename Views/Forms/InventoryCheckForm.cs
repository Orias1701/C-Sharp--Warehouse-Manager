using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;
using WarehouseManagement.Services;

namespace WarehouseManagement.Views.Forms
{
    public class InventoryCheckForm : Form
    {
        private InventoryCheck _check;
        private bool _isNew;
        private InventoryCheckController _controller;
        private ProductController _productController;
        private UserController _userController;

        private CustomComboBox cmbProduct;
        private CustomTextArea txtNote;
        private DataGridView dgvDetails;
        private CustomButton btnAddProduct, btnSave, btnComplete, btnClose;
        
        private List<InventoryCheckDetail> _detailsList;
        private Dictionary<int, string> _productNames = new Dictionary<int, string>();

        public InventoryCheckForm(InventoryCheck check = null)
        {
            _check = check;
            _isNew = (check == null);
            _controller = new InventoryCheckController();
            _productController = new ProductController();
            _userController = new UserController();
            _detailsList = new List<InventoryCheckDetail>();

            if (!_isNew)
            {
                _detailsList = check.Details ?? new List<InventoryCheckDetail>();
            }

            // Pre-load product names for lookup
            try 
            {
               var products = _productController.GetAllProducts();
               _productNames = products.ToDictionary(p => p.ProductID, p => p.ProductName);
            }
            catch {}

            InitializeComponent();
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            Text = _isNew ? $"{UIConstants.Icons.Check} Tạo Phiếu Kiểm Kê Mới" : $"{UIConstants.Icons.Check} Chi Tiết Phiếu Kiểm Kê #{_check.CheckID}";
            
            // Main container
            CustomPanel mainPanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = false,
                Padding = new Padding(0)
            };

            // Layout constants
            const int LEFT_MARGIN = 40;
            const int INPUT_WIDTH = 944; // 1024 - 40 - 40
            const int COL_WIDTH = 462; // (944 - 20) / 2
            const int COL_GAP = 20;
            int currentY = 30;
            int inputSpacing = 20;

            // Info Header (User, Date, Status)
            string userName = "Unknown";
            if (_isNew)
            {
                userName = GlobalUser.CurrentUser?.Username;
            }
            else
            {
                // Fetch creator name
                try
                {
                    var u = _userController.GetUserById(_check.CreatedByUserID);
                    userName = u != null ? $"{u.FullName} ({u.Username})" : $"User #{_check.CreatedByUserID}";
                }
                catch 
                {
                    userName = _check.CreatedByUserID.ToString();
                }
            }

            string userText = $"Người tạo: {userName}";
            string dateText = $"Ngày tạo: {(_isNew ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : _check.CheckDate.ToString("dd/MM/yyyy HH:mm"))}";
            
            Label lblInfo = new Label
            {
                Text = $"{userText}    |    {dateText}",
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH,
                AutoSize = false,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = ThemeManager.Instance.TextPrimary
            };
            mainPanel.Controls.Add(lblInfo);
            currentY += 25;

            if (!_isNew)
            {
                Label lblStatus = new Label
                {
                    Text = $"Trạng thái: {_check.Status}",
                    Left = LEFT_MARGIN,
                    Top = currentY,
                    Width = INPUT_WIDTH,
                    AutoSize = false,
                    Font = ThemeManager.Instance.FontSmall,
                    ForeColor = _check.Status == "Completed" ? UIConstants.SemanticColors.Success : UIConstants.SemanticColors.Warning
                };
                mainPanel.Controls.Add(lblStatus);
                currentY += 25;
            }
            currentY += 10;

            // Note
            if (_isNew)
            {
                Label lblNote = new Label
                {
                    Text = "Ghi chú",
                    Left = LEFT_MARGIN,
                    Top = currentY,
                    AutoSize = true,
                    Font = ThemeManager.Instance.FontSmall,
                    ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                              UIConstants.PrimaryColor.Default.G, 
                                              UIConstants.PrimaryColor.Default.B)
                };
                mainPanel.Controls.Add(lblNote);
                currentY += 20;

                txtNote = new CustomTextArea
                {
                    Left = LEFT_MARGIN,
                    Top = currentY,
                    Width = INPUT_WIDTH,
                    Height = 60,
                    Placeholder = "Ghi chú kiểm kê...",
                };
                mainPanel.Controls.Add(txtNote);
                currentY += 60 + inputSpacing;
            }
            else
            {
                // In View Mode, showing Note as a Label like "Người tạo"
                string noteContent = string.IsNullOrEmpty(_check.Note) ? "(Không có)" : _check.Note;
                Label lblNoteDisplay = new Label
                {
                    Text = $"Ghi chú: {noteContent}",
                    Left = LEFT_MARGIN,
                    Top = currentY,
                    Width = INPUT_WIDTH,
                    AutoSize = false,
                    Font = ThemeManager.Instance.FontSmall,
                    ForeColor = ThemeManager.Instance.TextPrimary
                };
                mainPanel.Controls.Add(lblNoteDisplay);
                currentY += 25 + 10; // Similar spacing to status
            }

            // Product Selection (Only for New)
            if (_isNew)
            {
                Label lblAddProduct = new Label
                {
                    Text = "Thêm sản phẩm",
                    Left = LEFT_MARGIN,
                    Top = currentY,
                    AutoSize = true,
                    Font = ThemeManager.Instance.FontSmall,
                    ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                              UIConstants.PrimaryColor.Default.G, 
                                              UIConstants.PrimaryColor.Default.B)
                };
                mainPanel.Controls.Add(lblAddProduct);
                currentY += 20;

                cmbProduct = new CustomComboBox
                {
                    Left = LEFT_MARGIN,
                    Top = currentY,
                    Width = COL_WIDTH
                };
                LoadProducts();

                btnAddProduct = new CustomButton
                {
                    Text = "Thêm",
                    Left = LEFT_MARGIN + COL_WIDTH + COL_GAP,
                    Top = currentY,
                    Width = 150,
                    ButtonStyleType = ButtonStyle.FilledNoOutline
                };
                btnAddProduct.Click += BtnAddProduct_Click;

                mainPanel.Controls.Add(cmbProduct);
                mainPanel.Controls.Add(btnAddProduct);
                currentY += UIConstants.Sizes.InputHeight + inputSpacing;
            }

            // Grid
            Label lblDetail = new Label
            {
                Text = "Chi tiết kiểm kê",
                Left = LEFT_MARGIN,
                Top = currentY,
                AutoSize = true,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B)
            };
            mainPanel.Controls.Add(lblDetail);
            currentY += 20;

            // Calculate Grid Height to fit 650 total height
            // Top used: currentY
            // Bottom needed: 80 (Buttons + Padding)
            // Available: 650 - currentY - 80
            int gridHeight = 650 - currentY - 80;
            if (gridHeight < 200) gridHeight = 200; // Minimum safety

            dgvDetails = new DataGridView
            {
                Left = LEFT_MARGIN,
                Top = currentY,
                Width = INPUT_WIDTH,
                Height = gridHeight,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
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

            // Setup Columns -> Fit to 540 Width
            // Suggested: ID(60) + Name(300) + Sys(100) + Act(100) + Diff(100) + Reason(260) = 920
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "ProductID", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 4, 10, 4)
                }
            });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tên sản phẩm", 
                Name = "ProductName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 4, 10, 4) }
            }); 
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tồn Máy", 
                DataPropertyName = "SystemQuantity", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 10, 4)
                }
            });
            
            var colActual = new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tồn Thực", 
                DataPropertyName = "ActualQuantity", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 10, 4)
                }
            };
            colActual.DefaultCellStyle.BackColor = _isNew ? Color.White : Color.WhiteSmoke;
            colActual.ReadOnly = !_isNew; 
            dgvDetails.Columns.Add(colActual);

            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Chênh lệch", 
                DataPropertyName = "Difference", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(10, 4, 10, 4)
                }
            });
            
            var colReason = new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Lý do", 
                DataPropertyName = "Reason", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 4, 10, 4) }
            };
            colReason.ReadOnly = !_isNew;
            dgvDetails.Columns.Add(colReason);

            // Sync Header Alignment
            foreach (DataGridViewColumn col in dgvDetails.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 4, 10, 4);
            }

            dgvDetails.CellEndEdit += DgvDetails_CellEndEdit;
            dgvDetails.CellFormatting += DgvDetails_CellFormatting;
            
            // Apply Hover Effect
            Helpers.DataGridViewHelper.ApplyHoverEffect(dgvDetails);
            // Apply Selection Effect
            Helpers.DataGridViewHelper.ApplySelectionEffect(dgvDetails);

            mainPanel.Controls.Add(dgvDetails);
            currentY += dgvDetails.Height + 30;

            // Buttons - Centered
            // Calculate total width based on visible buttons
             
            btnClose = new CustomButton
            {
                Text = "Đóng",
                Width = 100,
                ButtonStyleType = ButtonStyle.Outlined
            };
            btnClose.Click += (s, e) => Close();

            // Print Button
            CustomButton btnPrint = new CustomButton
            {
                Text = $"{UIConstants.Icons.Print} In Phiếu",
                Width = 120,
                ButtonStyleType = ButtonStyle.Outlined
            };
            btnPrint.Click += BtnPrint_Click;
            
            if (_isNew)
            {
                btnSave = new CustomButton
                {
                    Text = "Lưu Nháp",
                    Width = 120,
                    ButtonStyleType = ButtonStyle.Outlined
                };
                btnSave.Click += BtnSave_Click;

                btnComplete = new CustomButton
                {
                    Text = "Duyệt Phiếu",
                    Width = 120,
                    ButtonStyleType = ButtonStyle.FilledNoOutline
                };
                btnComplete.Click += BtnComplete_Click;

                // Layout: Print | Close | Save | Complete
                // But user wants Print "when creating record" -> Print Draft?
                // Let's layout: Close | Print | Save | Complete
                int totalW = 100 + 10 + 120 + 10 + 120 + 10 + 120;
                int startX = LEFT_MARGIN + (INPUT_WIDTH - totalW) / 2;

                btnClose.Left = startX;
                btnClose.Top = currentY;

                btnPrint.Left = startX + 110;
                btnPrint.Top = currentY;

                btnSave.Left = startX + 110 + 130;
                btnSave.Top = currentY;

                btnComplete.Left = startX + 110 + 130 + 130;
                btnComplete.Top = currentY;

                mainPanel.Controls.Add(btnPrint);
                mainPanel.Controls.Add(btnSave);
                mainPanel.Controls.Add(btnComplete);
            }
            else if (_check.Status == "Pending")
            {
                btnComplete = new CustomButton
                {
                    Text = "Duyệt Phiếu",
                    Width = 120,
                    ButtonStyleType = ButtonStyle.FilledNoOutline
                };
                btnComplete.Click += BtnComplete_Click;

                int totalW = 100 + 10 + 120 + 10 + 120;
                int startX = LEFT_MARGIN + (INPUT_WIDTH - totalW) / 2;
                
                btnClose.Left = startX;
                btnClose.Top = currentY;

                btnPrint.Left = startX + 110;
                btnPrint.Top = currentY;

                btnComplete.Left = startX + 110 + 130;
                btnComplete.Top = currentY;

                mainPanel.Controls.Add(btnPrint);
                mainPanel.Controls.Add(btnComplete);
            }
            else
            {
                // Completed/Cancelled: Close | Print
                int totalW = 100 + 10 + 120;
                int startX = LEFT_MARGIN + (INPUT_WIDTH - totalW) / 2;

                btnClose.Left = startX;
                btnClose.Top = currentY;

                btnPrint.Left = startX + 110;
                btnPrint.Top = currentY;

                mainPanel.Controls.Add(btnPrint);
            }

            mainPanel.Controls.Add(btnClose);
            Controls.Add(mainPanel);

            // Form Size
            // int contentHeight = currentY + 35; // Bottom of buttons
            // int paddingBottom = 40;
            // int calculatedHeight = contentHeight + paddingBottom;

            // User requested fixed size
            ClientSize = new Size(1024, 650);
            
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;
            
            // Re-center buttons if needed?
            // Buttons logic (lines 332, 357) uses INPUT_WIDTH/LEFT_MARGIN.
            // Since INPUT_WIDTH etc are updated, buttons will center correctly relative to new width.
            
            RefreshGrid();
        }

        private Label CreateLabel(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Left = x, 
                Top = y,
                Width = width,
                Font = ThemeManager.Instance.FontRegular,
                AutoSize = false
            };
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productController.GetAllProducts();
                cmbProduct.DataSource = products;
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";
                cmbProduct.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải sản phẩm: " + ex.Message);
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex < 0) return;
            
            int productId = (int)cmbProduct.SelectedValue;
            if (_detailsList.Any(d => d.ProductID == productId))
            {
                MessageBox.Show("Sản phẩm đã có trong danh sách");
                return;
            }

            var product = _productController.GetProductById(productId);
            if (product == null) return;

            var detail = new InventoryCheckDetail
            {
                ProductID = productId,
                SystemQuantity = product.Quantity,
                ActualQuantity = product.Quantity, // Default to same
                Difference = 0,
                Reason = ""
            };
            _detailsList.Add(detail);
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dgvDetails.DataSource = null;
            dgvDetails.DataSource = _detailsList;
        }

        private void DgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _detailsList.Count)
            {
                var detail = _detailsList[e.RowIndex];
                if (dgvDetails.Columns[e.ColumnIndex].Name == "ProductName")
                {
                     if (_productNames.TryGetValue(detail.ProductID, out string pName))
                     {
                         e.Value = pName;
                     }
                     else
                     {
                         e.Value = "Unknown Product";
                     }
                     e.FormattingApplied = true;
                }
                
                if (dgvDetails.Columns[e.ColumnIndex].DataPropertyName == "Difference")
                {
                   int diff = detail.Difference;
                   if (diff > 0) e.CellStyle.ForeColor = UIConstants.SemanticColors.Success; // Found more
                   else if (diff < 0) e.CellStyle.ForeColor = UIConstants.SemanticColors.Error; // Missing
                }
            }
        }

        private void DgvDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Recalc difference if ActualQuantity changed
            if (dgvDetails.Columns[e.ColumnIndex].DataPropertyName == "ActualQuantity")
            {
                var detail = _detailsList[e.RowIndex];
                detail.Difference = detail.ActualQuantity - detail.SystemQuantity;
                RefreshGrid(); // Refresh to update Difference column
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveCheck("Pending");
        }

        private void BtnComplete_Click(object sender, EventArgs e)
        {
             if (MessageBox.Show("Bạn có chắc chắn muốn hoàn tất phiếu kiểm kê này?\nHệ thống sẽ tự động tạo các phiếu Nhập/Xuất để cân bằng tồn kho.", 
                 "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
             {
                 if (_isNew)
                 {
                     SaveCheck("Completed");
                 }
                 else
                 {
                     // Approve existing Pending check
                     try
                     {
                        _controller.ApproveCheck(_check.CheckID, GlobalUser.CurrentUser?.UserID ?? 0);
                        MessageBox.Show("Duyệt phiếu kiểm kê thành công!");
                        DialogResult = DialogResult.OK;
                        Close();
                     }
                     catch(Exception ex)
                     {
                         MessageBox.Show("Lỗi: " + ex.Message);
                     }
                 }
             }
        }

        private void SaveCheck(string status)
        {
            if (_detailsList.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một sản phẩm");
                return;
            }

            try
            {
                string note = txtNote.Text;
                int userId = GlobalUser.CurrentUser?.UserID ?? 0;
                
                int checkId = _controller.CreateCheck(userId, note, _detailsList, status);
                
                MessageBox.Show($"Đã lưu phiếu kiểm kê #{checkId} ({status})");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu phiếu: " + ex.Message);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintService ps = new PrintService();
            if (_isNew)
            {
                // Print Draft
                 ps.PrintPendingCheck(_detailsList, txtNote.Text, GlobalUser.CurrentUser?.UserID ?? 0);
            }
            else
            {
                // Print Saved Check
                ps.PrintInventoryCheck(_check);
            }
        }
    }
}
