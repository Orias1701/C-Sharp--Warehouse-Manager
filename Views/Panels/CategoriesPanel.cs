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
    public class CategoriesPanel : Panel, ISearchable
    {
        private DataGridView dgvCategories;
        private CategoryController _categoryController;
        private List<Category> allCategories;

        public CategoriesPanel()
        {
            _categoryController = new CategoryController();
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
            dgvCategories = new DataGridView
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

            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "ID", 
                DataPropertyName = "CategoryID", 
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            
            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Tên Danh Mục", 
                DataPropertyName = "CategoryName", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Mô Tả", 
                DataPropertyName = "Description", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            dgvCategories.Columns.Add(new DataGridViewLinkColumn 
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
            
            dgvCategories.Columns.Add(new DataGridViewLinkColumn 
            { 
                HeaderText = "", 
                Width = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                UseColumnTextForLinkValue = true, 
                Text = UIConstants.Icons.Cancel,
                LinkBehavior = LinkBehavior.NeverUnderline,
                LinkColor = ThemeManager.Instance.TextPrimary,
                ActiveLinkColor = UIConstants.SemanticColors.Error,
                VisitedLinkColor = ThemeManager.Instance.TextPrimary,
                TrackVisitedState = false,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvCategories.CellClick += DgvCategories_CellClick;
            dgvCategories.VisibleChanged += (s, e) =>
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
            tablePanel.Controls.Add(dgvCategories);
            Controls.Add(tablePanel);
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvCategories.BackgroundColor = ThemeManager.Instance.BackgroundDefault;
            dgvCategories.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvCategories.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvCategories.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundLight;
            dgvCategories.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
        }

        public void LoadData()
        {
            try
            {
                // Load all categories including hidden ones if setting is enabled
                allCategories = _categoryController.GetAllCategories(SettingsForm.ShowHiddenItems);
                dgvCategories.DataSource = allCategories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải danh mục: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Search(string searchText)
        {
            try
            {
                string search = searchText.ToLower();
                List<Category> filtered = allCategories.FindAll(c => c.CategoryName.ToLower().Contains(search));
                dgvCategories.DataSource = filtered;
            }
            catch { }
        }

        private void DgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int categoryId = (int)dgvCategories.Rows[e.RowIndex].Cells[0].Value;
            string categoryName = dgvCategories.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";

            // Column 3 is the hide button
            if (e.ColumnIndex == 3)
            {
                DialogResult result = MessageBox.Show(
                    $"{UIConstants.Icons.Question} Bạn chắc chắn muốn đảo trạng thái danh mục '{categoryName}'?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _categoryController.HideCategory(categoryId);
                        MessageBox.Show($"{UIConstants.Icons.Success} Trạng thái danh mục đã được thay đổi.", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{UIConstants.Icons.Error} Lỗi ẩn danh mục: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            // Column 4 is the delete button
            if (e.ColumnIndex == 4)
            {
                DialogResult result = MessageBox.Show(
                    $"{UIConstants.Icons.Warning} Bạn chắc chắn muốn xóa danh mục '{categoryName}'?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _categoryController.DeleteCategory(categoryId);
                        MessageBox.Show($"{UIConstants.Icons.Success} Danh mục đã được xóa thành công.", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{UIConstants.Icons.Error} Lỗi xóa danh mục: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            // Other columns open the edit form
            CategoryForm form = new CategoryForm(categoryId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
            dgvCategories.Rows[e.RowIndex].Selected = true;
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
