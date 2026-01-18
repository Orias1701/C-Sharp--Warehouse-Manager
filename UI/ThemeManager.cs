using System;
using System.Drawing;
using System.Windows.Forms;

namespace WarehouseManagement.UI
{
    /// <summary>
    /// Quản lý theme (Dark/Light mode) cho toàn bộ ứng dụng
    /// Sử dụng Singleton pattern
    /// </summary>
    public class ThemeManager
    {
        private static ThemeManager _instance;
        private static readonly object _lock = new object();
        
        private bool _isDarkMode;

        // Singleton Instance
        public static ThemeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ThemeManager();
                        }
                    }
                }
                return _instance;
            }
        }

        // Event khi theme thay đổi
        public event EventHandler ThemeChanged;

        private ThemeManager()
        {
            _isDarkMode = false; // Mặc định Light mode
        }

        // Properties
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    OnThemeChanged();
                }
            }
        }

        // Toggle Dark/Light mode
        public void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
        }

        // Trigger event
        protected virtual void OnThemeChanged()
        {
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        // ========== GET CURRENT COLORS ==========

        // Background Colors
        public Color BackgroundDefault => _isDarkMode 
            ? UIConstants.BackgroundDark.Default 
            : UIConstants.BackgroundLight.Default;

        public Color BackgroundLighter => _isDarkMode 
            ? UIConstants.BackgroundDark.Lighter 
            : UIConstants.BackgroundLight.Lighter;

        public Color BackgroundLight => _isDarkMode 
            ? UIConstants.BackgroundDark.Light 
            : UIConstants.BackgroundLight.Light;

        public Color BackgroundMedium => _isDarkMode 
            ? UIConstants.BackgroundDark.Medium 
            : UIConstants.BackgroundLight.Medium;

        public Color BackgroundDark => _isDarkMode 
            ? UIConstants.BackgroundDark.Dark 
            : UIConstants.BackgroundLight.Dark;

        public Color BackgroundDarker => _isDarkMode 
            ? UIConstants.BackgroundDark.Darker 
            : UIConstants.BackgroundLight.Darker;

        // Text Colors
        public Color TextPrimary => _isDarkMode 
            ? UIConstants.TextDark.Primary 
            : UIConstants.TextLight.Primary;

        public Color TextSecondary => _isDarkMode 
            ? UIConstants.TextDark.Secondary 
            : UIConstants.TextLight.Secondary;

        public Color TextDisabled => _isDarkMode 
            ? UIConstants.TextDark.Disabled 
            : UIConstants.TextLight.Disabled;

        public Color TextHint => _isDarkMode 
            ? UIConstants.TextDark.Hint 
            : UIConstants.TextLight.Hint;

        // Primary Colors (không thay đổi theo theme)
        public Color PrimaryDefault => UIConstants.PrimaryColor.Default;
        public Color PrimaryActive => UIConstants.PrimaryColor.Active;
        public Color PrimaryHover => UIConstants.PrimaryColor.Hover;
        public Color PrimaryPressed => UIConstants.PrimaryColor.Pressed;
        public Color PrimaryDisabled => UIConstants.PrimaryColor.Disabled;
        public Color PrimaryLight => UIConstants.PrimaryColor.Light;
        public Color PrimaryDark => UIConstants.PrimaryColor.Dark;

        // ========== APPLY THEME TO CONTROL ==========

        /// <summary>
        /// Áp dụng theme cho một control
        /// </summary>
        public void ApplyTheme(Control control)
        {
            if (control == null) return;

            // Áp dụng cho control hiện tại
            control.BackColor = BackgroundDefault;
            control.ForeColor = TextPrimary;

            // Áp dụng đệ quy cho các control con
            foreach (Control child in control.Controls)
            {
                ApplyTheme(child);
            }
        }

        /// <summary>
        /// Áp dụng theme cho toàn bộ Form
        /// </summary>
        public void ApplyThemeToForm(Form form)
        {
            if (form == null) return;

            form.BackColor = BackgroundDefault;
            form.ForeColor = TextPrimary;

            ApplyTheme(form);
        }

        // ========== GET FONT ==========

        public Font GetFont(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(UIConstants.Fonts.FontFamily, size, style);
        }

        public Font FontXXSmall => GetFont(UIConstants.Fonts.XXSmall);
        public Font FontXSmall => GetFont(UIConstants.Fonts.XSmall);
        public Font FontSmall => GetFont(UIConstants.Fonts.Small);
        public Font FontRegular => GetFont(UIConstants.Fonts.Regular);
        public Font FontMedium => GetFont(UIConstants.Fonts.Medium);
        public Font FontLarge => GetFont(UIConstants.Fonts.Large);
        public Font FontXLarge => GetFont(UIConstants.Fonts.XLarge);
        public Font FontXXLarge => GetFont(UIConstants.Fonts.XXLarge);

        public Font FontBold => GetFont(UIConstants.Fonts.Regular, FontStyle.Bold);
        public Font FontBoldLarge => GetFont(UIConstants.Fonts.Large, FontStyle.Bold);
    }
}
