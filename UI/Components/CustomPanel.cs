using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Custom Panel với border radius, padding và margin
    /// </summary>
    public class CustomPanel : Panel
    {
        private int _borderRadius = UIConstants.Borders.RadiusMedium;
        private Color _borderColor = UIConstants.PrimaryColor.Default;
        private int _borderThickness = UIConstants.Borders.BorderThickness;
        private bool _showBorder = true;
        private bool _useThemeBackColor = true;

        public CustomPanel()
        {
            // Thiết lập mặc định
            Padding = UIConstants.Spacing.Padding.Panel;
            Margin = UIConstants.Spacing.Margin.Panel;
            
            // Subscribe theme changed event
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            
            // Only apply theme colors if UseThemeBackColor is true
            // This allows object initializer to set custom BackColor
            if (_useThemeBackColor)
                ApplyTheme();
            
            // Double buffering để giảm flicker
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
        }

        // Properties
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                Invalidate();
            }
        }

        public bool ShowBorder
        {
            get => _showBorder;
            set
            {
                _showBorder = value;
                Invalidate();
            }
        }

        public bool UseThemeBackColor
        {
            get => _useThemeBackColor;
            set
            {
                _useThemeBackColor = value;
                if (value)
                    ApplyTheme();
            }
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
            base.OnPaint(e);
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Rectangle cho background
            Rectangle bgRect = ClientRectangle;
            
            // Rectangle cho border (shrink để không bị clip)
            Rectangle borderRect = new Rectangle(
                ClientRectangle.X,
                ClientRectangle.Y,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - 1
            );
            
            // Set Region để clip child controls theo border radius
            if (_borderRadius > 0)
            {
                using (GraphicsPath regionPath = GetRoundedRectanglePath(bgRect, _borderRadius))
                {
                    this.Region = new Region(regionPath);
                }
            }
            
            // Draw background
            using (GraphicsPath bgPath = GetRoundedRectanglePath(bgRect, _borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillPath(brush, bgPath);
                }
            }
            
            // Draw border
            if (_showBorder)
            {
                using (GraphicsPath borderPath = GetRoundedRectanglePath(borderRect, _borderRadius))
                {
                    using (Pen pen = new Pen(_borderColor, _borderThickness))
                    {
                        e.Graphics.DrawPath(pen, borderPath);
                    }
                }
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
