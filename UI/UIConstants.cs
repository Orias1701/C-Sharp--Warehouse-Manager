using System.Drawing;

namespace WarehouseManagement.UI
{
    /// <summary>
    /// Định nghĩa tất cả các constants cho UI - Colors, Fonts, Sizes, Spacing
    /// </summary>
    public static class UIConstants
    {
        // ========== COLORS ==========
        
        // Primary Color - FF847D và các sắc độ
        public static class PrimaryColor
        {
            public static readonly Color Default = ColorTranslator.FromHtml("#FF847D");
            public static readonly Color Active = ColorTranslator.FromHtml("#FF6B62");   // Đậm hơn khi active
            public static readonly Color Hover = ColorTranslator.FromHtml("#FF9D97");    // Sáng hơn khi hover
            public static readonly Color Pressed = ColorTranslator.FromHtml("#E6776F");  // Tối hơn khi pressed
            public static readonly Color Disabled = ColorTranslator.FromHtml("#FFBFBA"); // Mờ đi khi disabled
            public static readonly Color Light = ColorTranslator.FromHtml("#FFD1CE");    // Rất sáng
            public static readonly Color Dark = ColorTranslator.FromHtml("#CC6964");     // Rất tối
        }

        // Background Colors - Light Theme
        public static class BackgroundLight
        {
            public static readonly Color Default = ColorTranslator.FromHtml("#FFFFFF");
            public static readonly Color Lighter = ColorTranslator.FromHtml("#FAFAFA");
            public static readonly Color Light = ColorTranslator.FromHtml("#F5F5F5");
            public static readonly Color Medium = ColorTranslator.FromHtml("#F0F0F0");
            public static readonly Color Dark = ColorTranslator.FromHtml("#E8E8E8");
            public static readonly Color Darker = ColorTranslator.FromHtml("#D8D8D8");
        }

        // Background Colors - Dark Theme
        public static class BackgroundDark
        {
            public static readonly Color Default = ColorTranslator.FromHtml("#1E1E1E");
            public static readonly Color Lighter = ColorTranslator.FromHtml("#2D2D2D");
            public static readonly Color Light = ColorTranslator.FromHtml("#252525");
            public static readonly Color Medium = ColorTranslator.FromHtml("#1A1A1A");
            public static readonly Color Dark = ColorTranslator.FromHtml("#151515");
            public static readonly Color Darker = ColorTranslator.FromHtml("#0D0D0D");
        }

        // Text Colors - Light Theme
        public static class TextLight
        {
            public static readonly Color Primary = ColorTranslator.FromHtml("#212121");
            public static readonly Color Secondary = ColorTranslator.FromHtml("#666666");
            public static readonly Color Disabled = ColorTranslator.FromHtml("#9E9E9E");
            public static readonly Color Hint = ColorTranslator.FromHtml("#BDBDBD");
        }

        // Text Colors - Dark Theme
        public static class TextDark
        {
            public static readonly Color Primary = ColorTranslator.FromHtml("#FFFFFF");
            public static readonly Color Secondary = ColorTranslator.FromHtml("#B0B0B0");
            public static readonly Color Disabled = ColorTranslator.FromHtml("#6E6E6E");
            public static readonly Color Hint = ColorTranslator.FromHtml("#4A4A4A");
        }

        // Semantic Colors
        public static class SemanticColors
        {
            public static readonly Color Success = ColorTranslator.FromHtml("#4CAF50");
            public static readonly Color Warning = ColorTranslator.FromHtml("#FF9800");
            public static readonly Color Error = ColorTranslator.FromHtml("#F44336");
            public static readonly Color Info = ColorTranslator.FromHtml("#2196F3");
        }

        // ========== FONTS ==========
        
        public static class Fonts
        {
            public const string FontFamily = "Segoe UI";
            
            // Font Sizes (8 cấp độ)
            public const float XXSmall = 9f;   // Rất nhỏ - chú thích phụ
            public const float XSmall = 10f;   // Nhỏ - label phụ
            public const float Small = 11f;    // Nhỏ vừa - label thường
            public const float Regular = 12f;  // Chuẩn - text thường
            public const float Medium = 14f;   // Vừa - subheading
            public const float Large = 16f;    // Lớn - heading
            public const float XLarge = 20f;   // Rất lớn - title
            public const float XXLarge = 24f;  // Cực lớn - main title
        }

        // ========== SIZES ==========
        
        public static class Sizes
        {
            // Button Sizes
            public const int ButtonHeight = 36;
            public const int ButtonWidthSmall = 80;
            public const int ButtonWidthMedium = 120;
            public const int ButtonWidthLarge = 160;

            // Input Sizes
            public const int InputHeight = 36;
            public const int InputHeightSmall = 28;
            public const int InputHeightLarge = 44;

            // Table Row Height
            public const int TableRowHeight = 40;
            public const int TableHeaderHeight = 44;

            // Icon Sizes
            public const int IconSmall = 16;
            public const int IconMedium = 20;
            public const int IconLarge = 24;
        }

        // ========== SPACING ==========
        
        public static class Spacing
        {
            // Padding (vùng an toàn trong)
            public static class Padding
            {
                public const int XXSmall = 2;
                public const int XSmall = 4;
                public const int Small = 8;
                public const int Medium = 12;
                public const int Large = 16;
                public const int XLarge = 20;
                public const int XXLarge = 24;
                
                // Button Padding
                public static readonly System.Windows.Forms.Padding Button = 
                    new System.Windows.Forms.Padding(Medium, Small, Medium, Small);
                
                // Input Padding
                public static readonly System.Windows.Forms.Padding Input = 
                    new System.Windows.Forms.Padding(Medium, Small, Medium, Small);
                
                // Panel Padding
                public static readonly System.Windows.Forms.Padding Panel = 
                    new System.Windows.Forms.Padding(Large);
            }

            // Margin (vùng an toàn ngoài)
            public static class Margin
            {
                public const int XXSmall = 2;
                public const int XSmall = 4;
                public const int Small = 8;
                public const int Medium = 12;
                public const int Large = 16;
                public const int XLarge = 20;
                public const int XXLarge = 24;
                
                // Button Margin
                public static readonly System.Windows.Forms.Padding Button = 
                    new System.Windows.Forms.Padding(Small);
                
                // Input Margin
                public static readonly System.Windows.Forms.Padding Input = 
                    new System.Windows.Forms.Padding(Small);
                
                // Panel Margin
                public static readonly System.Windows.Forms.Padding Panel = 
                    new System.Windows.Forms.Padding(Medium);
            }
        }

        // ========== BORDERS ==========
        
        public static class Borders
        {
            // Border Radius
            public const int RadiusNone = 0;
            public const int RadiusSmall = 4;
            public const int RadiusMedium = 8;
            public const int RadiusLarge = 12;
            public const int RadiusXLarge = 16;
            public const int RadiusFull = 999; // Circular

            // Border Width
            public const int BorderThickness = 1;
            public const int BorderThicknessMedium = 2;
            public const int BorderThicknessThick = 3;
        }

        // ========== ANIMATIONS ==========
        
        public static class Animations
        {
            public const int DurationFast = 100;      // ms
            public const int DurationNormal = 200;    // ms
            public const int DurationSlow = 300;      // ms
        }

        // ========== ICONS ==========
        
        public static class Icons
        {
            // Navigation
            public const string Home = "🏠";
            public const string Menu = "☰";
            public const string Back = "←";
            public const string Forward = "→";
            public const string Close = "✕";
            
            // Actions
            public const string Add = "➕";
            public const string Edit = "✏️";
            public const string Delete = "🗑️";
            public const string Save = "💾";
            public const string Cancel = "❌";
            public const string Refresh = "🔄";
            public const string Search = "🔍";
            public const string Filter = "🔽";
            
            // Status
            public const string Success = "✓";
            public const string Error = "✕";
            public const string Warning = "⚠️";
            public const string Info = "ℹ️";
            
            // Data
            public const string Import = "📥";
            public const string Export = "📤";
            public const string Upload = "⬆️";
            public const string Download = "⬇️";
            
            // Views
            public const string List = "📋";
            public const string Grid = "⊞";
            public const string Chart = "📊";
            public const string Table = "📑";
            
            // Other
            public const string Settings = "⚙️";
            public const string User = "👤";
            public const string Category = "📁";
            public const string Product = "📦";
            public const string Transaction = "💱";
            public const string Report = "📈";
            public const string Calendar = "📅";
            public const string Clock = "🕐";
            public const string Lock = "🔒";
            public const string Unlock = "🔓";
            public const string Eye = "👁️";
            public const string EyeOff = "🙈";
            public const string Sun = "☀️";
            public const string Moon = "🌙";
        }
    }
}
