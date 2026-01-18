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
    public class ProductsPanel : Panel, ISearchable
    {
        private DataGridView dgvProducts;
        private ProductController _productController;
        private List<Product> allProducts;

        public ProductsPanel()
        {
            _productController = new ProductController();
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

            // DataGridView
            dgvProducts = new DataGridView
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

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "ProductID", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Product} Tên Sản Phẩm", 
                DataPropertyName = "ProductName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Category} Danh Mục", 
                DataPropertyName = "CategoryID", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Money} Giá", 
                DataPropertyName = "Price", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0", 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Package} Tồn Kho", 
                DataPropertyName = "Quantity", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Warning} Ngưỡng Min", 
                DataPropertyName = "MinThreshold", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = $"{UIConstants.Icons.Chart} Tổng Giá Trị", 
                DataPropertyName = "InventoryValue", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0", 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                } 
            });
            
            dgvProducts.Columns.Add(new DataGridViewButtonColumn 
            { 
                HeaderText = UIConstants.Icons.Eye, 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForButtonValue = true, 
                Text = UIConstants.Icons.Eye 
            });
            
            dgvProducts.Columns.Add(new DataGridViewButtonColumn 
            { 
                HeaderText = UIConstants.Icons.Delete, 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForButtonValue = true, 
                Text = UIConstants.Icons.Delete 
            });

            dgvProducts.CellFormatting += DgvProducts_CellFormatting;
            dgvProducts.CellClick += DgvProducts_CellClick;
            dgvProducts.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                    LoadData();
            };

            Controls.Add(dgvProducts);
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvProducts.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvProducts.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvProducts.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundLight;
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
        }

        public void LoadData()
        {
            try
            {
                allProducts = _productController.GetAllProducts(SettingsForm.ShowHiddenItems);
                dgvProducts.DataSource = allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Search(string searchText)
        {
            try
            {
                string search = searchText.ToLower();
                List<Product> filtered = allProducts.FindAll(p => p.ProductName.ToLower().Contains(search));
                dgvProducts.DataSource = filtered;
            }
            catch { }
        }

        private void DgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProducts.Rows[e.RowIndex].DataBoundItem is Product product)
            {
                if (product.IsLowStock)
                {
                    e.CellStyle.BackColor = UIConstants.SemanticColors.Error;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.Font = ThemeManager.Instance.FontBold;
                }
                else
                {
                    e.CellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
                    e.CellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
                }
            }
        }

        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Column 7 is the hide button
            if (e.ColumnIndex == 7)
            {
                int productId = (int)dgvProducts.Rows[e.RowIndex].Cells[0].Value;
                string productName = dgvProducts.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
                
                DialogResult result = MessageBox.Show(
                    $"{UIConstants.Icons.Question} Bạn chắc chắn muốn đảo trạng thái sản phẩm '{productName}'?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _productController.HideProduct(productId);
                        MessageBox.Show($"{UIConstants.Icons.Success} Trạng thái sản phẩm đã được thay đổi.", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{UIConstants.Icons.Error} Lỗi ẩn sản phẩm: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            // Column 8 is the delete button
            if (e.ColumnIndex == 8)
            {
                int productId = (int)dgvProducts.Rows[e.RowIndex].Cells[0].Value;
                string productName = dgvProducts.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
                
                DialogResult result = MessageBox.Show(
                    $"{UIConstants.Icons.Warning} Bạn chắc chắn muốn xóa sản phẩm '{productName}'?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _productController.DeleteProduct(productId);
                        MessageBox.Show($"{UIConstants.Icons.Success} Sản phẩm đã được xóa thành công.", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{UIConstants.Icons.Error} Lỗi xóa sản phẩm: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            // Other columns open the edit form
            int id = (int)dgvProducts.Rows[e.RowIndex].Cells[0].Value;
            ProductForm form = new ProductForm(id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
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
