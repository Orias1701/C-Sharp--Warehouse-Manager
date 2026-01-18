using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Custom Panel với border radius, padding, margin và drop shadow
    /// </summary>
    public class CustomPanel : Panel
    {
        private int _borderRadius = UIConstants.Borders.RadiusMedium;
        private Color _borderColor = UIConstants.PrimaryColor.Default;
        private int _borderThickness = UIConstants.Borders.BorderThickness;
        private bool _showBorder = true;
        private bool _useThemeBackColor = true;
        
        // Shadow properties
        private bool _hasShadow = false;
        private int _shadowSize = 5;
        private Color _shadowColor = Color.FromArgb(20, 0, 0, 0); // Very subtle shadow
        private int _shadowOffsetX = 0;
        private int _shadowOffsetY = 0;

        public CustomPanel()
        {
            // Thiết lập mặc định
            Padding = UIConstants.Spacing.Padding.Panel;
            Margin = UIConstants.Spacing.Margin.Panel;
            
            // Subscribe theme changed event
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            
            // Only apply theme colors if UseThemeBackColor is true
            if (_useThemeBackColor)
                ApplyTheme();
            
            // Double buffering
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
        }

        // Properties
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = value; Invalidate(); }
        }

        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        public bool UseThemeBackColor
        {
            get => _useThemeBackColor;
            set
            {
                _useThemeBackColor = value;
                if (value) ApplyTheme();
            }
        }

        public bool HasShadow
        {
            get => _hasShadow;
            set { _hasShadow = value; Invalidate(); }
        }

        public int ShadowSize
        {
            get => _shadowSize;
            set { _shadowSize = value; Invalidate(); }
        }

        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        public int ShadowOffsetX
        {
            get => _shadowOffsetX;
            set { _shadowOffsetX = value; Invalidate(); }
        }

        public int ShadowOffsetY
        {
            get => _shadowOffsetY;
            set { _shadowOffsetY = value; Invalidate(); }
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (_useThemeBackColor)
                BackColor = ThemeManager.Instance.BackgroundDefault;
            ForeColor = ThemeManager.Instance.TextPrimary;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Xác định vùng vẽ
            Rectangle clientRect = ClientRectangle;
            
            // Nếu có shadow, thu nhỏ vùng nội dung chính lại để chừa chỗ cho shadow
            // Lưu ý: Cần tính toán offset để dịch chuyển content đúng chỗ
            Rectangle contentRect = clientRect;
            
            if (_hasShadow)
            {
                // Giảm kích thước vùng content để chừa chỗ cho shadow
                contentRect.Width -= Math.Abs(_shadowSize);
                contentRect.Height -= Math.Abs(_shadowSize);

                // Nếu không set offset thủ công, dùng default làm lệch sang phải dưới
                int offX = _shadowOffsetX != 0 ? _shadowOffsetX : _shadowSize / 2;
                int offY = _shadowOffsetY != 0 ? _shadowOffsetY : _shadowSize / 2;

                // Điều chỉnh vị trí của contentRect để shadow không bị cắt
                // Nếu shadow lệch sang phải (offX > 0), content phải lệch sang trái (hoặc giữ nguyên nếu shadow vẽ ra ngoài content)
                // Tuy nhiên ta đang vẽ trong ClientRect nên:
                // Nếu shadow ở dưới (offY > 0), content ở trên.
                // Nếu shadow ở trên (offY < 0), content ở dưới.
                
                if (offX < 0) contentRect.X += Math.Abs(offX); // Shadow bên trái -> content dịch phải
                if (offY < 0) contentRect.Y += Math.Abs(offY); // Shadow bên trên -> content dịch xuống
            }
            
            // Đảm bảo kích thước tối thiểu
            if (contentRect.Width <= 1 || contentRect.Height <= 1) return;

            // path cho content (background)
            using (GraphicsPath path = GetRoundedRectanglePath(contentRect, _borderRadius))
            {
                // 1. Vẽ Shadow nếu có
                if (_hasShadow)
                {
                    int offX = _shadowOffsetX != 0 ? _shadowOffsetX : _shadowSize / 2;
                    int offY = _shadowOffsetY != 0 ? _shadowOffsetY : _shadowSize / 2;
                    
                    Rectangle shadowRect = contentRect;
                    shadowRect.Offset(offX, offY);

                    // Nếu offset = 0, shadow sẽ nằm chính giữa sau lưng content (tạo hiệu ứng glow nhẹ xung quanh)
                    // Nếu offset < 0, shadow sẽ nằm bên trên/trái.

                    using (GraphicsPath shadowPath = GetRoundedRectanglePath(shadowRect, _borderRadius))
                    {
                        using (SolidBrush shadowBrush = new SolidBrush(_shadowColor))
                        {
                            e.Graphics.FillPath(shadowBrush, shadowPath);
                        }
                    }
                }

                // 2. Vẽ Background
                using (SolidBrush bgBrush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillPath(bgBrush, path);
                }

                // 3. Vẽ Border nếu có
                if (_showBorder)
                {
                    using (Pen pen = new Pen(_borderColor, _borderThickness))
                    {
                        pen.Alignment = PenAlignment.Center;
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
            
            this.Region = null;
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
