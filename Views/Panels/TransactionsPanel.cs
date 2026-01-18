using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Forms;
using WarehouseManagement.UI;

namespace WarehouseManagement.Views.Panels
{
    public class TransactionsPanel : Panel, ISearchable
    {
        private DataGridView dgvTransactions;
        private InventoryController _inventoryController;
        private List<StockTransaction> allTransactions;

        public TransactionsPanel()
        {
            _inventoryController = new InventoryController();
            InitializeComponent();
            SettingsForm.SettingsChanged += (s, e) => LoadData();
            
            // Subscribe to theme changes
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.Instance.BackgroundDefault;

            dgvTransactions = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = ThemeManager.Instance.BackgroundDefault,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToResizeRows = false,
                Font = ThemeManager.Instance.FontRegular,
                RowTemplate = { Height = UIConstants.Sizes.TableRowHeight },
                ColumnHeadersHeight = UIConstants.Sizes.TableHeaderHeight,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = ThemeManager.Instance.FontBold,
                    BackColor = ThemeManager.Instance.BackgroundLight,
                    ForeColor = ThemeManager.Instance.TextPrimary,
                    Padding = new Padding(UIConstants.Spacing.Padding.Small)
                }
            };

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "TransactionID", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } 
            });
            
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Transaction} Loại", 
                DataPropertyName = "Type", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } 
            });
            
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Calendar} Ngày", 
                DataPropertyName = "DateCreated", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } 
            });
            
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Money} Tổng Giá Trị", 
                DataPropertyName = "TotalValue", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0", 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                } 
            });
            
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.FileText} Ghi chú", 
                DataPropertyName = "Note", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            dgvTransactions.Columns.Add(new DataGridViewButtonColumn 
            { 
                HeaderText = UIConstants.Icons.Eye, 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForButtonValue = true, 
                Text = UIConstants.Icons.Eye 
            });

            dgvTransactions.CellDoubleClick += DgvTransactions_CellDoubleClick;
            dgvTransactions.CellClick += DgvTransactions_CellClick;
            dgvTransactions.CellFormatting += DgvTransactions_CellFormatting;
            dgvTransactions.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                    LoadData();
            };

            Controls.Add(dgvTransactions);
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvTransactions.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvTransactions.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvTransactions.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundLight;
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
        }

        public void LoadData()
        {
            try
            {
                allTransactions = _inventoryController.GetAllTransactions(SettingsForm.ShowHiddenItems);
                dgvTransactions.DataSource = allTransactions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải giao dịch: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Search(string searchText)
        {
            try
            {
                string search = searchText.ToLower();
                List<StockTransaction> filtered = allTransactions.FindAll(t => 
                    t.TransactionID.ToString().Contains(search) || 
                    t.Type.ToLower().Contains(search) || 
                    (t.Note != null && t.Note.ToLower().Contains(search)));
                dgvTransactions.DataSource = filtered;
            }
            catch { }
        }

        private void DgvTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int transactionId = (int)dgvTransactions.Rows[e.RowIndex].Cells[0].Value;

            try
            {
                StockTransaction transaction = _inventoryController.GetTransactionById(transactionId);

                if (transaction != null)
                {
                    TransactionDetailForm form = new TransactionDetailForm(transaction);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show($"{UIConstants.Icons.Error} Không tìm thấy giao dịch", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải giao dịch: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvTransactions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Column 5 is the hide button
            if (e.ColumnIndex == 5)
            {
                int transactionId = (int)dgvTransactions.Rows[e.RowIndex].Cells[0].Value;
                
                DialogResult result = MessageBox.Show(
                    $"{UIConstants.Icons.Question} Bạn chắc chắn muốn đảo trạng thái giao dịch này?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _inventoryController.HideTransaction(transactionId);
                        MessageBox.Show($"{UIConstants.Icons.Success} Trạng thái giao dịch đã được thay đổi.", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{UIConstants.Icons.Error} Lỗi ẩn giao dịch: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            // Other columns show details
            int id = (int)dgvTransactions.Rows[e.RowIndex].Cells[0].Value;

            try
            {
                StockTransaction transaction = _inventoryController.GetTransactionById(id);
                if (transaction != null)
                {
                    TransactionDetailForm form = new TransactionDetailForm(transaction);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show($"{UIConstants.Icons.Error} Không tìm thấy giao dịch", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải giao dịch: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvTransactions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Format Type column to show Vietnamese names with icons
            if (e.ColumnIndex == 1 && dgvTransactions.Rows[e.RowIndex].DataBoundItem is StockTransaction transaction)
            {
                if (e.Value != null)
                {
                    string icon = transaction.Type == "Import" ? UIConstants.Icons.Import : UIConstants.Icons.Export;
                    string text = transaction.Type == "Import" ? "Nhập" : transaction.Type == "Export" ? "Xuất" : transaction.Type;
                    e.Value = $"{icon} {text}";
                    e.FormattingApplied = true;
                    
                    // Set color based on type
                    if (transaction.Type == "Import")
                    {
                        e.CellStyle.ForeColor = UIConstants.SemanticColors.Success;
                    }
                    else if (transaction.Type == "Export")
                    {
                        e.CellStyle.ForeColor = UIConstants.SemanticColors.Info;
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.Instance.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }
    }
}
