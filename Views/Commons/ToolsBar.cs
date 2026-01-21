using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;
using FontAwesome.Sharp;

namespace WarehouseManagement.Views.Commons
{
    /// <summary>
    /// ToolsBar component - Thanh công cụ chứa các nút chức năng với logic xử lý
    /// </summary>
    public class ToolsBar : CustomPanel
    {
        public CustomTextBox TxtSearch { get; private set; }
        public CustomButton BtnAdd { get; private set; }

        public CustomButton BtnTransaction { get; private set; }
        public CustomButton BtnInventory { get; private set; }
        public CustomButton BtnUndo { get; private set; }
        public CustomButton BtnSave { get; private set; }
        public CustomButton BtnReport { get; private set; }
        public Label LblChangeStatus { get; private set; }

        // Events
        public event EventHandler SearchRequested;
        public event EventHandler AddRequested;

        public event EventHandler TransactionRequested;
        public event EventHandler InventoryRequested;
        public event EventHandler UndoRequested;
        public event EventHandler SaveRequested;
        public event EventHandler ReportRequested;

        public ToolsBar()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            // Panel configuration
            BackColor = ThemeManager.Instance.BackgroundLight;
            ShowBorder = false;
            HasShadow = true;
            ShadowSize = 5;
            BorderRadius = UIConstants.Borders.RadiusMedium;
            Padding = new Padding(UIConstants.Spacing.Padding.Large);

            int topOffset = 20;
            int spacing = 8;
            int currentX = 20;

            // Search section (Left)
            TxtSearch = new CustomTextBox
            {
                Placeholder = $"{UIConstants.Icons.Radio} Tìm kiếm...",
                Left = currentX,
                Top = topOffset,
                Width = 250
            };
            currentX += 250 + spacing * 3;

            // Middle section buttons
            BtnAdd = new CustomButton
            {
                Text = $"{UIConstants.Icons.Add} Thêm",
                Left = currentX,
                Top = topOffset,
                Width = 110,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            currentX += 110 + spacing;

            BtnTransaction = new CustomButton
            {
                Text = $"{UIConstants.Icons.Transaction} Giao dịch",
                Left = currentX,
                Top = topOffset,
                Width = 135,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            currentX += 135 + spacing;

            BtnInventory = new CustomButton
            {
                Text = $"{UIConstants.Icons.Check} Kiểm kê",
                Left = currentX,
                Top = topOffset,
                Width = 120,
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            currentX += 120 + spacing;

            BtnUndo = new CustomButton
            {
                Text = $"{UIConstants.Icons.Undo} Undo",
                Left = currentX,
                Top = topOffset,
                Width = 110,
                ButtonStyleType = ButtonStyle.Outlined
            };
            currentX += 110 + spacing;

            BtnSave = new CustomButton
            {
                Text = $"{UIConstants.Icons.Save} Save",
                Left = currentX,
                Top = topOffset,
                Width = 110,
                ButtonStyleType = ButtonStyle.Outlined
            };
            currentX += 110 + spacing;

            BtnReport = new CustomButton
            {
                Text = $"{UIConstants.Icons.Report} Report",
                Left = currentX,
                Top = topOffset,
                Width = 120,
                ButtonStyleType = ButtonStyle.Outlined
            };
            currentX += 120 + spacing * 3;

            // Status label (Right)
            LblChangeStatus = new Label
            {
                Text = "",
                Left = currentX,
                Top = topOffset + 8,
                Width = 300,
                Height = 25,
                ForeColor = UIConstants.SemanticColors.Warning,
                Font = ThemeManager.Instance.FontBold
            };

            Controls.AddRange(new Control[]
            {
                TxtSearch, BtnAdd, BtnTransaction, BtnInventory,
                BtnUndo, BtnSave, BtnReport, LblChangeStatus
            });
        }

        private void SetupEventHandlers()
        {
            TxtSearch.TextChanged += (s, e) => SearchRequested?.Invoke(this, EventArgs.Empty);
            BtnAdd.Click += (s, e) => AddRequested?.Invoke(this, EventArgs.Empty);
            BtnTransaction.Click += (s, e) => TransactionRequested?.Invoke(this, EventArgs.Empty);
            BtnInventory.Click += (s, e) => InventoryRequested?.Invoke(this, EventArgs.Empty);
            BtnUndo.Click += (s, e) => UndoRequested?.Invoke(this, EventArgs.Empty);
            BtnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
            BtnReport.Click += (s, e) => ReportRequested?.Invoke(this, EventArgs.Empty);
        }

        public void ResetSearch()
        {
            // Clear text và force hiển thị placeholder
            TxtSearch.Clear();
        }

        public string GetSearchText()
        {
            string text = TxtSearch.Text;
            string placeholder = TxtSearch.Placeholder;
            
            // Check if text is empty or is the placeholder
            if (string.IsNullOrWhiteSpace(text) || text == placeholder)
            {
                return "";
            }
            return text;
        }
    }
}
