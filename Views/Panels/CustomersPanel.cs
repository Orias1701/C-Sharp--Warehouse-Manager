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
    public class CustomersPanel : Panel, ISearchable
    {
        private DataGridView dgvCustomers;
        private CustomerController _controller;
        private List<Customer> _allCustomers;

        public CustomersPanel()
        {
            _controller = new CustomerController();
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
                Text = $"{UIConstants.Icons.Add} Thêm Khách Hàng",
                Width = 200,
                Height = 35,
                Top = 5,
                Left = 0,
                ButtonStyleType = ButtonStyle.Filled
            };
            btnAdd.Click += BtnAdd_Click;
            topPanel.Controls.Add(btnAdd);
            Controls.Add(topPanel);

            // DataGridView
            dgvCustomers = new DataGridView
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
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "CustomerID", 
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(10, 5, 10, 5) }
            });
            
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tên Khách Hàng", 
                DataPropertyName = "CustomerName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 5, 10, 5) }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Số Điện Thoại", 
                DataPropertyName = "Phone", 
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 5, 10, 5) }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Email", 
                DataPropertyName = "Email", 
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 5, 10, 5) }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Địa Chỉ", 
                DataPropertyName = "Address", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(10, 5, 10, 5) }
            });

            // Action Columns
            dgvCustomers.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Edit,
                LinkColor = ThemeManager.Instance.PrimaryDefault,
                ActiveLinkColor = ThemeManager.Instance.PrimaryDefault,
                VisitedLinkColor = ThemeManager.Instance.PrimaryDefault,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            
            dgvCustomers.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Delete,
                LinkColor = UIConstants.SemanticColors.Error,
                ActiveLinkColor = UIConstants.SemanticColors.Error,
                VisitedLinkColor = UIConstants.SemanticColors.Error,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Header Styling Sync
            foreach (DataGridViewColumn col in dgvCustomers.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                col.HeaderCell.Style.Padding = new Padding(10, 5, 10, 5);
            }

            dgvCustomers.CellClick += DgvCustomers_CellClick;

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
            tablePanel.Controls.Add(dgvCustomers);
            Controls.Add(tablePanel);
            
            topPanel.SendToBack();

            LoadData();
        }

        private void OnThemeChanged(object sender, EventArgs e) => ApplyTheme();

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvCustomers.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvCustomers.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvCustomers.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvCustomers.DefaultCellStyle.SelectionBackColor = UIConstants.PrimaryColor.Light;
            dgvCustomers.DefaultCellStyle.SelectionForeColor = ThemeManager.Instance.TextPrimary;
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = UIConstants.PrimaryColor.Default;
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        public void LoadData()
        {
            try
            {
                _allCustomers = _controller.GetAllCustomers();
                dgvCustomers.DataSource = _allCustomers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        public void Search(string text)
        {
            if (_allCustomers == null) return;
            string keyword = text.ToLower();
            var filtered = _allCustomers.FindAll(c => 
                c.CustomerName.ToLower().Contains(keyword) || 
                (c.Phone != null && c.Phone.Contains(keyword)) || 
                (c.Email != null && c.Email.ToLower().Contains(keyword)));
            dgvCustomers.DataSource = filtered;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            CustomerForm form = new CustomerForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void DgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Edit
            if (e.ColumnIndex == 5)
            {
                var customer = dgvCustomers.Rows[e.RowIndex].DataBoundItem as Customer;
                if (customer != null)
                {
                    CustomerForm form = new CustomerForm(customer);
                    if (form.ShowDialog() == DialogResult.OK) LoadData();
                }
            }
            // Delete
            else if (e.ColumnIndex == 6)
            {
                var customer = dgvCustomers.Rows[e.RowIndex].DataBoundItem as Customer;
                if (customer != null)
                {
                    if (MessageBox.Show($"Bạn có chắc chắn muốn xóa khách hàng '{customer.CustomerName}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        try
                        {
                            if (_controller.DeleteCustomer(customer.CustomerID))
                            {
                                MessageBox.Show("Đã xóa thành công!");
                                LoadData();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi xóa: " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
