using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Custom DateTimePicker với border radius và theme support
    /// </summary>
    public class CustomDateTimePicker : Control
    {
        private DateTimePicker _dateTimePicker;
        private int _borderRadius = UIConstants.Borders.RadiusMedium;
        private Color _borderColor = UIConstants.PrimaryColor.Default;
        private int _borderThickness = UIConstants.Borders.BorderThickness;
        private bool _isFocused = false;

        public CustomDateTimePicker()
        {
            // Tạo DateTimePicker bên trong - FLAT STYLE để không có border
            _dateTimePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Font = ThemeManager.Instance.FontRegular
            };

            _dateTimePicker.ValueChanged += (s, e) => OnValueChanged(e);
            _dateTimePicker.Enter += (s, e) => { _isFocused = true; Invalidate(); };
            _dateTimePicker.Leave += (s, e) => { _isFocused = false; Invalidate(); };

            // Thiết lập control
            Height = UIConstants.Sizes.InputHeight;
            Padding = UIConstants.Spacing.Padding.Input;
            Margin = UIConstants.Spacing.Margin.Input;
            
            Controls.Add(_dateTimePicker);
            
            // Subscribe theme changed
            ThemeManager.Instance.ThemeChanged += OnThemeChanged;
            
            ApplyTheme();
            
            // Double buffering
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);

            UpdateDateTimePickerSize();
        }

        // Properties
        public DateTime Value
        {
            get => _dateTimePicker.Value;
            set => _dateTimePicker.Value = value;
        }

        public DateTimePickerFormat Format
        {
            get => _dateTimePicker.Format;
            set => _dateTimePicker.Format = value;
        }

        public string CustomFormat
        {
            get => _dateTimePicker.CustomFormat;
            set => _dateTimePicker.CustomFormat = value;
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

        public DateTime MinDate
        {
            get => _dateTimePicker.MinDate;
            set => _dateTimePicker.MinDate = value;
        }

        public DateTime MaxDate
        {
            get => _dateTimePicker.MaxDate;
            set => _dateTimePicker.MaxDate = value;
        }

        public bool ShowUpDown
        {
            get => _dateTimePicker.ShowUpDown;
            set => _dateTimePicker.ShowUpDown = value;
        }

        // Events
        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Instance.BackgroundDefault;
            _dateTimePicker.BackColor = ThemeManager.Instance.BackgroundDefault;
            _dateTimePicker.ForeColor = ThemeManager.Instance.TextPrimary;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDateTimePickerSize();
        }

        private void UpdateDateTimePickerSize()
        {
            if (_dateTimePicker != null)
            {
                // Tính toán vị trí Y để center theo chiều dọc
                int dtpHeight = _dateTimePicker.PreferredHeight;
                int yPosition = (Height - dtpHeight) / 2;
                
                // Đặt DateTimePicker vừa khít trong container
                _dateTimePicker.Location = new Point(UIConstants.Spacing.Padding.Medium, yPosition);
                _dateTimePicker.Size = new Size(
                    Width - UIConstants.Spacing.Padding.Medium * 2,
                    dtpHeight
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
            
            // Rectangle cho background
            Rectangle bgRect = ClientRectangle;
            
            // Rectangle cho border (shrink để không bị clip)
            Rectangle borderRect = new Rectangle(
                ClientRectangle.X,
                ClientRectangle.Y,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - 1
            );
            
            // Draw background
            using (GraphicsPath bgPath = GetRoundedRectanglePath(bgRect, _borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, bgPath);
                }
            }
            
            // Draw border
            using (GraphicsPath borderPath = GetRoundedRectanglePath(borderRect, _borderRadius))
            {
                using (Pen pen = new Pen(currentBorderColor, _borderThickness))
                {
                    g.DrawPath(pen, borderPath);
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
                _dateTimePicker?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
