using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Commons
{
    /// <summary>
    /// ToolsBar component - Thanh công cụ chứa các nút chức năng
    /// </summary>
    public class ToolsBar : CustomPanel
    {
        public CustomTextBox TxtSearch { get; private set; }
        public CustomButton BtnSearch { get; private set; }
        public CustomButton BtnAddRecord { get; private set; }
        public CustomButton BtnImport { get; private set; }
        public CustomButton BtnExport { get; private set; }
        public CustomButton BtnUndo { get; private set; }
        public CustomButton BtnSave { get; private set; }
        public CustomButton BtnReport { get; private set; }
        public Label LblChangeStatus { get; private set; }

        public ToolsBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Panel configuration
            BackColor = ThemeManager.Instance.BackgroundLight;
            ShowBorder = true;
            BorderRadius = UIConstants.Borders.RadiusMedium;
            Padding = new Padding(UIConstants.Spacing.Padding.Medium);

            int topOffset = 15;
            int spacing = 8;
            int currentX = 15;

            // Search section (Left)
            TxtSearch = new CustomTextBox
            {
                Placeholder = $"{UIConstants.Icons.Search} Tìm kiếm...",
                Left = currentX,
                Top = topOffset,
                Width = 250
            };
            currentX += 250 + spacing;

            BtnSearch = new CustomButton
            {
                Text = UIConstants.Icons.Search,
                Left = currentX,
                Top = topOffset,
                Width = 50,
                ButtonStyleType = ButtonStyle.Outlined
            };
            currentX += 50 + spacing * 3;

            // Middle section buttons
            BtnAddRecord = new CustomButton
            {
                Text = $"{UIConstants.Icons.Add} Thêm",
                Left = currentX,
                Top = topOffset,
                Width = 100,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            currentX += 100 + spacing;

            BtnImport = new CustomButton
            {
                Text = $"{UIConstants.Icons.Import} Nhập",
                Left = currentX,
                Top = topOffset,
                Width = 95,
                ButtonStyleType = ButtonStyle.Outlined
            };
            currentX += 95 + spacing;

            BtnExport = new CustomButton
            {
                Text = $"{UIConstants.Icons.Export} Xuất",
                Left = currentX,
                Top = topOffset,
                Width = 95,
                ButtonStyleType = ButtonStyle.Outlined
            };
            currentX += 95 + spacing;

            BtnUndo = new CustomButton
            {
                Text = $"{UIConstants.Icons.Undo} Hoàn tác",
                Left = currentX,
                Top = topOffset,
                Width = 110,
                ButtonStyleType = ButtonStyle.Text
            };
            currentX += 110 + spacing;

            BtnSave = new CustomButton
            {
                Text = $"{UIConstants.Icons.Save} Lưu",
                Left = currentX,
                Top = topOffset,
                Width = 90,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            currentX += 90 + spacing;

            BtnReport = new CustomButton
            {
                Text = $"{UIConstants.Icons.Report} Báo cáo",
                Left = currentX,
                Top = topOffset,
                Width = 110,
                ButtonStyleType = ButtonStyle.Outlined
            };
            currentX += 110 + spacing * 3;

            // Status label (Right)
            LblChangeStatus = new Label
            {
                Text = "",
                Left = currentX,
                Top = topOffset + 8,
                Width = 200,
                Height = 25,
                ForeColor = UIConstants.SemanticColors.Warning,
                Font = ThemeManager.Instance.FontBold
            };

            Controls.AddRange(new Control[]
            {
                TxtSearch, BtnSearch, BtnAddRecord, BtnImport, BtnExport,
                BtnUndo, BtnSave, BtnReport, LblChangeStatus
            });
        }
    }
}
