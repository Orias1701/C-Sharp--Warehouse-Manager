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

            // ========== BUTTONS ==========
            yPosition = AddSection(scrollContent, yPosition, "BUTTONS - 5 kiểu Button", CreateButtonsSection());
            yPosition += sectionSpacing;

            // ========== INPUTS ==========
            yPosition = AddSection(scrollContent, yPosition, "INPUTS - TextBox & ComboBox", CreateInputsSection());
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
                // Tạo container panel để vẽ màu sắc
                Panel colorBox = new Panel
                {
                    Width = 80,
                    Height = 40,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5, 0, 5, 0)
                };
                
                // Set màu và disable paint để giữ màu
                colorBox.BackColor = color;
                colorBox.Paint += (s, e) =>
                {
                    // Vẽ lại màu để đảm bảo hiển thị đúng
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(brush, colorBox.ClientRectangle);
                    }
                };

                Label lblColorName = new Label
                {
                    Text = name,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font(UIConstants.Fonts.FontFamily, UIConstants.Fonts.XXSmall),
                    ForeColor = GetContrastColor(color),
                    BackColor = Color.Transparent
                };
                colorBox.Controls.Add(lblColorName);

                row.Controls.Add(colorBox);
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
