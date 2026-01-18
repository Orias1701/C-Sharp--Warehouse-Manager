using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

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
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = ThemeManager.Instance.BackgroundLight,
                    ForeColor = ThemeManager.Instance.TextPrimary,
                    Font = ThemeManager.Instance.FontBold,
                    Padding = new Padding(10, 0, 0, 0)
                },
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToResizeRows = false,
                Font = ThemeManager.Instance.FontRegular,
                RowTemplate = { Height = UIConstants.Sizes.TableRowHeight },
                ColumnHeadersHeight = UIConstants.Sizes.TableHeaderHeight
            };

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "TransactionID", 
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } 
            });
            
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Loại", 
                DataPropertyName = "Type", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } 
            });
            
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Ngày", 
                DataPropertyName = "DateCreated", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } 
            });
            
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tổng Giá Trị", 
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
                HeaderText = "Ghi chú", 
                DataPropertyName = "Note", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            dgvTransactions.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Transaction,
                LinkBehavior = LinkBehavior.NeverUnderline,
                LinkColor = ThemeManager.Instance.TextPrimary,
                ActiveLinkColor = ThemeManager.Instance.PrimaryDefault,
                VisitedLinkColor = ThemeManager.Instance.TextPrimary,
                TrackVisitedState = false,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvTransactions.CellDoubleClick += DgvTransactions_CellDoubleClick;
            dgvTransactions.CellClick += DgvTransactions_CellClick;
            dgvTransactions.CellFormatting += DgvTransactions_CellFormatting;
            dgvTransactions.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                    LoadData();
            };

            CustomPanel tablePanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                HasShadow = true,
                ShowBorder = false,
                Padding = new Padding(10),
                BorderRadius = UIConstants.Borders.RadiusMedium,
                BackColor = ThemeManager.Instance.BackgroundDefault
            };
            tablePanel.Controls.Add(dgvTransactions);
            Controls.Add(tablePanel);
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
                    string text = transaction.Type == "Import" ? "Nhập" : transaction.Type == "Export" ? "Xuất" : transaction.Type;
                    e.Value = text;
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
