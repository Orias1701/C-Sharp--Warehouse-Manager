using System;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views
{
    /// <summary>
    /// Form Thêm/Sửa danh mục sản phẩm - Đồng bộ với ProductForm
    /// </summary>
    public partial class CategoryForm : Form
    {
        private ProductController _productController;
        private int? _categoryId = null;
        private TextBox txtCategoryName;
        private Button btnSave, btnCancel;

        public CategoryForm(int? categoryId = null)
        {
            _categoryId = categoryId;
            _productController = new ProductController();
            InitializeComponent();
            Text = _categoryId.HasValue ? "Sửa danh mục" : "Thêm danh mục";
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Layout standard đồng bộ với ProductForm
            const int LABEL_WIDTH = 100;
            const int INPUT_WIDTH = 300;
            const int LABEL_LEFT = 20;
            const int INPUT_LEFT = 130;
            const int ITEM_SPACING = 35;
            const int BUTTON_WIDTH = 100;
            const int BUTTON_HEIGHT = 35;

            // Label và Input
            Label lblCategoryName = new Label { 
                Text = "Tên danh mục:", 
                Left = LABEL_LEFT, 
                Top = 30, 
                Width = LABEL_WIDTH, 
                AutoSize = false, 
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft 
            };
            
            txtCategoryName = new TextBox { 
                Left = INPUT_LEFT, 
                Top = 30, 
                Width = INPUT_WIDTH, 
                Height = 25 
            };

            // Nút điều khiển bố trí ở phía dưới
            btnSave = new Button { 
                Text = "💾 Lưu", 
                Left = INPUT_LEFT, 
                Top = 30 + ITEM_SPACING + 20, 
                Width = BUTTON_WIDTH, 
                Height = BUTTON_HEIGHT 
            };
            
            btnCancel = new Button { 
                Text = "❌ Hủy", 
                Left = INPUT_LEFT + BUTTON_WIDTH + 15, 
                Top = 30 + ITEM_SPACING + 20, 
                Width = BUTTON_WIDTH, 
                Height = BUTTON_HEIGHT, 
                DialogResult = DialogResult.Cancel 
            };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => Close();

            // Cấu hình Form
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
                LoadCategoryData();
            }
        }

        private void LoadCategoryData()
        {
            try
            {
                // Giả định controller có hàm lấy category theo ID
                var categories = _productController.GetAllCategories();
                var cat = categories.Find(c => c.CategoryID == _categoryId.Value);
                if (cat != null)
                {
                    txtCategoryName.Text = cat.CategoryName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Frontend validation
            string catName = txtCategoryName.Text.Trim();
            if (string.IsNullOrWhiteSpace(catName))
            {
                MessageBox.Show("❌ Vui lòng nhập tên danh mục");
                txtCategoryName.Focus();
                return;
            }

            if (catName.Length > 100)
            {
                MessageBox.Show("❌ Tên danh mục không quá 100 ký tự");
                txtCategoryName.Focus();
                return;
            }

            try
            {
                if (_categoryId.HasValue)
                {
                    _productController.UpdateCategory(new Category 
                    { 
                        CategoryID = _categoryId.Value, 
                        CategoryName = catName 
                    });
                    MessageBox.Show("✅ Cập nhật danh mục thành công!");
                }
                else
                {
                    _productController.AddCategory(new Category { CategoryName = catName });
                    MessageBox.Show("✅ Thêm danh mục thành công!");
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi: " + ex.Message);
            }
        }
    }
}