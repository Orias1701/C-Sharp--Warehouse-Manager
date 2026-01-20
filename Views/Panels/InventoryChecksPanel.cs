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
    public class InventoryChecksPanel : Panel, ISearchable
    {
        private DataGridView dgvChecks;
        private InventoryCheckController _controller;
        private List<InventoryCheck> _allChecks;

        public InventoryChecksPanel()
        {
            _controller = new InventoryCheckController();
            InitializeComponent();
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.Instance.BackgroundDefault;
            Padding = new Padding(20);

            // Top Panel (Action Bar)
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.Transparent };
            
            CustomButton btnAdd = new CustomButton
            {
                Text = $"{UIConstants.Icons.Add} Tạo Phiếu Kiểm Kê",
                Width = 220,
                Height = 35,
                Top = 5,
                Left = 0,
                ButtonStyleType = ButtonStyle.Filled
            };
            btnAdd.Click += BtnAdd_Click;
            topPanel.Controls.Add(btnAdd);
            Controls.Add(topPanel);

            // DataGridView
            dgvChecks = new DataGridView
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
            dgvChecks.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Mã Phiếu", 
                DataPropertyName = "CheckID", 
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(10, 5, 10, 5) }
            });
            
            dgvChecks.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Ngày Kiểm", 
                DataPropertyName = "CheckDate", 
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm", Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(10, 5, 10, 5) }
            });

            dgvChecks.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Người Kiểm", 
                DataPropertyName = "CreatedByUserID", 
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(10, 5, 10, 5) }
            });

            dgvChecks.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Trạng Thái", 
                DataPropertyName = "Status", 
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(10, 5, 10, 5) }
            });

            dgvChecks.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Ghi Chú", 
                DataPropertyName = "Note", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 5, 10, 5) }
            });

            // Action Column (View Details)
            dgvChecks.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Detail, // Using Detail icon instead of Transaction
                LinkColor = ThemeManager.Instance.PrimaryDefault,
                ActiveLinkColor = ThemeManager.Instance.PrimaryDefault,
                VisitedLinkColor = ThemeManager.Instance.PrimaryDefault,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Header Styling Sync
            foreach (DataGridViewColumn col in dgvChecks.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 5, 10, 5);
            }

            dgvChecks.CellDoubleClick += DgvChecks_CellDoubleClick;
            dgvChecks.CellClick += DgvChecks_CellClick;
            dgvChecks.CellFormatting += DgvChecks_CellFormatting;

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
            tablePanel.Controls.Add(dgvChecks);
            Controls.Add(tablePanel);
            
            topPanel.SendToBack();

            LoadData();
        }

        private void OnThemeChanged(object sender, EventArgs e) => ApplyTheme();

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvChecks.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvChecks.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvChecks.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvChecks.DefaultCellStyle.SelectionBackColor = UIConstants.PrimaryColor.Light;
            dgvChecks.DefaultCellStyle.SelectionForeColor = ThemeManager.Instance.TextPrimary;
            dgvChecks.ColumnHeadersDefaultCellStyle.BackColor = UIConstants.PrimaryColor.Default;
            dgvChecks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        public void LoadData()
        {
            try
            {
                _allChecks = _controller.GetAllChecks();
                dgvChecks.DataSource = _allChecks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu kiểm kê: " + ex.Message);
            }
        }

        public void Search(string text)
        {
            if (_allChecks == null) return;
            string keyword = text.ToLower();
            var filtered = _allChecks.FindAll(c => 
                c.CheckID.ToString().Contains(keyword) || 
                (c.Note != null && c.Note.ToLower().Contains(keyword)) ||
                c.Status.ToLower().Contains(keyword));
            dgvChecks.DataSource = filtered;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            InventoryCheckForm form = new InventoryCheckForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void DgvChecks_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OpenCheckDetail(e.RowIndex);
        }

        private void DgvChecks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            
            // View Detail Column (Index 5)
            if (e.ColumnIndex == 5)
            {
                OpenCheckDetail(e.RowIndex);
            }
        }

        private void OpenCheckDetail(int rowIndex)
        {
            var check = dgvChecks.Rows[rowIndex].DataBoundItem as InventoryCheck;
            if (check == null) return;

            InventoryCheck fullCheck = _controller.GetCheckById(check.CheckID);
            InventoryCheckForm form = new InventoryCheckForm(fullCheck);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void DgvChecks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvChecks.Rows.Count) return;

            // Status Column color
            if (dgvChecks.Columns[e.ColumnIndex].DataPropertyName == "Status")
            {
                if (e.Value != null)
                {
                    string status = e.Value.ToString();
                    if (status == "Completed")
                    {
                        e.CellStyle.ForeColor = UIConstants.SemanticColors.Success;
                        e.Value = "Hoàn thành";
                    }
                    else if (status == "Pending")
                    {
                        e.CellStyle.ForeColor = UIConstants.SemanticColors.Warning;
                        e.Value = "Chờ xử lý";
                    }
                }
            }
        }
    }
}
