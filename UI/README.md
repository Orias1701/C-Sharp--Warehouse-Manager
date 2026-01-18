# UI Components System - Hệ Thống Giao Diện

Hệ thống giao diện hiện đại và có thể tùy chỉnh cho ứng dụng Warehouse Management.

## 📁 Cấu trúc thư mục

```
UI/
├── UIConstants.cs          - Định nghĩa constants (colors, fonts, sizes, spacing, icons)
├── ThemeManager.cs         - Quản lý Dark/Light theme
├── Components/
│   ├── CustomPanel.cs      - Panel với border radius
│   ├── CustomButton.cs     - Button với 5 styles khác nhau
│   ├── CustomTextBox.cs    - TextBox với border radius & placeholder
│   ├── CustomComboBox.cs   - ComboBox với border radius
│   ├── CustomTextArea.cs   - TextArea (multi-line) với border radius
│   └── ComponentsTestPanel.cs - Panel test/preview components
└── README.md              - File này
```

## 🎨 UIConstants - Định nghĩa Constants

### 1. Colors (Màu sắc)

#### Primary Color - FF847D
- `Default` - Màu chính (#FF847D)
- `Active` - Khi active (#FF6B62)
- `Hover` - Khi hover (#FF9D97)
- `Pressed` - Khi nhấn (#E6776F)
- `Disabled` - Khi disabled (#FFBFBA)
- `Light` - Sắc sáng (#FFD1CE)
- `Dark` - Sắc tối (#CC6964)

#### Background Colors
**Light Theme:**
- `Default`, `Lighter`, `Light`, `Medium`, `Dark`, `Darker`

**Dark Theme:**
- `Default`, `Lighter`, `Light`, `Medium`, `Dark`, `Darker`

#### Text Colors
**Light/Dark Theme:**
- `Primary`, `Secondary`, `Disabled`, `Hint`

#### Semantic Colors
- `Success` (#4CAF50)
- `Warning` (#FF9800)
- `Error` (#F44336)
- `Info` (#2196F3)

### 2. Fonts (Font chữ)

**Font Family:** Segoe UI

**8 cấp độ kích thước:**
- `XXSmall` - 9px (chú thích phụ)
- `XSmall` - 10px (label phụ)
- `Small` - 11px (label thường)
- `Regular` - 12px (text thường) ⭐ Default
- `Medium` - 14px (subheading)
- `Large` - 16px (heading)
- `XLarge` - 20px (title)
- `XXLarge` - 24px (main title)

### 3. Sizes (Kích thước)

**Button:**
- Height: 36px
- Width: Small (80px), Medium (120px), Large (160px)

**Input:**
- Height: 36px
- Small: 28px, Large: 44px

**Table:**
- Row Height: 40px
- Header Height: 44px

**Icons:**
- Small: 16px, Medium: 20px, Large: 24px

### 4. Spacing (Khoảng cách)

**Padding (vùng an toàn trong):**
- XXSmall: 2px
- XSmall: 4px
- Small: 8px
- Medium: 12px ⭐ Default
- Large: 16px
- XLarge: 20px
- XXLarge: 24px

**Margin (vùng an toàn ngoài):**
Tương tự Padding

### 5. Borders (Viền)

**Border Radius:**
- None: 0px
- Small: 4px
- Medium: 8px ⭐ Default
- Large: 12px
- XLarge: 16px
- Full: 999px (tròn hoàn toàn)

**Border Thickness:**
- Default: 1px ⭐
- Medium: 2px
- Thick: 3px

### 6. Icons (Biểu tượng)

Định nghĩa sẵn 30+ icons thông dụng:
- Navigation: Home, Menu, Back, Forward, Close
- Actions: Add, Edit, Delete, Save, Cancel, Refresh, Search
- Status: Success, Error, Warning, Info
- Data: Import, Export, Upload, Download
- Views: List, Grid, Chart, Table
- Other: Settings, User, Category, Product, Sun, Moon...

**Sử dụng:**
```csharp
string icon = UIConstants.Icons.Save; // "💾"
button.Text = $"{icon} Lưu";
```

## 🎭 ThemeManager - Quản lý Theme

### Singleton Pattern
```csharp
ThemeManager theme = ThemeManager.Instance;
```

### Properties
```csharp
bool isDark = theme.IsDarkMode;       // Get/Set dark mode
theme.ToggleTheme();                  // Chuyển đổi theme
```

### Event
```csharp
ThemeManager.Instance.ThemeChanged += (s, e) => {
    // Xử lý khi theme thay đổi
};
```

### Lấy màu theo theme hiện tại
```csharp
Color bg = theme.BackgroundDefault;
Color text = theme.TextPrimary;
Color primary = theme.PrimaryDefault;
```

### Apply theme cho control
```csharp
theme.ApplyTheme(myPanel);           // Apply cho 1 control
theme.ApplyThemeToForm(myForm);      // Apply cho cả Form
```

### Lấy font
```csharp
Font regular = theme.FontRegular;
Font bold = theme.FontBold;
Font large = theme.FontLarge;
```

## 🧩 Components - Các thành phần UI

### 1. CustomPanel

Panel với border radius tùy chỉnh.

**Properties:**
```csharp
CustomPanel panel = new CustomPanel
{
    BorderRadius = UIConstants.Borders.RadiusMedium,  // 8px
    BorderColor = ThemeManager.Instance.PrimaryDefault,
    BorderThickness = UIConstants.Borders.BorderThickness,
    ShowBorder = true
};
```

**Tự động:**
- Apply theme khi ThemeChanged
- Padding/Margin mặc định
- Border radius mượt mà

### 2. CustomButton

Button với 5 styles khác nhau.

**5 Button Styles:**

```csharp
CustomButton btn = new CustomButton
{
    Text = "Click me",
    ButtonStyleType = ButtonStyle.Filled, // Chọn 1 trong 5 style
    BorderRadius = UIConstants.Borders.RadiusMedium
};
```

**5 Styles:**
1. `Outlined` - Nền BG, viền PrimaryColor
2. `Filled` - Nền PrimaryColor, viền BG ⭐ Default
3. `Text` - Nền BG, viền Transparent
4. `FilledNoOutline` - Nền PrimaryColor, viền Transparent
5. `Ghost` - Nền và viền Transparent

**Tự động:**
- Hover effect
- Pressed effect
- Disabled state
- Apply theme

### 3. CustomTextBox

TextBox với border radius và placeholder.

```csharp
CustomTextBox txt = new CustomTextBox
{
    Width = 250,
    Placeholder = "Nhập văn bản...",
    BorderRadius = UIConstants.Borders.RadiusMedium,
    IsPassword = false,
    MaxLength = 100
};
```

**Features:**
- Placeholder text tự động ẩn/hiện
- Focus state (border đổi màu)
- Password mode
- Apply theme

### 4. CustomComboBox

ComboBox với border radius và custom dropdown button.

```csharp
CustomComboBox combo = new CustomComboBox
{
    Width = 250,
    BorderRadius = UIConstants.Borders.RadiusMedium,
    ButtonColor = ThemeManager.Instance.PrimaryDefault
};
combo.Items.AddRange(new[] { "Option 1", "Option 2", "Option 3" });
combo.SelectedIndex = 0;
```

**Features:**
- Dropdown button tùy chỉnh màu
- Focus state
- Apply theme

### 5. CustomTextArea

Multi-line TextBox với border radius.

```csharp
CustomTextArea area = new CustomTextArea
{
    Width = 400,
    Height = 120,
    Placeholder = "Nhập văn bản dài...",
    ScrollBars = ScrollBars.Vertical,
    WordWrap = true
};
```

**Features:**
- Multi-line support
- Scrollbar
- Word wrap
- Placeholder
- Apply theme

## 🧪 ComponentsTestPanel

Panel để xem trước tất cả components.

**Cách sử dụng:**
1. Vào `Settings` (⚙️ Cài Đặt)
2. Click `👁️ Xem Components`
3. Xem preview của tất cả components

**Hiển thị:**
- ✅ Tất cả colors (Primary, Background, Text, Semantic)
- ✅ Tất cả 8 font sizes
- ✅ 5 button styles (Normal + Disabled state)
- ✅ TextBox & ComboBox
- ✅ TextArea
- ✅ Panels với các border radius khác nhau
- ✅ Spacing visualization

## 🌓 Dark Mode

### Bật/Tắt Dark Mode

**Cách 1: Qua Settings**
1. Vào `Settings` (⚙️ Cài Đặt)
2. Toggle `🌙 Chế độ tối (Dark Mode)`
3. Lưu settings

**Cách 2: Qua code**
```csharp
ThemeManager.Instance.IsDarkMode = true;  // Bật dark mode
ThemeManager.Instance.ToggleTheme();      // Toggle
```

### Tự động apply theme

Tất cả custom components tự động cập nhật khi theme thay đổi:
- CustomPanel
- CustomButton
- CustomTextBox
- CustomComboBox
- CustomTextArea

## 📝 Best Practices

### 1. Sử dụng Constants
```csharp
// ✅ ĐÚNG
button.Height = UIConstants.Sizes.ButtonHeight;
panel.Padding = UIConstants.Spacing.Padding.Panel;

// ❌ SAI
button.Height = 36;
panel.Padding = new Padding(16);
```

### 2. Sử dụng ThemeManager cho màu
```csharp
// ✅ ĐÚNG
panel.BackColor = ThemeManager.Instance.BackgroundDefault;

// ❌ SAI
panel.BackColor = Color.White; // Không đổi theo theme
```

### 3. Subscribe ThemeChanged event
```csharp
// ✅ ĐÚNG - Khi tạo custom control
public MyControl()
{
    ThemeManager.Instance.ThemeChanged += OnThemeChanged;
}

private void OnThemeChanged(object sender, EventArgs e)
{
    ApplyTheme();
}

protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        ThemeManager.Instance.ThemeChanged -= OnThemeChanged;
    }
    base.Dispose(disposing);
}
```

### 4. Sử dụng Custom Components
```csharp
// ✅ ĐÚNG
CustomButton btn = new CustomButton
{
    Text = "Click me",
    ButtonStyleType = ButtonStyle.Filled
};

// ❌ TỐT HƠN NÊN DÙNG CUSTOM
Button btn = new Button
{
    Text = "Click me"
};
```

## 🚀 Ví dụ tích hợp

```csharp
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

public class MyForm : Form
{
    public MyForm()
    {
        // Apply theme cho form
        ThemeManager.Instance.ApplyThemeToForm(this);
        
        // Tạo panel container
        CustomPanel container = new CustomPanel
        {
            Dock = DockStyle.Fill,
            BorderRadius = UIConstants.Borders.RadiusLarge
        };
        
        // Tạo button
        CustomButton saveBtn = new CustomButton
        {
            Text = $"{UIConstants.Icons.Save} Lưu",
            ButtonStyleType = ButtonStyle.Filled,
            Width = UIConstants.Sizes.ButtonWidthMedium
        };
        saveBtn.Click += SaveBtn_Click;
        
        // Tạo textbox
        CustomTextBox nameBox = new CustomTextBox
        {
            Placeholder = "Nhập tên...",
            Width = 300
        };
        
        container.Controls.Add(saveBtn);
        container.Controls.Add(nameBox);
        Controls.Add(container);
    }
    
    private void SaveBtn_Click(object sender, EventArgs e)
    {
        // Handle save
    }
}
```

## 🎯 Summary

Hệ thống UI Components cung cấp:
- ✅ Theme system (Dark/Light mode)
- ✅ Consistent colors, fonts, sizes, spacing
- ✅ 5 button styles
- ✅ Custom controls với border radius
- ✅ Placeholder support
- ✅ Auto theme switching
- ✅ Test panel để preview
- ✅ Easy to use & maintain

**Enjoy coding! 🎨✨**
