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
    public class SuppliersPanel : Panel, ISearchable
    {
        private DataGridView dgvSuppliers;
        private SupplierController _controller;
        private List<Supplier> _allSuppliers;

        public SuppliersPanel()
        {
            _controller = new SupplierController();
            InitializeComponent();
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.Instance.BackgroundDefault;

            // DataGridView
            dgvSuppliers = new DataGridView
            {
                Dock = DockStyle.Fill,
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
                RowTemplate = { Height = UIConstants.Sizes.TableRowHeight },
                ColumnHeadersHeight = UIConstants.Sizes.TableHeaderHeight
            };

            // Columns
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "SupplierID", 
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter, 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });
            
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tên Nhà Cung Cấp", 
                DataPropertyName = "SupplierName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Số Điện Thoại", 
                DataPropertyName = "Phone", 
                Width = 150,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter, 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Email", 
                DataPropertyName = "Email", 
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Địa Chỉ", 
                DataPropertyName = "Address", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Padding = new Padding(10, 5, 30, 5) 
                }
            });

            // Action Column: Hide/Status (Using Transaction Icon to match Standard)
            dgvSuppliers.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Transaction,
                LinkColor = ThemeManager.Instance.TextPrimary,
                ActiveLinkColor = ThemeManager.Instance.PrimaryDefault,
                VisitedLinkColor = ThemeManager.Instance.TextPrimary,
                LinkBehavior = LinkBehavior.NeverUnderline,
                TrackVisitedState = false,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 10, 5)
                }
            });

            // Action Column: Delete (Using Cancel Icon to match Standard)
            dgvSuppliers.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Cancel,
                LinkColor = ThemeManager.Instance.TextPrimary,
                ActiveLinkColor = UIConstants.SemanticColors.Error,
                VisitedLinkColor = ThemeManager.Instance.TextPrimary,
                LinkBehavior = LinkBehavior.NeverUnderline,
                TrackVisitedState = false,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(10, 5, 10, 5)
                }
            });

            // Header Styling Sync
            foreach (DataGridViewColumn col in dgvSuppliers.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 5, 10, 5);
            }

            dgvSuppliers.CellClick += DgvSuppliers_CellClick;

            // Container Panel
            CustomPanel tablePanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                HasShadow = true,
                ShowBorder = false,
                Padding = new Padding(10),
                BorderRadius = UIConstants.Borders.RadiusMedium,
                BackColor = ThemeManager.Instance.BackgroundDefault
            };
            tablePanel.Controls.Add(dgvSuppliers);
            Controls.Add(tablePanel);

            LoadData();
        }

        private void OnThemeChanged(object sender, EventArgs e) => ApplyTheme();

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvSuppliers.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvSuppliers.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvSuppliers.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvSuppliers.DefaultCellStyle.SelectionBackColor = UIConstants.PrimaryColor.Light;
            dgvSuppliers.DefaultCellStyle.SelectionForeColor = ThemeManager.Instance.TextPrimary;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.BackColor = UIConstants.PrimaryColor.Default;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.SelectionBackColor = UIConstants.PrimaryColor.Default;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }

        public void LoadData()
        {
            try
            {
                _allSuppliers = _controller.GetAllSuppliers();
                dgvSuppliers.DataSource = _allSuppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách nhà cung cấp: " + ex.Message);
            }
        }

        public void Search(string text)
        {
            if (_allSuppliers == null) return;
            string keyword = text.ToLower();
            var filtered = _allSuppliers.FindAll(s => 
                s.SupplierID.ToString().Contains(keyword) || 
                s.SupplierName.ToLower().Contains(keyword) || 
                (s.Phone != null && s.Phone.Contains(keyword)) ||
                (s.Email != null && s.Email.ToLower().Contains(keyword)));
            dgvSuppliers.DataSource = filtered;
        }

        private void DgvSuppliers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int supplierId = (int)dgvSuppliers.Rows[e.RowIndex].Cells[0].Value;
            string supplierName = dgvSuppliers.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";

            // Hide/Status Column (Index 5)
            if (e.ColumnIndex == 5)
            {
                DialogResult result = MessageBox.Show(
                    $"{UIConstants.Icons.Question} Bạn chắc chắn muốn đảo trạng thái nhà cung cấp '{supplierName}'?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _controller.SoftDeleteSupplier(supplierId);
                        MessageBox.Show($"{UIConstants.Icons.Success} Trạng thái nhà cung cấp đã được thay đổi.", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{UIConstants.Icons.Error} Lỗi thay đổi trạng thái: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            // Delete Column (Index 6)
            if (e.ColumnIndex == 6)
            {
                DialogResult result = MessageBox.Show(
                    $"{UIConstants.Icons.Warning} Bạn chắc chắn muốn xóa nhà cung cấp '{supplierName}'?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _controller.SoftDeleteSupplier(supplierId);
                        MessageBox.Show($"{UIConstants.Icons.Success} Nhà cung cấp đã được xóa thành công.", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{UIConstants.Icons.Error} Lỗi xóa nhà cung cấp: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            // Other columns open Edit
            var supplierToEdit = dgvSuppliers.Rows[e.RowIndex].DataBoundItem as Supplier;
            if (supplierToEdit != null)
            {
                SupplierForm form = new SupplierForm(supplierToEdit);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
    }
}
