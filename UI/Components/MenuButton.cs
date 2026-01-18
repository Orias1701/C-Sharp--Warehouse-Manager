using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Custom button component specifically designed for menu/sidebar navigation
    /// Features: Full width, no border radius, bold text, primary color with hover/active states
    /// </summary>
    public class MenuButton : Button
    {
        private bool _isHovered = false;
        private bool _isSelected = false;

        public MenuButton()
        {
            // Default settings
            Height = 55;
            Dock = DockStyle.Top;
            Padding = new Padding(UIConstants.Spacing.Padding.Medium);
            Margin = new Padding(0); // No margin for full width
            Font = ThemeManager.Instance.FontBold;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            TextAlign = ContentAlignment.MiddleLeft;
            
            // Subscribe to theme changes
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            
            ApplyStyle();
            
            // Double buffering for smooth rendering
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);
            
            UpdateStyles();
        }

        /// <summary>
        /// Gets or sets whether this button is currently selected/active
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                Invalidate();
            }
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyStyle();
        }

        private void ApplyStyle()
        {
            // Base colors - transparent background, primary color text
            BackColor = Color.Transparent;
            ForeColor = ThemeManager.Instance.PrimaryDefault;
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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Clear background
            g.Clear(Parent?.BackColor ?? ThemeManager.Instance.BackgroundDefault);
            
            // Determine colors based on state
            Color backColor = GetBackgroundColor();
            Color foreColor = GetForegroundColor();
            
            // Draw background (no border radius - sharp corners)
            Rectangle bgRect = ClientRectangle;
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, bgRect);
            }
            
            // Draw text with left alignment and padding
            Rectangle textRect = new Rectangle(
                ClientRectangle.X + Padding.Left,
                ClientRectangle.Y,
                ClientRectangle.Width - Padding.Left - Padding.Right,
                ClientRectangle.Height
            );
            
            TextRenderer.DrawText(g, Text, Font, textRect, foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private Color GetBackgroundColor()
        {
            if (!Enabled)
            {
                return ThemeManager.Instance.BackgroundLight;
            }

            // Selected state (active)
            if (_isSelected)
            {
                return ThemeManager.Instance.PrimaryActive;
            }

            // Hover state
            if (_isHovered)
            {
                return ThemeManager.Instance.PrimaryHover;
            }

            // Default state - transparent
            return Color.Transparent;
        }

        private Color GetForegroundColor()
        {
            if (!Enabled)
            {
                return ThemeManager.Instance.TextDisabled;
            }

            // Selected or hover state - use Light color for hover, White for selected
            if (_isSelected)
            {
                return Color.White;
            }

            if (_isHovered)
            {
                return ThemeManager.Instance.PrimaryLight;
            }

            // Default state - primary color
            return ThemeManager.Instance.PrimaryDefault;
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
