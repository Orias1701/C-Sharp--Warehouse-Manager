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
            public static readonly Color Light = ColorTranslator.FromHtml("#FFE5E3");    // Màu nhẹ cho selection
            public static readonly Color Disabled = ColorTranslator.FromHtml("#FFBFBA"); // Mờ đi khi disabled
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
            // ===== NAVIGATION =====
            public const string Home = "🏠";
            public const string Menu = "☰";
            public const string MenuDots = "⋮";
            public const string MenuDotsHorizontal = "⋯";
            public const string Back = "←";
            public const string Forward = "→";
            public const string Up = "↑";
            public const string Down = "↓";
            public const string Left = "←";
            public const string Right = "→";
            public const string Close = "✕";
            public const string Minimize = "−";
            public const string Maximize = "□";
            public const string Fullscreen = "⛶";
            public const string ExitFullscreen = "⊡";
            
            // ===== ACTIONS =====
            public const string Add = "➕";
            public const string AddCircle = "⊕";
            public const string Remove = "➖";
            public const string RemoveCircle = "⊖";
            public const string Edit = "✏️";
            public const string EditAlt = "📝";
            public const string Delete = "🗑️";
            public const string DeleteAlt = "✖";
            public const string Save = "✔";
            public const string SaveAlt = "✓";
            public const string Cancel = "❌";
            public const string CancelAlt = "✗";
            public const string Refresh = "↺";
            public const string RefreshAlt = "↻";
            public const string Search = "🔍";
            public const string Filter = "🔽";
            public const string FilterAlt = "⊲";
            public const string Sort = "⇅";
            public const string SortAsc = "↑";
            public const string SortDesc = "↓";
            public const string Copy = "📋";
            public const string Cut = "✂️";
            public const string Paste = "📄";
            public const string Undo = "➲";
            public const string Redo = "↷";
            public const string Print = "🖨️";
            public const string Download = "⬇️";
            public const string Upload = "⬆️";
            public const string Import = "📥";
            public const string Export = "📤";
            public const string Share = "📤";
            public const string Send = "📮";
            public const string Pin = "📌";
            public const string Unpin = "📍";
            
            // ===== STATUS & ALERTS =====
            public const string Success = "✓";
            public const string SuccessCircle = "✔";
            public const string Error = "✕";
            public const string ErrorCircle = "⊗";
            public const string Warning = "⚠️";
            public const string Info = "ℹ️";
            public const string InfoCircle = "🛈";
            public const string Help = "❓";
            public const string HelpCircle = "❔";
            public const string Question = "❓";
            public const string Exclamation = "❗";
            public const string Loading = "⟳";
            public const string Done = "✓";
            public const string Pending = "⏳";
            public const string Block = "🚫";
            public const string Detail = "📄";
            
            // ===== FILES & FOLDERS =====
            public const string File = "📄";
            public const string FileText = "📃";
            public const string FileImage = "🖼️";
            public const string FileVideo = "🎬";
            public const string FileAudio = "🎵";
            public const string FileZip = "🗜️";
            public const string FileCode = "📜";
            public const string FilePdf = "📕";
            public const string Folder = "📁";
            public const string FolderOpen = "📂";
            public const string FolderAdd = "📁➕";
            public const string Category = "📁";
            public const string Archive = "🗄️";
            public const string Document = "📰";
            public const string Documents = "📚";
            
            // ===== COMMUNICATION =====
            public const string Mail = "✉️";
            public const string MailOpen = "📧";
            public const string Message = "💬";
            public const string MessageCircle = "🗨️";
            public const string Chat = "💬";
            public const string Phone = "📞";
            public const string PhoneCall = "📲";
            public const string Notification = "🔔";
            public const string NotificationOff = "🔕";
            public const string Alert = "🚨";
            public const string Inbox = "📨";
            
            // ===== MEDIA & PLAYBACK =====
            public const string Play = "▶";
            public const string Pause = "⏸";
            public const string Stop = "⏹";
            public const string Record = "⏺";
            public const string PlayCircle = "▶️";
            public const string SkipBack = "⏮";
            public const string SkipForward = "⏭";
            public const string Rewind = "⏪";
            public const string FastForward = "⏩";
            public const string Volume = "🔊";
            public const string VolumeMute = "🔇";
            public const string VolumeUp = "🔊";
            public const string VolumeDown = "🔉";
            public const string Camera = "📷";
            public const string CameraAlt = "📸";
            public const string Video = "🎥";
            public const string Microphone = "🎤";
            public const string MicrophoneOff = "🔇";
            public const string Image = "🖼️";
            public const string Images = "🌄";
            
            // ===== BUSINESS & COMMERCE =====
            public const string Product = "📦";
            public const string Package = "📦";
            public const string Transaction = "💱";
            public const string Money = "💰";
            public const string Dollar = "💵";
            public const string Euro = "💶";
            public const string Yen = "💴";
            public const string CreditCard = "💳";
            public const string Cart = "🛒";
            public const string CartAdd = "🛒➕";
            public const string Bag = "🛍️";
            public const string Tag = "🏷️";
            public const string Tags = "🏷️";
            public const string Barcode = "📊";
            public const string QRCode = "▦";
            public const string Receipt = "🧾";
            public const string Invoice = "📑";
            public const string Report = "📈";
            public const string Chart = "📊";
            public const string ChartBar = "📊";
            public const string ChartLine = "📈";
            public const string ChartPie = "⊙";
            public const string Analytics = "📉";
            public const string Trending = "📈";
            public const string TrendingUp = "📈";
            public const string TrendingDown = "📉";
            
            // ===== USER & ACCOUNT =====
            public const string User = "👤";
            public const string Users = "👥";
            public const string UserAdd = "👤➕";
            public const string UserRemove = "👤➖";
            public const string UserCircle = "👤";
            public const string Account = "👤";
            public const string Profile = "👤";
            public const string Login = "🔓";
            public const string Logout = "🔒";
            public const string Lock = "🔒";
            public const string Unlock = "🔓";
            public const string Key = "🔑";
            public const string Password = "🔐";
            public const string Shield = "🛡️";
            public const string Security = "🔒";
            
            // ===== VIEWS & LAYOUT =====
            public const string List = "📋";
            public const string ListAlt = "☰";
            public const string Grid = "⊞";
            public const string GridAlt = "▦";
            public const string Table = "📑";
            public const string Kanban = "▦";
            public const string Columns = "|||";
            public const string Rows = "☰";
            public const string Layout = "▦";
            public const string Dashboard = "▦";
            public const string Window = "🗔";
            public const string Windows = "🗗";
            
            // ===== UI CONTROLS =====
            public const string Settings = "⚙️";
            public const string SettingsAlt = "🔧";
            public const string Tools = "🛠️";
            public const string Sliders = "🎚️";
            public const string Toggle = "⎚";
            public const string Checkbox = "☑";
            public const string CheckboxEmpty = "☐";
            public const string Radio = "⦿";
            public const string RadioEmpty = "○";
            public const string Dropdown = "▼";
            public const string DropdownUp = "▲";
            public const string ExpandMore = "▼";
            public const string ExpandLess = "▲";
            public const string ChevronRight = ">";
            public const string ChevronLeft = "<";
            public const string ChevronUp = "^";
            public const string ChevronDown = "v";
            
            // ===== TIME & CALENDAR =====
            public const string Calendar = "📅";
            public const string CalendarAlt = "🗓️";
            public const string Clock = "🕐";
            public const string ClockAlt = "⏰";
            public const string Timer = "⏱️";
            public const string Stopwatch = "⏱️";
            public const string Hourglass = "⌛";
            public const string Time = "🕐";
            public const string Today = "📅";
            public const string Week = "📆";
            public const string Month = "📆";
            
            // ===== VISIBILITY =====
            public const string Eye = "👁️";
            public const string EyeOff = "🙈";
            public const string Visible = "👁️";
            public const string Hidden = "🙈";
            public const string Show = "👁️";
            public const string Hide = "🙈";
            
            // ===== SOCIAL & INTERACTION =====
            public const string Like = "👍";
            public const string Dislike = "👎";
            public const string Heart = "♥";
            public const string HeartOutline = "♡";
            public const string Star = "⭐";
            public const string StarOutline = "☆";
            public const string StarHalf = "⭒";
            public const string Favorite = "⭐";
            public const string Bookmark = "🔖";
            public const string BookmarkOutline = "🔖";
            public const string Comment = "💬";
            public const string Comments = "💬";
            public const string ShareAlt = "🔗";
            public const string Link = "🔗";
            public const string LinkExternal = "🔗↗";
            
            // ===== WEATHER & NATURE =====
            public const string Sun = "☀️";
            public const string Moon = "🌙";
            public const string Cloud = "☁️";
            public const string CloudRain = "🌧️";
            public const string CloudSnow = "❄️";
            public const string Bolt = "⚡";
            public const string Thunder = "⚡";
            public const string Umbrella = "☂️";
            public const string Wind = "🌀";
            public const string Fire = "🔥";
            public const string Water = "💧";
            public const string Tree = "🌲";
            public const string Leaf = "🍃";
            
            // ===== LOCATIONS & PLACES =====
            public const string Location = "📍";
            public const string LocationOn = "📍";
            public const string LocationOff = "📌";
            public const string Map = "🗺️";
            public const string MapPin = "📍";
            public const string Navigation = "🧭";
            public const string Compass = "🧭";
            public const string Globe = "🌐";
            public const string World = "🌍";
            public const string Building = "🏢";
            public const string Store = "🏪";

            public const string Factory = "🏭";
            public const string Supplier = "🚚";
            public const string Customer = "👥";
            public const string Check = "📋";
            
            // ===== ARROWS =====
            public const string ArrowUp = "↑";
            public const string ArrowDown = "↓";
            public const string ArrowLeft = "←";
            public const string ArrowRight = "→";
            public const string ArrowUpRight = "↗";
            public const string ArrowUpLeft = "↖";
            public const string ArrowDownRight = "↘";
            public const string ArrowDownLeft = "↙";
            public const string ArrowCircleUp = "⇧";
            public const string ArrowCircleDown = "⇩";
            public const string ArrowCircleLeft = "⇦";
            public const string ArrowCircleRight = "⇨";
            
            // ===== SHAPES & SYMBOLS =====
            public const string Circle = "○";
            public const string CircleFilled = "●";
            public const string Square = "□";
            public const string SquareFilled = "■";
            public const string Triangle = "△";
            public const string TriangleFilled = "▲";
            public const string Diamond = "◇";
            public const string DiamondFilled = "◆";
            public const string Plus = "＋";
            public const string Minus = "－";
            public const string Multiply = "×";
            public const string Divide = "÷";
            public const string Equal = "＝";
            public const string NotEqual = "≠";
            public const string Percent = "％";
            public const string Infinity = "∞";
            
            // ===== MISC =====
            public const string Database = "🗄️";
            public const string Server = "🖥️";
            public const string Desktop = "💻";
            public const string Laptop = "💻";
            public const string Mobile = "📱";
            public const string Tablet = "📱";
            public const string Keyboard = "⌨️";
            public const string Mouse = "🖱️";
            public const string Wifi = "📡";
            public const string WifiOff = "📡✕";
            public const string Bluetooth = "🔵";
            public const string Battery = "🔋";
            public const string BatteryLow = "🔋";
            public const string Power = "⏻";
            public const string PowerOff = "⏼";
            public const string Plug = "🔌";
            public const string Bug = "🐛";
            public const string Code = "💻";
            public const string Terminal = "💻";
            public const string Cpu = "⚙️";
            public const string Memory = "🧠";
            public const string Flag = "🚩";
            public const string FlagAlt = "⚑";
            public const string Award = "🏆";
            public const string Trophy = "🏆";
            public const string Medal = "🏅";
            public const string Gift = "🎁";
            public const string Rocket = "🚀";
            public const string Plane = "✈️";
            public const string Car = "🚗";
            public const string Truck = "🚚";
            public const string Box = "📦";
            public const string Palette = "🎨";
            public const string Brush = "🖌️";
            public const string Pencil = "✏️";
            public const string Eraser = "🧹";
            public const string Warehouse = "🏠";
        }
    }
}
