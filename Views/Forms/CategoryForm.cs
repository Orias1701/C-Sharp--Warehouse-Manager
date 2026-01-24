using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Forms
{
    /// <summary>
    /// Form Thêm/Sửa danh mục sản phẩm
    /// </summary>
    public partial class CategoryForm : Form
    {
        private CategoryController _categoryController;
        private int? _categoryId = null;
        private CustomTextBox txtCategoryName;
        private CustomTextArea txtCategoryDesc;
        private CustomButton btnSave, btnCancel;

        public CategoryForm(int? categoryId = null)
        {
            _categoryId = categoryId;
            _categoryController = new CategoryController();
            InitializeComponent();
            Text = categoryId.HasValue 
                ? $"{UIConstants.Icons.Edit} Sửa danh mục" 
                : $"{UIConstants.Icons.Add} Thêm danh mục";
            
            // Apply theme
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main container
            CustomPanel mainPanel = new CustomPanel
            {
                Dock = DockStyle.Fill,
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = false,
                Padding = new Padding(0)  // Không dùng padding của panel
            };

            // Layout constants
            const int INPUT_WIDTH = 400;
            const int LEFT_MARGIN = 40;  // Margin từ bên trái
            int currentY = 30;
            int inputSpacing = 20;

            // Category Name Label
            Label lblCategoryName = new Label 
            { 
                Text = "Tên danh mục",
                Left = LEFT_MARGIN, 
                Top = currentY, 
                AutoSize = true,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B),
                TabStop = false
            };
            currentY += 20;

            txtCategoryName = new CustomTextBox 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = INPUT_WIDTH,
                Placeholder = "Nhập tên danh mục...",
                TabIndex = 0
            };
            currentY += UIConstants.Sizes.InputHeight + inputSpacing;

            // Category Description Label
            Label lblCategoryDesc = new Label 
            { 
                Text = "Mô tả",
                Left = LEFT_MARGIN, 
                Top = currentY, 
                AutoSize = true,
                Font = ThemeManager.Instance.FontSmall,
                ForeColor = Color.FromArgb(180, UIConstants.PrimaryColor.Default.R, 
                                          UIConstants.PrimaryColor.Default.G, 
                                          UIConstants.PrimaryColor.Default.B),
                TabStop = false
            };
            currentY += 20;

            txtCategoryDesc = new CustomTextArea 
            { 
                Left = LEFT_MARGIN, 
                Top = currentY, 
                Width = INPUT_WIDTH, 
                Height = 90,
                Placeholder = "Nhập mô tả (không bắt buộc)...",
                TabIndex = 1
            };
            currentY += 90 + 30;

            // Buttons - Centered
            int totalButtonWidth = 120 + 10 + 120; // Lưu + spacing + Hủy
            int buttonStartX = LEFT_MARGIN + (INPUT_WIDTH - totalButtonWidth) / 2;

            btnSave = new CustomButton 
            { 
                Text = "Lưu",
                Left = buttonStartX, 
                Top = currentY, 
                Width = 120,
                ButtonStyleType = ButtonStyle.FilledNoOutline,
                TabIndex = 2
            };

            btnCancel = new CustomButton 
            { 
                Text = "Hủy",
                Left = buttonStartX + 120 + 10, 
                Top = currentY, 
                Width = 120,
                ButtonStyleType = ButtonStyle.Outlined,
                CausesValidation = false,
                TabIndex = 3
            };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            mainPanel.Controls.Add(lblCategoryName);
            mainPanel.Controls.Add(txtCategoryName);
            mainPanel.Controls.Add(lblCategoryDesc);
            mainPanel.Controls.Add(txtCategoryDesc);
            mainPanel.Controls.Add(btnSave);
            mainPanel.Controls.Add(btnCancel);

            Controls.Add(mainPanel);

            ClientSize = new Size(480, 330);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ThemeManager.Instance.BackgroundLight;
            AcceptButton = btnSave;
            CancelButton = btnCancel;

            Load += CategoryForm_Load;
            
            // Focus Save Button
            this.Load += (s, e) => {
                if (btnSave != null) ActiveControl = btnSave;
            };
            
            ResumeLayout(false);
        }

        private void CategoryForm_Load(object sender, EventArgs e)
        {
            if (_categoryId.HasValue)
            {
                LoadCategory();
            }
        }

        private void LoadCategory()
        {
            try
            {
                Category category = _categoryController.GetCategoryById(_categoryId.Value);
                if (category != null)
                {
                    txtCategoryName.Text = category.CategoryName;
                    txtCategoryDesc.Text = category.Description ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Frontend validation
            string categoryName = txtCategoryName.Text.Trim();
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Vui lòng nhập tên danh mục", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            if (categoryName.Length > 100)
            {
                MessageBox.Show($"{UIConstants.Icons.Warning} Tên danh mục không được vượt quá 100 ký tự", "Cảnh báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            try
            {
                if (_categoryId.HasValue)
                {
                    _categoryController.UpdateCategory(_categoryId.Value, categoryName, txtCategoryDesc.Text);
                    MessageBox.Show($"{UIConstants.Icons.Success} Cập nhật danh mục thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _categoryController.CreateCategory(categoryName, txtCategoryDesc.Text);
                    MessageBox.Show($"{UIConstants.Icons.Success} Thêm danh mục thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
