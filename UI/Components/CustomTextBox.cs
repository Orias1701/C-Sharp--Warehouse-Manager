using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Custom TextBox với border radius, placeholder và theme support
    /// </summary>
    public class CustomTextBox : Control
    {
        private TextBox _textBox;
        private int _borderRadius = UIConstants.Borders.RadiusMedium;
        private Color _borderColor = UIConstants.PrimaryColor.Default;
        private int _borderThickness = UIConstants.Borders.BorderThickness;
        private bool _isFocused = false;
        private string _placeholder = "";
        private bool _isPassword = false;

        public CustomTextBox()
        {
            // Tạo TextBox bên trong
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = ThemeManager.Instance.FontRegular,
                Location = new Point(UIConstants.Spacing.Padding.Medium, UIConstants.Spacing.Padding.Small + 2),
            };

            _textBox.TextChanged += (s, e) => OnTextChanged(e);
            _textBox.Enter += (s, e) => { _isFocused = true; Invalidate(); };
            _textBox.Leave += (s, e) => { _isFocused = false; Invalidate(); };
            _textBox.GotFocus += TextBox_GotFocus;
            _textBox.LostFocus += TextBox_LostFocus;

            // Thiết lập control
            Height = UIConstants.Sizes.InputHeight;
            Padding = UIConstants.Spacing.Padding.Input;
            Margin = UIConstants.Spacing.Margin.Input;
            
            Controls.Add(_textBox);
            
            // Subscribe theme changed
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            
            ApplyTheme();
            
            // Double buffering
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);

            UpdateTextBoxSize();
        }

        // Properties
        public override string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value;
        }

        public string Placeholder
        {
            get => _placeholder;
            set
            {
                _placeholder = value;
                UpdatePlaceholder();
            }
        }

        public bool IsPassword
        {
            get => _isPassword;
            set
            {
                _isPassword = value;
                _textBox.UseSystemPasswordChar = value;
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

        public int MaxLength
        {
            get => _textBox.MaxLength;
            set => _textBox.MaxLength = value;
        }

        public bool ReadOnly
        {
            get => _textBox.ReadOnly;
            set => _textBox.ReadOnly = value;
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            _textBox.BackColor = ThemeManager.Instance.BackgroundDefault;
            _textBox.ForeColor = ThemeManager.Instance.TextPrimary;
            UpdatePlaceholder();
            Invalidate();
        }

        private void UpdatePlaceholder()
        {
            if (!_isFocused && string.IsNullOrEmpty(_textBox.Text) && !string.IsNullOrEmpty(_placeholder))
            {
                _textBox.Text = _placeholder;
                _textBox.ForeColor = ThemeManager.Instance.TextHint;
            }
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            if (_textBox.Text == _placeholder)
            {
                _textBox.Text = "";
                _textBox.ForeColor = ThemeManager.Instance.TextPrimary;
            }
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_textBox.Text))
            {
                UpdatePlaceholder();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateTextBoxSize();
        }

        private void UpdateTextBoxSize()
        {
            if (_textBox != null)
            {
                _textBox.Size = new Size(
                    Width - UIConstants.Spacing.Padding.Medium * 2,
                    Height - UIConstants.Spacing.Padding.Small * 2
                );
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
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
                _textBox?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
