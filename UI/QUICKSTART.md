# 🚀 Quick Start - Hướng dẫn nhanh

## Xem Components ngay lập tức

1. Chạy ứng dụng
2. Vào menu `⚙️ Cài Đặt`
3. Click `👁️ Xem Components`
4. Khám phá tất cả components!

## Bật Dark Mode

1. Vào menu `⚙️ Cài Đặt`
2. Tích vào `🌙 Chế độ tối (Dark Mode)`
3. Click `💾 Lưu`
4. Toàn bộ ứng dụng sẽ chuyển sang dark mode!

## Sử dụng trong code

### 1. Màu sắc

```csharp
using WarehouseManagement.UI;

// Màu chính
Color primary = UIConstants.PrimaryColor.Default;  // #FF847D
Color hover = UIConstants.PrimaryColor.Hover;

// Màu nền (tự động theo theme)
Color bg = ThemeManager.Instance.BackgroundDefault;
Color text = ThemeManager.Instance.TextPrimary;
```

### 2. Button (5 styles)

```csharp
using WarehouseManagement.UI.Components;

// Style 1: Filled (mặc định)
CustomButton btn1 = new CustomButton
{
    Text = "Lưu",
    ButtonStyleType = ButtonStyle.Filled
};

// Style 2: Outlined
CustomButton btn2 = new CustomButton
{
    Text = "Hủy",
    ButtonStyleType = ButtonStyle.Outlined
};
```

### 3. TextBox với Placeholder

```csharp
CustomTextBox txt = new CustomTextBox
{
    Placeholder = "Nhập tên sản phẩm...",
    Width = 300
};
```

### 4. ComboBox

```csharp
CustomComboBox combo = new CustomComboBox
{
    Width = 250
};
combo.Items.AddRange(new[] { "Option 1", "Option 2" });
```

### 5. TextArea (Multi-line)

```csharp
CustomTextArea area = new CustomTextArea
{
    Width = 400,
    Height = 120,
    Placeholder = "Nhập mô tả..."
};
```

### 6. DateTimePicker

```csharp
CustomDateTimePicker dtp = new CustomDateTimePicker
{
    Width = 250,
    Value = DateTime.Now,
    CustomFormat = "dd/MM/yyyy"
};
dtp.ValueChanged += (s, e) => {
    DateTime selected = dtp.Value;
};
```

### 7. Panel với Border Radius

```csharp
CustomPanel panel = new CustomPanel
{
    BorderRadius = UIConstants.Borders.RadiusLarge,  // 12px
    Width = 500,
    Height = 300
};
```

## Constants thông dụng

```csharp
// Kích thước
int btnHeight = UIConstants.Sizes.ButtonHeight;        // 36px
int inputHeight = UIConstants.Sizes.InputHeight;       // 36px
int rowHeight = UIConstants.Sizes.TableRowHeight;      // 40px

// Khoảng cách
Padding padding = UIConstants.Spacing.Padding.Panel;   // 16px
int spacing = UIConstants.Spacing.Margin.Medium;       // 12px

// Border radius
int radius = UIConstants.Borders.RadiusMedium;         // 8px

// Icons
string saveIcon = UIConstants.Icons.Save;              // "💾"
string addIcon = UIConstants.Icons.Add;                // "➕"
string deleteIcon = UIConstants.Icons.Delete;          // "🗑️"
```

## Apply Theme cho Form

```csharp
public class MyForm : Form
{
    public MyForm()
    {
        InitializeComponent();
        
        // Apply theme cho toàn bộ form
        ThemeManager.Instance.ApplyThemeToForm(this);
        
        // Subscribe để update khi theme thay đổi
        ThemeManager.Instance.ThemeChanged += (s, e) => {
            ThemeManager.Instance.ApplyThemeToForm(this);
        };
    }
}
```

## Ví dụ hoàn chỉnh

```csharp
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

public class ProductForm : Form
{
    private CustomTextBox txtName;
    private CustomComboBox cmbCategory;
    private CustomTextArea txtDescription;
    private CustomButton btnSave;
    private CustomButton btnCancel;
    
    public ProductForm()
    {
        InitializeUI();
        ThemeManager.Instance.ApplyThemeToForm(this);
    }
    
    private void InitializeUI()
    {
        // Container
        CustomPanel container = new CustomPanel
        {
            Dock = DockStyle.Fill,
            BorderRadius = UIConstants.Borders.RadiusLarge
        };
        
        // Name TextBox
        txtName = new CustomTextBox
        {
            Placeholder = "Tên sản phẩm...",
            Width = 300,
            Top = 20,
            Left = 20
        };
        
        // Category ComboBox
        cmbCategory = new CustomComboBox
        {
            Width = 300,
            Top = 70,
            Left = 20
        };
        cmbCategory.Items.AddRange(new[] { "Danh mục 1", "Danh mục 2" });
        
        // Description TextArea
        txtDescription = new CustomTextArea
        {
            Placeholder = "Mô tả sản phẩm...",
            Width = 300,
            Height = 120,
            Top = 120,
            Left = 20
        };
        
        // Save Button
        btnSave = new CustomButton
        {
            Text = $"{UIConstants.Icons.Save} Lưu",
            ButtonStyleType = ButtonStyle.Filled,
            Width = UIConstants.Sizes.ButtonWidthMedium,
            Top = 260,
            Left = 20
        };
        btnSave.Click += (s, e) => {
            // Save logic
            MessageBox.Show("Đã lưu!");
        };
        
        // Cancel Button
        btnCancel = new CustomButton
        {
            Text = "Hủy",
            ButtonStyleType = ButtonStyle.Outlined,
            Width = UIConstants.Sizes.ButtonWidthMedium,
            Top = 260,
            Left = 150
        };
        btnCancel.Click += (s, e) => Close();
        
        // Add to container
        container.Controls.AddRange(new Control[] {
            txtName, cmbCategory, txtDescription, btnSave, btnCancel
        });
        
        Controls.Add(container);
        
        // Form settings
        Width = 380;
        Height = 350;
        Text = "Thêm sản phẩm";
        StartPosition = FormStartPosition.CenterParent;
    }
}
```

---

**Xem chi tiết hơn trong [README.md](./README.md)**
