using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views
{
    /// <summary>
    /// Form Thêm/Sửa danh mục sản phẩm
    /// </summary>
    public partial class CategoryForm : Form
    {
        private CategoryController _categoryController;
        private int? _categoryId = null;
        private TextBox txtCategoryName;
        private Button btnSave, btnCancel;

        public CategoryForm(int? categoryId = null)
        {
            _categoryId = categoryId;
            _categoryController = new CategoryController();
            InitializeComponent();
            Text = categoryId.HasValue ? "Sửa danh mục" : "Thêm danh mục";
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Layout standard: Label 100px, Input 300px, spacing 35px
            const int LABEL_WIDTH = 100;
            const int INPUT_WIDTH = 300;
            const int LABEL_LEFT = 20;
            const int INPUT_LEFT = 130;
            const int ITEM_SPACING = 35;
            const int BUTTON_WIDTH = 100;
            const int BUTTON_HEIGHT = 35;

            // Labels và TextBoxes
            Label lblCategoryName = new Label { Text = "Tên danh mục:", Left = LABEL_LEFT, Top = 20, Width = LABEL_WIDTH, AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            txtCategoryName = new TextBox { Left = INPUT_LEFT, Top = 20, Width = INPUT_WIDTH, Height = 25 };

            btnSave = new Button { Text = "💾 Lưu", Left = INPUT_LEFT, Top = 20 + ITEM_SPACING + 10, Width = BUTTON_WIDTH, Height = BUTTON_HEIGHT };
            btnCancel = new Button { Text = "❌ Hủy", Left = INPUT_LEFT + BUTTON_WIDTH + 15, Top = 20 + ITEM_SPACING + 10, Width = BUTTON_WIDTH, Height = BUTTON_HEIGHT, DialogResult = DialogResult.Cancel };

            btnSave.Click += BtnSave_Click;

            Controls.Add(lblCategoryName);
            Controls.Add(txtCategoryName);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            Width = 480;
            Height = 180;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            CancelButton = btnCancel;
            Padding = new Padding(10);

            Load += CategoryForm_Load;
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Nút Lưu
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Frontend validation
            string categoryName = txtCategoryName.Text.Trim();
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("❌ Vui lòng nhập tên danh mục");
                txtCategoryName.Focus();
                return;
            }

            if (categoryName.Length > 100)
            {
                MessageBox.Show("❌ Tên danh mục không được vượt quá 100 ký tự");
                txtCategoryName.Focus();
                return;
            }

            try
            {
                if (_categoryId.HasValue)
                {
                    _categoryController.UpdateCategory(_categoryId.Value, categoryName);
                    MessageBox.Show("Cập nhật danh mục thành công!");
                }
                else
                {
                    _categoryController.CreateCategory(categoryName);
                    MessageBox.Show("Thêm danh mục thành công!");
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Nút Hủy
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}