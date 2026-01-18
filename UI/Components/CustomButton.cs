using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Các kiểu button khả dụng
    /// </summary>
    public enum ButtonStyle
    {
        /// <summary>Nền BG, viền PrimaryColor</summary>
        Outlined,
        
        /// <summary>Nền PrimaryColor, viền BG</summary>
        Filled,
        
        /// <summary>Nền BG, viền Transparent</summary>
        Text,
        
        /// <summary>Nền PrimaryColor, viền Transparent</summary>
        FilledNoOutline,
        
        /// <summary>Nền và viền Transparent</summary>
        Ghost
    }

    /// <summary>
    /// Custom Button với 5 styles khác nhau, border radius và state management
    /// </summary>
    public class CustomButton : Button
    {
        private ButtonStyle _buttonStyle = ButtonStyle.Filled;
        private int _borderRadius = UIConstants.Borders.RadiusMedium;
        private bool _isHovered = false;
        private bool _isPressed = false;

        public CustomButton()
        {
            // Thiết lập mặc định
            Height = UIConstants.Sizes.ButtonHeight;
            Width = UIConstants.Sizes.ButtonWidthMedium;
            Padding = UIConstants.Spacing.Padding.Button;
            Margin = UIConstants.Spacing.Margin.Button;
            Font = ThemeManager.Instance.FontRegular;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            
            // Subscribe events
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            
            ApplyStyle();
            
            // Double buffering
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);
        }

        // Properties
        public ButtonStyle ButtonStyleType
        {
            get => _buttonStyle;
            set
            {
                _buttonStyle = value;
                ApplyStyle();
                Invalidate();
            }
        }

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyStyle();
        }

        private void ApplyStyle()
        {
            Color backColor, foreColor, borderColor;
            bool hasBorder = true;
            
            switch (_buttonStyle)
            {
                case ButtonStyle.Outlined:
                    // Nền BG, viền PrimaryColor
                    backColor = ThemeManager.Instance.BackgroundDefault;
                    foreColor = ThemeManager.Instance.PrimaryDefault;
                    borderColor = ThemeManager.Instance.PrimaryDefault;
                    hasBorder = true;
                    break;
                    
                case ButtonStyle.Filled:
                    // Nền PrimaryColor, viền BG (hoặc viền cùng màu)
                    backColor = ThemeManager.Instance.PrimaryDefault;
                    foreColor = Color.White;
                    borderColor = ThemeManager.Instance.BackgroundDefault;
                    hasBorder = true;
                    break;
                    
                case ButtonStyle.Text:
                    // Nền BG, không có viền
                    backColor = ThemeManager.Instance.BackgroundDefault;
                    foreColor = ThemeManager.Instance.PrimaryDefault;
                    borderColor = Color.Transparent;
                    hasBorder = false;
                    break;
                    
                case ButtonStyle.FilledNoOutline:
                    // Nền PrimaryColor, không có viền
                    backColor = ThemeManager.Instance.PrimaryDefault;
                    foreColor = Color.White;
                    borderColor = Color.Transparent;
                    hasBorder = false;
                    break;
                    
                case ButtonStyle.Ghost:
                    // Nền và viền Transparent
                    backColor = Color.Transparent;
                    foreColor = ThemeManager.Instance.PrimaryDefault;
                    borderColor = Color.Transparent;
                    hasBorder = false;
                    break;
                    
                default:
                    backColor = ThemeManager.Instance.PrimaryDefault;
                    foreColor = Color.White;
                    borderColor = ThemeManager.Instance.PrimaryDefault;
                    hasBorder = true;
                    break;
            }

            BackColor = backColor;
            ForeColor = foreColor;
            
            // Chỉ set BorderColor khi có border (không phải Transparent)
            // Windows Forms không cho phép set BorderColor = Transparent
            if (hasBorder)
            {
                FlatAppearance.BorderSize = 0; // Vẫn set 0 vì ta tự vẽ border trong OnPaint
                // Không set BorderColor vì ta tự vẽ
            }
            else
            {
                FlatAppearance.BorderSize = 0;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isPressed = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Clear toàn bộ background trước để tránh hiển thị backdrop
            g.Clear(Parent?.BackColor ?? SystemColors.Control);
            
            // Determine colors based on state
            Color backColor = GetBackgroundColor();
            Color foreColor = GetForegroundColor();
            Color borderColor = GetBorderColor();
            
            // Draw background with border radius
            using (GraphicsPath path = GetRoundedRectanglePath(ClientRectangle, _borderRadius))
            {
                // Fill background
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Draw border (nếu không transparent)
                if (borderColor != Color.Transparent)
                {
                    using (Pen pen = new Pen(borderColor, UIConstants.Borders.BorderThickness))
                    {
                        Rectangle borderRect = ClientRectangle;
                        borderRect.Width -= 1;
                        borderRect.Height -= 1;
                        
                        using (GraphicsPath borderPath = GetRoundedRectanglePath(borderRect, _borderRadius))
                        {
                            g.DrawPath(pen, borderPath);
                        }
                    }
                }
            }
            
            // Draw text
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private Color GetBackgroundColor()
        {
            if (!Enabled)
            {
                return _buttonStyle == ButtonStyle.Outlined || _buttonStyle == ButtonStyle.Text
                    ? ThemeManager.Instance.BackgroundLight
                    : ThemeManager.Instance.PrimaryDisabled;
            }

            if (_isPressed)
            {
                return _buttonStyle == ButtonStyle.Outlined || _buttonStyle == ButtonStyle.Text
                    ? ThemeManager.Instance.BackgroundMedium
                    : ThemeManager.Instance.PrimaryPressed;
            }

            if (_isHovered)
            {
                return _buttonStyle == ButtonStyle.Outlined || _buttonStyle == ButtonStyle.Text
                    ? ThemeManager.Instance.BackgroundLight
                    : ThemeManager.Instance.PrimaryHover;
            }

            switch (_buttonStyle)
            {
                case ButtonStyle.Outlined:
                case ButtonStyle.Text:
                    return ThemeManager.Instance.BackgroundDefault;
                case ButtonStyle.Filled:
                case ButtonStyle.FilledNoOutline:
                    return ThemeManager.Instance.PrimaryDefault;
                case ButtonStyle.Ghost:
                    return Color.Transparent;
                default:
                    return ThemeManager.Instance.PrimaryDefault;
            }
        }

        private Color GetForegroundColor()
        {
            if (!Enabled)
            {
                return ThemeManager.Instance.TextDisabled;
            }

            switch (_buttonStyle)
            {
                case ButtonStyle.Outlined:
                case ButtonStyle.Text:
                case ButtonStyle.Ghost:
                    return ThemeManager.Instance.PrimaryDefault;
                case ButtonStyle.Filled:
                case ButtonStyle.FilledNoOutline:
                    return Color.White;
                default:
                    return Color.White;
            }
        }

        private Color GetBorderColor()
        {
            if (!Enabled)
            {
                return _buttonStyle == ButtonStyle.Outlined 
                    ? ThemeManager.Instance.PrimaryDisabled 
                    : Color.Transparent;
            }

            switch (_buttonStyle)
            {
                case ButtonStyle.Outlined:
                    return ThemeManager.Instance.PrimaryDefault;
                case ButtonStyle.Filled:
                    return ThemeManager.Instance.BackgroundDefault;
                case ButtonStyle.Text:
                case ButtonStyle.FilledNoOutline:
                case ButtonStyle.Ghost:
                    return Color.Transparent;
                default:
                    return ThemeManager.Instance.PrimaryDefault;
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);

            // Top left
            path.AddArc(arc, 180, 90);

            // Top right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
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
