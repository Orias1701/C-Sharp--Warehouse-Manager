using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Custom ComboBox với border radius và theme support
    /// </summary>
    public class CustomComboBox : ComboBox
    {
        private int _borderRadius = UIConstants.Borders.RadiusMedium;
        private Color _borderColor = UIConstants.PrimaryColor.Default;
        private int _borderThickness = UIConstants.Borders.BorderThickness;
        private Color _buttonColor = UIConstants.PrimaryColor.Default;
        private bool _isFocused = false;

        public CustomComboBox()
        {
            // Thiết lập mặc định
            Padding = UIConstants.Spacing.Padding.Input;
            Margin = UIConstants.Spacing.Margin.Input;
            Font = ThemeManager.Instance.FontRegular;
            
            FlatStyle = FlatStyle.Flat;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
            
            // Set height cố định bằng với Button và TextBox
            ItemHeight = UIConstants.Sizes.InputHeight - 2;
            Height = UIConstants.Sizes.InputHeight;
            
            // Subscribe events
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            Enter += (s, e) => { _isFocused = true; Invalidate(); };
            Leave += (s, e) => { _isFocused = false; Invalidate(); };
            DrawItem += CustomComboBox_DrawItem;
            
            ApplyTheme();
            
            // Double buffering
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

        public Color ButtonColor
        {
            get => _buttonColor;
            set
            {
                _buttonColor = value;
                Invalidate();
            }
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            ForeColor = ThemeManager.Instance.TextPrimary;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            Color currentBorderColor = _isFocused 
                ? ThemeManager.Instance.PrimaryActive 
                : _borderColor;
            
            // Draw background with border radius
            using (GraphicsPath path = GetRoundedRectanglePath(ClientRectangle, _borderRadius))
            {
                // Fill background
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Draw border
                using (Pen pen = new Pen(currentBorderColor, _borderThickness))
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
            
            // Draw button area
            int buttonWidth = 35;
            Rectangle buttonRect = new Rectangle(Width - buttonWidth - 1, 1, buttonWidth, Height - 2);
            
            using (SolidBrush brush = new SolidBrush(_buttonColor))
            {
                // Create rounded path for button
                using (GraphicsPath buttonPath = new GraphicsPath())
                {
                    int radius = _borderRadius - 1;
                    if (radius > 0)
                    {
                        int diameter = radius * 2;
                        buttonPath.AddArc(buttonRect.Right - diameter, buttonRect.Top, diameter, diameter, 270, 90);
                        buttonPath.AddArc(buttonRect.Right - diameter, buttonRect.Bottom - diameter, diameter, diameter, 0, 90);
                        buttonPath.AddLine(buttonRect.Left, buttonRect.Bottom, buttonRect.Left, buttonRect.Top);
                    }
                    else
                    {
                        buttonPath.AddRectangle(buttonRect);
                    }
                    
                    g.FillPath(brush, buttonPath);
                }
            }
            
            // Draw arrow
            DrawArrow(g, buttonRect);
            
            // Draw text
            if (SelectedIndex >= 0)
            {
                Rectangle textRect = new Rectangle(
                    UIConstants.Spacing.Padding.Medium, 
                    0, 
                    Width - buttonWidth - UIConstants.Spacing.Padding.Medium - 5, 
                    Height
                );
                
                TextRenderer.DrawText(g, GetItemText(SelectedItem), Font, textRect, 
                    ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }

        private void DrawArrow(Graphics g, Rectangle rect)
        {
            // Draw dropdown arrow
            int arrowSize = 8;
            Point[] arrow = new Point[3];
            arrow[0] = new Point(rect.Left + (rect.Width - arrowSize) / 2, rect.Top + (rect.Height - arrowSize / 2) / 2);
            arrow[1] = new Point(arrow[0].X + arrowSize, arrow[0].Y);
            arrow[2] = new Point(arrow[0].X + arrowSize / 2, arrow[0].Y + arrowSize / 2);
            
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillPolygon(brush, arrow);
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

        private void CustomComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            
            // Vẽ item
            string text = GetItemText(Items[e.Index]);
            Color textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? SystemColors.HighlightText
                : ThemeManager.Instance.TextPrimary;
            
            using (SolidBrush brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(text, Font, brush, e.Bounds);
            }
            
            e.DrawFocusRectangle();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.Instance.ThemeChanged -= OnThemeChanged;
                DrawItem -= CustomComboBox_DrawItem;
            }
            base.Dispose(disposing);
        }
    }
}
