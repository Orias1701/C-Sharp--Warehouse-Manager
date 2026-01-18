# 🔧 Border Radius Fixes - v2.0.2

## 📅 Date: 2026-01-18

---

## 🐛 Vấn đề phát hiện

### 1. CustomButton - Viền BG không hiển thị ở góc

**Mô tả:**
- Button style "Filled" (Nền Primary, viền BG) có vấn đề ở các góc
- Viền không hiển thị hoàn chỉnh ở 4 góc bo tròn
- Border bị mất hoặc không mượt

**Nguyên nhân:**
- `g.Clear(Parent?.BackColor)` clear toàn bộ canvas
- Sau đó vẽ background với border radius
- Nhưng khi vẽ border, rectangle bị shrink (`borderRect.Width -= 1`) làm border không khớp với background path
- Clear operation có thể làm mất anti-aliasing ở góc

---

### 2. CustomDateTimePicker - Border mặc định lộ ra

**Mô tả:**
- DateTimePicker bên trong vẫn hiển thị border mặc định của Windows Forms
- Phần bao bọc có border radius, nhưng DateTimePicker bên trong vẫn vuông
- Tạo hiệu ứng "box trong box"

**Nguyên nhân:**
- DateTimePicker không có property BorderStyle.None
- Luôn có border mặc định của Windows Forms
- Border này không bị ẩn bởi custom rendering

---

### 3. CustomComboBox - Tương tự DateTimePicker

**Mô tả:**
- ComboBox bên trong hiển thị border mặc định
- Không khớp với border radius bên ngoài
- Visual inconsistency

**Nguyên nhân:**
- ComboBox với FlatStyle.Flat vẫn có border
- UserPaint vẽ lên trên nhưng không che được border bên dưới

---

## ✅ Giải pháp

### Fix 1: CustomButton - Chỉ clear khi cần thiết

**Before:**
```csharp
protected override void OnPaint(PaintEventArgs pevent)
{
    Graphics g = pevent.Graphics;
    
    // Clear toàn bộ - GÂY LỖI
    g.Clear(Parent?.BackColor ?? SystemColors.Control);
    
    // Vẽ background và border...
}
```

**After:**
```csharp
protected override void OnPaint(PaintEventArgs pevent)
{
    Graphics g = pevent.Graphics;
    
    // CHỈ clear khi style Ghost hoặc Transparent
    if (_buttonStyle == ButtonStyle.Ghost || backColor == Color.Transparent)
    {
        g.Clear(Parent?.BackColor ?? SystemColors.Control);
    }
    
    // Vẽ border với PenAlignment.Inset (vẽ BÊN TRONG path)
    using (Pen pen = new Pen(borderColor, thickness))
    {
        pen.Alignment = PenAlignment.Inset;  // ← KEY FIX
        g.DrawPath(pen, path);
    }
}
```

**Key changes:**
1. ✅ Không clear background cho mọi style
2. ✅ Sử dụng `PenAlignment.Inset` để vẽ border bên trong path
3. ✅ Không shrink rectangle trước khi vẽ border
4. ✅ Border khớp hoàn hảo với background path

---

### Fix 2: CustomDateTimePicker - Clip region

**Approach:**
- Sử dụng `Region` property để clip toàn bộ control
- DateTimePicker được đặt lớn hơn container một chút
- Region clip phần border thừa
- Chỉ hiển thị phần trong với border radius

**Code:**
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    using (GraphicsPath path = GetRoundedRectanglePath(ClientRectangle, _borderRadius))
    {
        // Set region để clip DateTimePicker
        Region = new Region(path);
        
        // Clear background
        g.Clear(Parent?.BackColor ?? SystemColors.Control);
        
        // Vẽ background và border
        g.FillPath(backgroundBrush, path);
        g.DrawPath(borderPen, path);
    }
}

private void UpdateDateTimePickerSize()
{
    // Đặt DateTimePicker LỚN HƠN một chút (-2, +4)
    // Để border của nó bị clip bởi Region
    _dateTimePicker.Location = new Point(-2, yPosition - 1);
    _dateTimePicker.Size = new Size(Width + 4, dtpHeight);
}
```

**Result:**
- ✅ DateTimePicker border bị clip
- ✅ Chỉ hiển thị custom border với radius
- ✅ Smooth corners

---

### Fix 3: CustomComboBox - Clip region tương tự

**Approach:** Giống CustomDateTimePicker

**Code:**
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    using (GraphicsPath path = GetRoundedRectanglePath(ClientRectangle, _borderRadius))
    {
        // Set clip region
        Region = new Region(path);
        
        // Clear và vẽ
        g.Clear(Parent?.BackColor);
        g.FillPath(backgroundBrush, path);
        
        // Border với PenAlignment.Inset
        pen.Alignment = PenAlignment.Inset;
        g.DrawPath(borderPen, path);
    }
}
```

---

## 📊 Technical Details

### Region Clipping:
- ✅ `Region = new Region(path)` - Clip toàn bộ control
- ✅ Chỉ hiển thị phần trong GraphicsPath
- ✅ Border mặc định của controls bị ẩn

### PenAlignment.Inset:
- ✅ Vẽ pen BÊN TRONG path thay vì centered
- ✅ Border không bị lộ ra ngoài
- ✅ Khớp hoàn hảo với background path

### Clear Strategy:
- ✅ CustomButton: Chỉ clear khi Ghost/Transparent
- ✅ CustomDateTimePicker: Clear để che DateTimePicker border
- ✅ CustomComboBox: Clear để che ComboBox border

---

## 🧪 Testing

### Test CustomButton:
1. Vào Components preview
2. Check button style "Filled" (Nền Primary, viền BG)
3. Verify:
   - ✅ Viền hiển thị đầy đủ ở 4 góc
   - ✅ Border radius mượt mà
   - ✅ Không có gap giữa background và border

### Test CustomDateTimePicker:
1. Vào Components preview → "DATE TIME PICKER"
2. Verify:
   - ✅ Border radius hiển thị đẹp
   - ✅ Không thấy border mặc định của DateTimePicker
   - ✅ Chỉ có 1 border (custom border)

### Test CustomComboBox:
1. Vào Components preview → "INPUTS"
2. Verify:
   - ✅ Border radius hiển thị đẹp
   - ✅ Không thấy border mặc định của ComboBox
   - ✅ Dropdown button với border radius

---

## 📁 Files Modified

| File | Lines Changed | Change Type |
|------|---------------|-------------|
| CustomButton.cs | ~15 lines | Logic fix |
| CustomDateTimePicker.cs | ~20 lines | Region clip + position |
| CustomComboBox.cs | ~10 lines | Region clip |

**Total:** 3 files, ~45 lines

---

## 🎯 Results

### Before:
```
CustomButton (Filled):
┌─────────┐
│ ███████ │  ← Border mất ở góc
│ ███████ │
└─────────┘

CustomDateTimePicker:
┌───────────┐  ← Custom border
│ ┌───────┐ │  ← DateTimePicker border (lộ ra)
│ └───────┘ │
└───────────┘

CustomComboBox:
Similar issue
```

### After:
```
CustomButton (Filled):
╭─────────╮  ← Border đầy đủ, mượt mà
│ ███████ │
╰─────────╯

CustomDateTimePicker:
╭───────────╮  ← Chỉ có custom border
│ 18/01/2026│  ← Content bên trong
╰───────────╯

CustomComboBox:
╭───────────╮
│ Option 1 ▼│
╰───────────╯
```

---

## 🔑 Key Techniques

### 1. Conditional Clear:
```csharp
// Chỉ clear khi cần
if (_buttonStyle == ButtonStyle.Ghost || backColor == Color.Transparent)
{
    g.Clear(Parent?.BackColor);
}
```

### 2. PenAlignment.Inset:
```csharp
// Vẽ border BÊN TRONG path
using (Pen pen = new Pen(color, thickness))
{
    pen.Alignment = PenAlignment.Inset;
    g.DrawPath(pen, path);
}
```

### 3. Region Clipping:
```csharp
// Clip control trong GraphicsPath
using (GraphicsPath path = GetRoundedRectanglePath(...))
{
    Region = new Region(path);  // Control chỉ hiển thị trong region này
    // Vẽ background và border...
}
```

### 4. Control Positioning:
```csharp
// Đặt control bên trong lớn hơn container
// Region sẽ clip phần thừa (bao gồm border mặc định)
control.Location = new Point(-2, y - 1);
control.Size = new Size(Width + 4, height);
```

---

## ✅ Build Status

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## 📝 Summary

| Issue | Status | Solution |
|-------|--------|----------|
| Button border at corners | ✅ Fixed | Conditional clear + PenAlignment.Inset |
| DateTimePicker default border | ✅ Fixed | Region clipping |
| ComboBox default border | ✅ Fixed | Region clipping |

**All components now have perfect border radius!** ✅

---

**Version:** 2.0.2  
**Date:** 2026-01-18  
**Status:** ✅ Fixed  
**Build:** ✅ Success
