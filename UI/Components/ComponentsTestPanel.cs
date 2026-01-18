using System;
using System.Drawing;
using System.Windows.Forms;

namespace WarehouseManagement.UI.Components
{
    /// <summary>
    /// Panel để test và preview tất cả các components
    /// </summary>
    public class ComponentsTestPanel : CustomPanel
    {
        private Panel scrollContent;

        public ComponentsTestPanel()
        {
            Dock = DockStyle.Fill;
            AutoScroll = true;
            ShowBorder = false;
            
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            SuspendLayout();

            // Container cho nội dung có thể scroll
            scrollContent = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(20, 20),
                Width = 1200
            };

            int yPosition = 0;
            int sectionSpacing = 40;

            // ========== COLORS ==========
            yPosition = AddSection(scrollContent, yPosition, "COLORS - Màu sắc", CreateColorsSection());
            yPosition += sectionSpacing;

            // ========== FONTS ==========
            yPosition = AddSection(scrollContent, yPosition, "FONTS - Font chữ", CreateFontsSection());
            yPosition += sectionSpacing;

            // ========== ICONS ==========
            yPosition = AddSection(scrollContent, yPosition, "ICONS - Bộ biểu tượng", CreateIconsSection());
            yPosition += sectionSpacing;

            // ========== BUTTONS ==========
            yPosition = AddSection(scrollContent, yPosition, "BUTTONS - 5 kiểu Button", CreateButtonsSection());
            yPosition += sectionSpacing;

            // ========== INPUTS ==========
            yPosition = AddSection(scrollContent, yPosition, "INPUTS - TextBox & ComboBox", CreateInputsSection());
            yPosition += sectionSpacing;

            // ========== DATE TIME PICKER ==========
            yPosition = AddSection(scrollContent, yPosition, "DATE TIME PICKER - Chọn ngày giờ", CreateDateTimePickerSection());
            yPosition += sectionSpacing;

            // ========== TEXT AREA ==========
            yPosition = AddSection(scrollContent, yPosition, "TEXT AREA - Vùng văn bản", CreateTextAreaSection());
            yPosition += sectionSpacing;

            // ========== PANELS ==========
            yPosition = AddSection(scrollContent, yPosition, "PANELS - Custom Panel", CreatePanelsSection());
            yPosition += sectionSpacing;

            // ========== SPACING ==========
            yPosition = AddSection(scrollContent, yPosition, "SPACING - Khoảng cách", CreateSpacingSection());
            
            Controls.Add(scrollContent);
            ResumeLayout(false);
        }

        private int AddSection(Panel container, int yPos, string title, Control content)
        {
            // Title
            Label lblTitle = new Label
            {
                Text = title,
                Font = ThemeManager.Instance.FontBoldLarge,
                ForeColor = ThemeManager.Instance.PrimaryDefault,
                AutoSize = true,
                Location = new Point(0, yPos)
            };
            container.Controls.Add(lblTitle);
            yPos += 35;

            // Content
            content.Location = new Point(20, yPos);
            container.Controls.Add(content);
            yPos += content.Height;

            return yPos;
        }

        // ========== COLORS SECTION ==========
        private Control CreateColorsSection()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };

            // Primary Colors
            panel.Controls.Add(CreateColorRow("Primary Colors", new[]
            {
                ("Default", UIConstants.PrimaryColor.Default),
                ("Active", UIConstants.PrimaryColor.Active),
                ("Hover", UIConstants.PrimaryColor.Hover),
                ("Pressed", UIConstants.PrimaryColor.Pressed),
                ("Disabled", UIConstants.PrimaryColor.Disabled),
                ("Light", UIConstants.PrimaryColor.Light)
            }));

            // Background Light
            panel.Controls.Add(CreateColorRow("Background Light", new[]
            {
                ("Default", UIConstants.BackgroundLight.Default),
                ("Lighter", UIConstants.BackgroundLight.Lighter),
                ("Light", UIConstants.BackgroundLight.Light),
                ("Medium", UIConstants.BackgroundLight.Medium),
                ("Dark", UIConstants.BackgroundLight.Dark),
                ("Darker", UIConstants.BackgroundLight.Darker)
            }));

            // Background Dark
            panel.Controls.Add(CreateColorRow("Background Dark", new[]
            {
                ("Default", UIConstants.BackgroundDark.Default),
                ("Lighter", UIConstants.BackgroundDark.Lighter),
                ("Light", UIConstants.BackgroundDark.Light),
                ("Medium", UIConstants.BackgroundDark.Medium),
                ("Dark", UIConstants.BackgroundDark.Dark),
                ("Darker", UIConstants.BackgroundDark.Darker)
            }));

            // Semantic Colors
            panel.Controls.Add(CreateColorRow("Semantic Colors", new[]
            {
                ("Success", UIConstants.SemanticColors.Success),
                ("Warning", UIConstants.SemanticColors.Warning),
                ("Error", UIConstants.SemanticColors.Error),
                ("Info", UIConstants.SemanticColors.Info)
            }));

            return panel;
        }

        private Control CreateColorRow(string rowName, (string name, Color color)[] colors)
        {
            FlowLayoutPanel row = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 5, 0, 5)
            };

            Label lblName = new Label
            {
                Text = rowName + ":",
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = ThemeManager.Instance.FontRegular
            };
            row.Controls.Add(lblName);

            foreach (var (name, color) in colors)
            {
                // Container cho mỗi color item (box + label dưới)
                Panel colorItem = new Panel
                {
                    Width = 90,
                    Height = 65,
                    Margin = new Padding(5, 0, 5, 0)
                };

                // Color box - hiển thị màu thuần
                Panel colorBox = new Panel
                {
                    Width = 90,
                    Height = 45,
                    BackColor = color,
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(0, 0)
                };
                
                // Override Paint để đảm bảo màu hiển thị đúng
                Color capturedColor = color; // Capture biến trong closure
                colorBox.Paint += (s, e) =>
                {
                    using (SolidBrush brush = new SolidBrush(capturedColor))
                    {
                        e.Graphics.FillRectangle(brush, colorBox.ClientRectangle);
                    }
                };

                // Label tên màu - hiển thị bên dưới
                Label lblColorName = new Label
                {
                    Text = name,
                    Width = 90,
                    Height = 18,
                    Location = new Point(0, 47),
                    TextAlign = ContentAlignment.TopCenter,
                    Font = new Font(UIConstants.Fonts.FontFamily, UIConstants.Fonts.XXSmall),
                    ForeColor = ThemeManager.Instance.TextSecondary
                };

                colorItem.Controls.Add(colorBox);
                colorItem.Controls.Add(lblColorName);
                row.Controls.Add(colorItem);
            }

            return row;
        }

        private Color GetContrastColor(Color bgColor)
        {
            int brightness = (bgColor.R + bgColor.G + bgColor.B) / 3;
            return brightness > 128 ? Color.Black : Color.White;
        }

        // ========== FONTS SECTION ==========
        private Control CreateFontsSection()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };

            var fonts = new[]
            {
                ("XXSmall - 9px", ThemeManager.Instance.FontXXSmall),
                ("XSmall - 10px", ThemeManager.Instance.FontXSmall),
                ("Small - 11px", ThemeManager.Instance.FontSmall),
                ("Regular - 12px", ThemeManager.Instance.FontRegular),
                ("Medium - 14px", ThemeManager.Instance.FontMedium),
                ("Large - 16px", ThemeManager.Instance.FontLarge),
                ("XLarge - 20px", ThemeManager.Instance.FontXLarge),
                ("XXLarge - 24px", ThemeManager.Instance.FontXXLarge)
            };

            foreach (var (name, font) in fonts)
            {
                Label lbl = new Label
                {
                    Text = $"{name} - The quick brown fox jumps over the lazy dog",
                    Font = font,
                    AutoSize = true,
                    Margin = new Padding(0, 5, 0, 5)
                };
                panel.Controls.Add(lbl);
            }

            return panel;
        }

        // ========== ICONS SECTION ==========
        private Control CreateIconsSection()
        {
            FlowLayoutPanel mainPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Width = 1150
            };

            // Helper method để tạo icon category
            void AddIconCategory(string categoryName, (string name, string icon)[] icons)
            {
                // Category label
                Label lblCategory = new Label
                {
                    Text = categoryName,
                    Font = ThemeManager.Instance.FontBold,
                    ForeColor = ThemeManager.Instance.PrimaryDefault,
                    AutoSize = true,
                    Margin = new Padding(0, 15, 0, 10)
                };
                mainPanel.Controls.Add(lblCategory);

                // Icons grid
                FlowLayoutPanel iconsGrid = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    WrapContents = true,
                    Width = 1150,
                    Margin = new Padding(0, 0, 0, 10)
                };

                foreach (var (name, icon) in icons)
                {
                    Panel iconItem = new Panel
                    {
                        Width = 110,
                        Height = 80,
                        Margin = new Padding(5)
                    };

                    // Icon display
                    Label lblIcon = new Label
                    {
                        Text = icon,
                        Width = 110,
                        Height = 45,
                        Location = new Point(0, 0),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font(UIConstants.Fonts.FontFamily, 24),
                        Cursor = Cursors.Hand
                    };

                    // Tooltip on hover
                    ToolTip tooltip = new ToolTip();
                    tooltip.SetToolTip(lblIcon, $"Click to copy: {name}");

                    // Click to copy
                    lblIcon.Click += (s, e) =>
                    {
                        try
                        {
                            Clipboard.SetText(icon);
                            tooltip.SetToolTip(lblIcon, $"Copied: {icon}");
                            System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
                            {
                                if (lblIcon.IsHandleCreated)
                                {
                                    lblIcon.Invoke((MethodInvoker)(() =>
                                    {
                                        tooltip.SetToolTip(lblIcon, $"Click to copy: {name}");
                                    }));
                                }
                            });
                        }
                        catch { }
                    };

                    // Icon name
                    Label lblName = new Label
                    {
                        Text = name,
                        Width = 110,
                        Height = 35,
                        Location = new Point(0, 45),
                        TextAlign = ContentAlignment.TopCenter,
                        Font = new Font(UIConstants.Fonts.FontFamily, UIConstants.Fonts.XXSmall),
                        ForeColor = ThemeManager.Instance.TextSecondary
                    };

                    iconItem.Controls.Add(lblIcon);
                    iconItem.Controls.Add(lblName);
                    iconsGrid.Controls.Add(iconItem);
                }

                mainPanel.Controls.Add(iconsGrid);
            }

            // ===== NAVIGATION ICONS =====
            AddIconCategory("Navigation", new[]
            {
                ("Home", UIConstants.Icons.Home),
                ("Menu", UIConstants.Icons.Menu),
                ("Menu Dots", UIConstants.Icons.MenuDots),
                ("Menu Dots H", UIConstants.Icons.MenuDotsHorizontal),
                ("Back", UIConstants.Icons.Back),
                ("Forward", UIConstants.Icons.Forward),
                ("Up", UIConstants.Icons.Up),
                ("Down", UIConstants.Icons.Down),
                ("Close", UIConstants.Icons.Close),
                ("Minimize", UIConstants.Icons.Minimize),
                ("Maximize", UIConstants.Icons.Maximize),
                ("Fullscreen", UIConstants.Icons.Fullscreen)
            });

            // ===== ACTION ICONS =====
            AddIconCategory("Actions", new[]
            {
                ("Add", UIConstants.Icons.Add),
                ("Remove", UIConstants.Icons.Remove),
                ("Edit", UIConstants.Icons.Edit),
                ("Delete", UIConstants.Icons.Delete),
                ("Save", UIConstants.Icons.Save),
                ("Cancel", UIConstants.Icons.Cancel),
                ("Refresh", UIConstants.Icons.Refresh),
                ("Search", UIConstants.Icons.Search),
                ("Filter", UIConstants.Icons.Filter),
                ("Sort", UIConstants.Icons.Sort),
                ("Copy", UIConstants.Icons.Copy),
                ("Cut", UIConstants.Icons.Cut),
                ("Paste", UIConstants.Icons.Paste),
                ("Undo", UIConstants.Icons.Undo),
                ("Redo", UIConstants.Icons.Redo),
                ("Print", UIConstants.Icons.Print),
                ("Download", UIConstants.Icons.Download),
                ("Upload", UIConstants.Icons.Upload),
                ("Import", UIConstants.Icons.Import),
                ("Export", UIConstants.Icons.Export),
                ("Share", UIConstants.Icons.Share),
                ("Send", UIConstants.Icons.Send),
                ("Pin", UIConstants.Icons.Pin)
            });

            // ===== STATUS ICONS =====
            AddIconCategory("Status & Alerts", new[]
            {
                ("Success", UIConstants.Icons.Success),
                ("Error", UIConstants.Icons.Error),
                ("Warning", UIConstants.Icons.Warning),
                ("Info", UIConstants.Icons.Info),
                ("Help", UIConstants.Icons.Help),
                ("Question", UIConstants.Icons.Question),
                ("Exclamation", UIConstants.Icons.Exclamation),
                ("Loading", UIConstants.Icons.Loading),
                ("Done", UIConstants.Icons.Done),
                ("Pending", UIConstants.Icons.Pending),
                ("Block", UIConstants.Icons.Block)
            });

            // ===== FILES & FOLDERS =====
            AddIconCategory("Files & Folders", new[]
            {
                ("File", UIConstants.Icons.File),
                ("File Text", UIConstants.Icons.FileText),
                ("File Image", UIConstants.Icons.FileImage),
                ("File Video", UIConstants.Icons.FileVideo),
                ("File Audio", UIConstants.Icons.FileAudio),
                ("File Code", UIConstants.Icons.FileCode),
                ("File PDF", UIConstants.Icons.FilePdf),
                ("Folder", UIConstants.Icons.Folder),
                ("Folder Open", UIConstants.Icons.FolderOpen),
                ("Archive", UIConstants.Icons.Archive),
                ("Document", UIConstants.Icons.Document)
            });

            // ===== COMMUNICATION =====
            AddIconCategory("Communication", new[]
            {
                ("Mail", UIConstants.Icons.Mail),
                ("Mail Open", UIConstants.Icons.MailOpen),
                ("Message", UIConstants.Icons.Message),
                ("Chat", UIConstants.Icons.Chat),
                ("Phone", UIConstants.Icons.Phone),
                ("Phone Call", UIConstants.Icons.PhoneCall),
                ("Notification", UIConstants.Icons.Notification),
                ("Alert", UIConstants.Icons.Alert),
                ("Inbox", UIConstants.Icons.Inbox)
            });

            // ===== MEDIA =====
            AddIconCategory("Media & Playback", new[]
            {
                ("Play", UIConstants.Icons.Play),
                ("Pause", UIConstants.Icons.Pause),
                ("Stop", UIConstants.Icons.Stop),
                ("Record", UIConstants.Icons.Record),
                ("Volume", UIConstants.Icons.Volume),
                ("Volume Mute", UIConstants.Icons.VolumeMute),
                ("Camera", UIConstants.Icons.Camera),
                ("Video", UIConstants.Icons.Video),
                ("Microphone", UIConstants.Icons.Microphone),
                ("Image", UIConstants.Icons.Image)
            });

            // ===== BUSINESS =====
            AddIconCategory("Business & Commerce", new[]
            {
                ("Product", UIConstants.Icons.Product),
                ("Transaction", UIConstants.Icons.Transaction),
                ("Money", UIConstants.Icons.Money),
                ("Dollar", UIConstants.Icons.Dollar),
                ("Credit Card", UIConstants.Icons.CreditCard),
                ("Cart", UIConstants.Icons.Cart),
                ("Bag", UIConstants.Icons.Bag),
                ("Tag", UIConstants.Icons.Tag),
                ("Barcode", UIConstants.Icons.Barcode),
                ("Receipt", UIConstants.Icons.Receipt),
                ("Report", UIConstants.Icons.Report),
                ("Chart", UIConstants.Icons.Chart),
                ("Analytics", UIConstants.Icons.Analytics),
                ("Trending Up", UIConstants.Icons.TrendingUp)
            });

            // ===== USER & ACCOUNT =====
            AddIconCategory("User & Account", new[]
            {
                ("User", UIConstants.Icons.User),
                ("Users", UIConstants.Icons.Users),
                ("Profile", UIConstants.Icons.Profile),
                ("Login", UIConstants.Icons.Login),
                ("Logout", UIConstants.Icons.Logout),
                ("Lock", UIConstants.Icons.Lock),
                ("Unlock", UIConstants.Icons.Unlock),
                ("Key", UIConstants.Icons.Key),
                ("Password", UIConstants.Icons.Password),
                ("Shield", UIConstants.Icons.Shield)
            });

            // ===== VIEWS & LAYOUT =====
            AddIconCategory("Views & Layout", new[]
            {
                ("List", UIConstants.Icons.List),
                ("Grid", UIConstants.Icons.Grid),
                ("Table", UIConstants.Icons.Table),
                ("Dashboard", UIConstants.Icons.Dashboard),
                ("Window", UIConstants.Icons.Window),
                ("Layout", UIConstants.Icons.Layout)
            });

            // ===== UI CONTROLS =====
            AddIconCategory("UI Controls", new[]
            {
                ("Settings", UIConstants.Icons.Settings),
                ("Tools", UIConstants.Icons.Tools),
                ("Sliders", UIConstants.Icons.Sliders),
                ("Checkbox", UIConstants.Icons.Checkbox),
                ("Dropdown", UIConstants.Icons.Dropdown),
                ("Expand More", UIConstants.Icons.ExpandMore),
                ("Expand Less", UIConstants.Icons.ExpandLess)
            });

            // ===== TIME =====
            AddIconCategory("Time & Calendar", new[]
            {
                ("Calendar", UIConstants.Icons.Calendar),
                ("Clock", UIConstants.Icons.Clock),
                ("Timer", UIConstants.Icons.Timer),
                ("Hourglass", UIConstants.Icons.Hourglass),
                ("Today", UIConstants.Icons.Today)
            });

            // ===== VISIBILITY =====
            AddIconCategory("Visibility", new[]
            {
                ("Eye", UIConstants.Icons.Eye),
                ("Eye Off", UIConstants.Icons.EyeOff),
                ("Show", UIConstants.Icons.Show),
                ("Hide", UIConstants.Icons.Hide)
            });

            // ===== SOCIAL =====
            AddIconCategory("Social & Interaction", new[]
            {
                ("Like", UIConstants.Icons.Like),
                ("Dislike", UIConstants.Icons.Dislike),
                ("Heart", UIConstants.Icons.Heart),
                ("Star", UIConstants.Icons.Star),
                ("Bookmark", UIConstants.Icons.Bookmark),
                ("Comment", UIConstants.Icons.Comment),
                ("Link", UIConstants.Icons.Link)
            });

            // ===== WEATHER =====
            AddIconCategory("Weather & Nature", new[]
            {
                ("Sun", UIConstants.Icons.Sun),
                ("Moon", UIConstants.Icons.Moon),
                ("Cloud", UIConstants.Icons.Cloud),
                ("Bolt", UIConstants.Icons.Bolt),
                ("Fire", UIConstants.Icons.Fire),
                ("Water", UIConstants.Icons.Water),
                ("Tree", UIConstants.Icons.Tree)
            });

            // ===== LOCATION =====
            AddIconCategory("Location & Places", new[]
            {
                ("Location", UIConstants.Icons.Location),
                ("Map", UIConstants.Icons.Map),
                ("Navigation", UIConstants.Icons.Navigation),
                ("Globe", UIConstants.Icons.Globe),
                ("Building", UIConstants.Icons.Building),
                ("Store", UIConstants.Icons.Store),
                ("Warehouse", UIConstants.Icons.Warehouse)
            });

            // ===== ARROWS =====
            AddIconCategory("Arrows", new[]
            {
                ("Arrow Up", UIConstants.Icons.ArrowUp),
                ("Arrow Down", UIConstants.Icons.ArrowDown),
                ("Arrow Left", UIConstants.Icons.ArrowLeft),
                ("Arrow Right", UIConstants.Icons.ArrowRight),
                ("Arrow Up Right", UIConstants.Icons.ArrowUpRight),
                ("Arrow Down Left", UIConstants.Icons.ArrowDownLeft)
            });

            // ===== SHAPES =====
            AddIconCategory("Shapes & Symbols", new[]
            {
                ("Circle", UIConstants.Icons.Circle),
                ("Circle Filled", UIConstants.Icons.CircleFilled),
                ("Square", UIConstants.Icons.Square),
                ("Square Filled", UIConstants.Icons.SquareFilled),
                ("Triangle", UIConstants.Icons.Triangle),
                ("Diamond", UIConstants.Icons.Diamond),
                ("Plus", UIConstants.Icons.Plus),
                ("Minus", UIConstants.Icons.Minus)
            });

            // ===== MISC =====
            AddIconCategory("Miscellaneous", new[]
            {
                ("Database", UIConstants.Icons.Database),
                ("Server", UIConstants.Icons.Server),
                ("Desktop", UIConstants.Icons.Desktop),
                ("Mobile", UIConstants.Icons.Mobile),
                ("Wifi", UIConstants.Icons.Wifi),
                ("Battery", UIConstants.Icons.Battery),
                ("Power", UIConstants.Icons.Power),
                ("Bug", UIConstants.Icons.Bug),
                ("Code", UIConstants.Icons.Code),
                ("Flag", UIConstants.Icons.Flag),
                ("Award", UIConstants.Icons.Award),
                ("Gift", UIConstants.Icons.Gift),
                ("Rocket", UIConstants.Icons.Rocket),
                ("Truck", UIConstants.Icons.Truck),
                ("Palette", UIConstants.Icons.Palette),
                ("Brush", UIConstants.Icons.Brush)
            });

            return mainPanel;
        }

        // ========== BUTTONS SECTION ==========
        private Control CreateButtonsSection()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };

            var buttonStyles = new[]
            {
                (ButtonStyle.Outlined, "Outlined - Nền BG, viền Primary"),
                (ButtonStyle.Filled, "Filled - Nền Primary, viền BG"),
                (ButtonStyle.Text, "Text - Nền BG, viền Transparent"),
                (ButtonStyle.FilledNoOutline, "Filled No Outline - Nền Primary, viền Transparent"),
                (ButtonStyle.Ghost, "Ghost - Nền và viền Transparent")
            };

            foreach (var (style, description) in buttonStyles)
            {
                FlowLayoutPanel row = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Margin = new Padding(0, 5, 0, 5)
                };

                Label lbl = new Label
                {
                    Text = description,
                    Width = 350,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                row.Controls.Add(lbl);

                CustomButton btnNormal = new CustomButton
                {
                    Text = "Normal",
                    ButtonStyleType = style,
                    Width = 120
                };
                row.Controls.Add(btnNormal);

                CustomButton btnDisabled = new CustomButton
                {
                    Text = "Disabled",
                    ButtonStyleType = style,
                    Width = 120,
                    Enabled = false
                };
                row.Controls.Add(btnDisabled);

                panel.Controls.Add(row);
            }

            return panel;
        }

        // ========== INPUTS SECTION ==========
        private Control CreateInputsSection()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };

            // TextBox
            FlowLayoutPanel row1 = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 5, 0, 5)
            };

            Label lbl1 = new Label
            {
                Text = "CustomTextBox:",
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };
            row1.Controls.Add(lbl1);

            CustomTextBox txt1 = new CustomTextBox
            {
                Width = 250,
                Placeholder = "Nhập văn bản..."
            };
            row1.Controls.Add(txt1);

            CustomTextBox txt2 = new CustomTextBox
            {
                Width = 250,
                Text = "Có giá trị"
            };
            row1.Controls.Add(txt2);

            panel.Controls.Add(row1);

            // ComboBox
            FlowLayoutPanel row2 = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 5, 0, 5)
            };

            Label lbl2 = new Label
            {
                Text = "CustomComboBox:",
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };
            row2.Controls.Add(lbl2);

            CustomComboBox combo = new CustomComboBox
            {
                Width = 250
            };
            combo.Items.AddRange(new[] { "Tùy chọn 1", "Tùy chọn 2", "Tùy chọn 3" });
            combo.SelectedIndex = 0;
            row2.Controls.Add(combo);

            panel.Controls.Add(row2);

            return panel;
        }

        // ========== DATE TIME PICKER SECTION ==========
        private Control CreateDateTimePickerSection()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };

            // DateTimePicker - Date format
            FlowLayoutPanel row1 = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 5, 0, 5)
            };

            Label lbl1 = new Label
            {
                Text = "Date Format:",
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };
            row1.Controls.Add(lbl1);

            CustomDateTimePicker dtp1 = new CustomDateTimePicker
            {
                Width = 250,
                Value = DateTime.Now,
                CustomFormat = "dd/MM/yyyy"
            };
            row1.Controls.Add(dtp1);

            panel.Controls.Add(row1);

            // DateTimePicker - DateTime format
            FlowLayoutPanel row2 = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 5, 0, 5)
            };

            Label lbl2 = new Label
            {
                Text = "DateTime Format:",
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };
            row2.Controls.Add(lbl2);

            CustomDateTimePicker dtp2 = new CustomDateTimePicker
            {
                Width = 250,
                Value = DateTime.Now,
                CustomFormat = "dd/MM/yyyy HH:mm"
            };
            row2.Controls.Add(dtp2);

            panel.Controls.Add(row2);

            // DateTimePicker - Time format
            FlowLayoutPanel row3 = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 5, 0, 5)
            };

            Label lbl3 = new Label
            {
                Text = "Time Format:",
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };
            row3.Controls.Add(lbl3);

            CustomDateTimePicker dtp3 = new CustomDateTimePicker
            {
                Width = 250,
                Value = DateTime.Now,
                CustomFormat = "HH:mm:ss"
            };
            row3.Controls.Add(dtp3);

            panel.Controls.Add(row3);

            return panel;
        }

        // ========== TEXT AREA SECTION ==========
        private Control CreateTextAreaSection()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            CustomTextArea textArea = new CustomTextArea
            {
                Width = 400,
                Height = 120,
                Placeholder = "Nhập văn bản dài..."
            };
            panel.Controls.Add(textArea);

            CustomTextArea textArea2 = new CustomTextArea
            {
                Width = 400,
                Height = 120,
                Text = "Đây là một đoạn văn bản mẫu\nCó nhiều dòng\nĐể test TextArea component"
            };
            panel.Controls.Add(textArea2);

            return panel;
        }

        // ========== PANELS SECTION ==========
        private Control CreatePanelsSection()
        {
            FlowLayoutPanel container = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            var radiusValues = new[] { 0, 4, 8, 12, 16 };

            foreach (int radius in radiusValues)
            {
                CustomPanel testPanel = new CustomPanel
                {
                    Width = 150,
                    Height = 100,
                    BorderRadius = radius,
                    Margin = new Padding(10)
                };

                Label lbl = new Label
                {
                    Text = $"Radius: {radius}px",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                testPanel.Controls.Add(lbl);

                container.Controls.Add(testPanel);
            }

            return container;
        }

        // ========== SPACING SECTION ==========
        private Control CreateSpacingSection()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            var spacings = new[]
            {
                ("XXSmall", UIConstants.Spacing.Padding.XXSmall),
                ("XSmall", UIConstants.Spacing.Padding.XSmall),
                ("Small", UIConstants.Spacing.Padding.Small),
                ("Medium", UIConstants.Spacing.Padding.Medium),
                ("Large", UIConstants.Spacing.Padding.Large),
                ("XLarge", UIConstants.Spacing.Padding.XLarge),
                ("XXLarge", UIConstants.Spacing.Padding.XXLarge)
            };

            foreach (var (name, value) in spacings)
            {
                FlowLayoutPanel row = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Margin = new Padding(0, 5, 0, 5)
                };

                Label lbl = new Label
                {
                    Text = $"{name} ({value}px):",
                    Width = 150,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                row.Controls.Add(lbl);

                Panel box = new Panel
                {
                    Width = value * 10,
                    Height = 20,
                    BackColor = ThemeManager.Instance.PrimaryDefault,
                    BorderStyle = BorderStyle.FixedSingle
                };
                row.Controls.Add(box);

                panel.Controls.Add(row);
            }

            return panel;
        }
    }
}
