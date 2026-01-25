using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Services;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Forms
{
    /// <summary>
    /// Form báo cáo Nhập/Xuất theo ngày (30 ngày gần nhất)
    /// Kích thước cố định dựa trên A4 landscape (1400x900px)
    /// </summary>
    public class TransactionReportForm : Form
    {
        private PictureBox pictureBoxImport;
        private PictureBox pictureBoxExport;
        private DataGridView dgvReport;
        private ChartService chartService;
        private List<string> days;
        private List<decimal> imports;
        private List<decimal> exports;
        private decimal maxImport;
        private decimal maxExport;
        private CustomDateTimePicker dtpAnchorDate;
        private CustomButton btnExportReport;

        // Kích thước cố định dựa trên A4 landscape scaled 1.4x
        private const int FORM_WIDTH = 1400;
        private const int FORM_HEIGHT = 950;
        private const int MARGIN = 75;
        private const int BUTTON_HEIGHT = 55;
        private const int GAP = 20;
        
        // Tính toán kích thước nội dung
        private const int CONTENT_WIDTH = FORM_WIDTH - (MARGIN * 2); // 1250px - margin trái phải
        // CONTENT_HEIGHT = Total height - Button height - Margin below button - Margin at bottom
        private const int CONTENT_HEIGHT = FORM_HEIGHT - BUTTON_HEIGHT - MARGIN*2 + GAP*2; // 950 - 65 - 150 + 40 = 775px
        private const int TABLE_WIDTH = (int)(CONTENT_WIDTH * 0.34); // 34%
        private const int CHART_AREA_WIDTH = CONTENT_WIDTH - TABLE_WIDTH - GAP;
        private const int CHART_HEIGHT = (CONTENT_HEIGHT - GAP) / 2;

        public TransactionReportForm()
        {
            chartService = new ChartService();
            days = new List<string>();
            imports = new List<decimal>();
            exports = new List<decimal>();
            InitializeComponent();
            
            // Apply theme
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Form settings - FIXED SIZE based on A4 landscape
            Text = $"{UIConstants.Icons.Chart} Báo Cáo Nhập/Xuất";
            ClientSize = new Size(FORM_WIDTH, FORM_HEIGHT);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            BackColor = ThemeManager.Instance.BackgroundLight;

            // Filter/Button panel - styled panel at top
            Panel filterPanel = new Panel
            {
                Location = new Point(MARGIN, 20),
                Size = new Size(CONTENT_WIDTH, 50),
                BackColor = ThemeManager.Instance.BackgroundDefault
            };

            dtpAnchorDate = new CustomDateTimePicker
            {
                Location = new Point(0, 10),
                Size = new Size(200, UIConstants.Sizes.InputHeight),
                Value = DateTime.Now,
                CustomFormat = "dd/MM/yyyy",
                BorderRadius = UIConstants.Borders.RadiusMedium
            };
            dtpAnchorDate.ValueChanged += (s, e) => LoadReport();
            filterPanel.Controls.Add(dtpAnchorDate);

            btnExportReport = new CustomButton
            {
                Text = $"{UIConstants.Icons.Export} Xuất Báo Cáo",
                Location = new Point(CONTENT_WIDTH - 160, 10),
                Size = new Size(160, UIConstants.Sizes.ButtonHeight),
                ButtonStyleType = ButtonStyle.FilledNoOutline
            };
            btnExportReport.Click += BtnExportReport_Click;
            filterPanel.Controls.Add(btnExportReport);

            Controls.Add(filterPanel);

            // Content starts after filter panel
            int contentTop = 90;

            // Data table panel (left side)
            CustomPanel dataPanel = new CustomPanel
            {
                Location = new Point(MARGIN, contentTop),
                Size = new Size(TABLE_WIDTH, CONTENT_HEIGHT),
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = true,
                Padding = new Padding(UIConstants.Spacing.Padding.Medium)
            };

            dgvReport = new DataGridView
            {
                Location = new Point(UIConstants.Spacing.Padding.Medium, UIConstants.Spacing.Padding.Medium),
                Size = new Size(TABLE_WIDTH - (UIConstants.Spacing.Padding.Medium * 2), 
                               CONTENT_HEIGHT - (UIConstants.Spacing.Padding.Medium * 2)),
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = false,
                BackgroundColor = ThemeManager.Instance.BackgroundDefault,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = UIConstants.PrimaryColor.Default,
                    ForeColor = Color.White,
                    Font = ThemeManager.Instance.FontBold
                },
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToResizeRows = false,
                Font = ThemeManager.Instance.FontSmall,
                RowTemplate = { Height = 32 },
                ColumnHeadersHeight = 35
            };

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Ngày", 
                DataPropertyName = "Day",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(8, 2, 10, 4) }
            });
            
            dgvReport.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Nhập Kho", 
                DataPropertyName = "Import",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(8, 2, 10, 4)
                }
            });
            
            dgvReport.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                HeaderText = "Xuất Kho", 
                DataPropertyName = "Export",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(8, 2, 10, 4)
                }
            });

            // Sync header padding and alignment
            foreach (DataGridViewColumn col in dgvReport.Columns)
            {
                if (col.DefaultCellStyle.Alignment != DataGridViewContentAlignment.NotSet)
                {
                    col.HeaderCell.Style.Alignment = col.DefaultCellStyle.Alignment;
                }
                col.HeaderCell.Style.Padding = new Padding(8, 2, 10, 4);
            }
            
            // Apply theme
            dgvReport.DefaultCellStyle.BackColor = ThemeManager.Instance.BackgroundDefault;
            dgvReport.DefaultCellStyle.ForeColor = ThemeManager.Instance.TextPrimary;
            dgvReport.DefaultCellStyle.SelectionBackColor = UIConstants.PrimaryColor.Light;
            dgvReport.DefaultCellStyle.SelectionForeColor = ThemeManager.Instance.TextPrimary;
            dgvReport.ColumnHeadersDefaultCellStyle.SelectionBackColor = UIConstants.PrimaryColor.Default;
            dgvReport.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            
            dataPanel.Controls.Add(dgvReport);

            // Import chart panel (top right)
            int chartLeft = MARGIN + TABLE_WIDTH + GAP;
            CustomPanel importChartPanel = new CustomPanel
            {
                Location = new Point(chartLeft, contentTop),
                Size = new Size(CHART_AREA_WIDTH, CHART_HEIGHT),
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = true,
                Padding = new Padding(UIConstants.Spacing.Padding.Medium)
            };

            Label lblImportTitle = new Label
            {
                Text = $"{UIConstants.Icons.Import} Biểu Đồ Nhập Kho",
                Location = new Point(UIConstants.Spacing.Padding.Medium, UIConstants.Spacing.Padding.Medium),
                Size = new Size(200, 30),
                Font = ThemeManager.Instance.FontBold,
                ForeColor = Color.Green
            };
            importChartPanel.Controls.Add(lblImportTitle);

            pictureBoxImport = new PictureBox
            {
                Location = new Point(UIConstants.Spacing.Padding.Medium, UIConstants.Spacing.Padding.Medium + 30),
                Size = new Size(CHART_AREA_WIDTH - (UIConstants.Spacing.Padding.Medium * 2), 
                               CHART_HEIGHT - (UIConstants.Spacing.Padding.Medium * 2) - 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            importChartPanel.Controls.Add(pictureBoxImport);

            // Export chart panel (bottom right)
            CustomPanel exportChartPanel = new CustomPanel
            {
                Location = new Point(chartLeft, contentTop + CHART_HEIGHT + GAP),
                Size = new Size(CHART_AREA_WIDTH, CHART_HEIGHT),
                BorderRadius = UIConstants.Borders.RadiusLarge,
                ShowBorder = true,
                Padding = new Padding(UIConstants.Spacing.Padding.Medium)
            };

            Label lblExportTitle = new Label
            {
                Text = $"{UIConstants.Icons.Export} Biểu Đồ Xuất Kho",
                Location = new Point(UIConstants.Spacing.Padding.Medium, UIConstants.Spacing.Padding.Medium),
                Size = new Size(200, 30),
                Font = ThemeManager.Instance.FontBold,
                ForeColor = Color.Red
            };
            exportChartPanel.Controls.Add(lblExportTitle);

            pictureBoxExport = new PictureBox
            {
                Location = new Point(UIConstants.Spacing.Padding.Medium, UIConstants.Spacing.Padding.Medium + 30),
                Size = new Size(CHART_AREA_WIDTH - (UIConstants.Spacing.Padding.Medium * 2), 
                               CHART_HEIGHT - (UIConstants.Spacing.Padding.Medium * 2) - 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            exportChartPanel.Controls.Add(pictureBoxExport);

            // Add all to form (button controls already added above)
            Controls.Add(dataPanel);
            Controls.Add(importChartPanel);
            Controls.Add(exportChartPanel);

            Load += TransactionReportForm_Load;
            ResumeLayout(false);
        }

        private void TransactionReportForm_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void BtnExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx|PDF Document (*.pdf)|*.pdf",
                    DefaultExt = "xlsx",
                    FileName = $"BaoCaoNhapXuat_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveDialog.FileName.EndsWith(".pdf"))
                    {
                        ExportToPdf(saveDialog.FileName);
                    }
                    else
                    {
                        ExportToExcel(saveDialog.FileName);
                    }
                    MessageBox.Show($"{UIConstants.Icons.Success} Xuất báo cáo thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Auto open file
                    try
                    {
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                    catch { } // Ignore if cannot open
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi xuất báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel(string filePath)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("BaoCao");
                
                // --- REPORT HEADER ---
                var titleRange = worksheet.Range(1, 1, 1, 7);
                titleRange.Merge();
                titleRange.Value = "BÁO CÁO NHẬP XUẤT THEO NGÀY";
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.FontSize = 16;
                titleRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                titleRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromColor(UIConstants.PrimaryColor.Light);

                var dateRange = worksheet.Range(2, 1, 2, 7);
                dateRange.Merge();
                dateRange.Value = $"Ngày báo cáo: {dtpAnchorDate.Value:dd/MM/yyyy} - Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
                dateRange.Style.Font.Italic = true;
                dateRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                
                // --- DATA TABLE ---
                int tableStartRow = 4;

                // Header
                worksheet.Cell(tableStartRow, 1).Value = "Ngày";
                worksheet.Cell(tableStartRow, 2).Value = "Tổng Nhập Kho";
                worksheet.Cell(tableStartRow, 3).Value = "Tổng Xuất Kho";

                // Data
                for (int i = 0; i < days.Count; i++)
                {
                    worksheet.Cell(tableStartRow + 1 + i, 1).Value = days[i];
                    worksheet.Cell(tableStartRow + 1 + i, 2).Value = imports[i];
                    worksheet.Cell(tableStartRow + 1 + i, 3).Value = exports[i];
                }

                // Number format
                worksheet.Column(2).Style.NumberFormat.Format = "#,##0";
                worksheet.Column(3).Style.NumberFormat.Format = "#,##0";

                worksheet.Columns().AdjustToContents();

                // Refine Formatting
                // 1. Vertical Center Alignment for all used cells
                var usedRange = worksheet.Range(tableStartRow, 1, tableStartRow + days.Count, 3);
                usedRange.Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;
                usedRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                usedRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                
                // Horizontal Alignment & Widths
                // Column 1 (Date): Center, Width 2.0x
                worksheet.Column(1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                worksheet.Column(1).Width *= 2.0;

                // Columns 2, 3 (Money): Right, Width 1.6x (80% of 2.0), Indent to simulate margins
                var moneyCols = worksheet.Columns("2,3");
                moneyCols.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                moneyCols.Style.Alignment.Indent = 3; 
                
                for (int c = 2; c <= 3; c++)
                {
                    worksheet.Column(c).Width *= 1.6;
                }
                
                // Style Header (Bold + Border)
                var headerRange = worksheet.Range(tableStartRow, 1, tableStartRow, 3);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                // Header Background Color (UIConstants.PrimaryColor.Light)
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromColor(UIConstants.PrimaryColor.Light);
                // Ensure headers are Centered regardless of data alignment
                headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Indent = 0; 
                headerRange.Style.Font.FontSize = 12;

                // Increase Row Height
                // Header row
                worksheet.Row(tableStartRow).Height = 22.5; 
                // Data rows
                for (int r = 0; r < days.Count; r++)
                {
                    worksheet.Row(tableStartRow + 1 + r).Height = 22.5;
                }

                // --- CHARTS ---
                int chartCol = 5; // Column E
                int chartRow = tableStartRow;

                if (pictureBoxImport.Image != null)
                {
                    // Chart Title
                    var chartTitle = worksheet.Cell(chartRow, chartCol);
                    chartTitle.Value = "BIỂU ĐỒ NHẬP KHO";
                    chartTitle.Style.Font.Bold = true;
                    chartTitle.Style.Font.FontColor = ClosedXML.Excel.XLColor.Green;
                    
                    using (var ms = new System.IO.MemoryStream())
                    {
                        pictureBoxImport.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                        
                        var picture = worksheet.AddPicture(ms).MoveTo(worksheet.Cell(chartRow + 1, chartCol));
                        
                        // Scale to fit approx 10 rows height
                        double scale = 1.0;
                        if (pictureBoxImport.Height > 0)
                        {
                            // Target height ~300px (15 rows * 20px)
                            scale = 300.0 / pictureBoxImport.Height;
                        }
                        picture.Scale(scale);
                        
                        // Move next chart down
                        chartRow += 17; // Title + Chart + Gap
                    }
                }

                if (pictureBoxExport.Image != null)
                {
                    // Chart Title
                    var chartTitle = worksheet.Cell(chartRow, chartCol);
                    chartTitle.Value = "BIỂU ĐỒ XUẤT KHO";
                    chartTitle.Style.Font.Bold = true;
                    chartTitle.Style.Font.FontColor = ClosedXML.Excel.XLColor.Red;

                    using (var ms = new System.IO.MemoryStream())
                    {
                        pictureBoxExport.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                        
                        var picture = worksheet.AddPicture(ms).MoveTo(worksheet.Cell(chartRow + 1, chartCol));
                        
                        double scale = 1.0;
                        if (pictureBoxExport.Height > 0)
                        {
                            scale = 300.0 / pictureBoxExport.Height;
                        }
                        picture.Scale(scale);
                    }
                }

                workbook.SaveAs(filePath);
            }
        }

        private void ExportToPdf(string filePath)
        {
            var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 20, 20);
            try
            {
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new System.IO.FileStream(filePath, System.IO.FileMode.Create));
                doc.Open();

                // Fonts
                var baseFont = iTextSharp.text.pdf.BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                var fontTitle = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
                var fontHeader = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                var fontNormal = new iTextSharp.text.Font(baseFont, 11, iTextSharp.text.Font.NORMAL);

                // Title
                var pTitle = new iTextSharp.text.Paragraph($"BÁO CÁO NHẬP XUẤT (Tính đến {dtpAnchorDate.Value:dd/MM/yyyy})", fontTitle);
                pTitle.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                pTitle.SpacingAfter = 10;
                doc.Add(pTitle);

                // Table
                iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 2, 2 });

                // Table Header
                AddPdfCell(table, "Ngày", fontHeader, true);
                AddPdfCell(table, "Tổng Nhập Kho", fontHeader, true);
                AddPdfCell(table, "Tổng Xuất Kho", fontHeader, true);

                // Table Data
                for (int i = 0; i < days.Count; i++)
                {
                    AddPdfCell(table, days[i], fontNormal);
                    AddPdfCell(table, imports[i].ToString("N0"), fontNormal, false, iTextSharp.text.Element.ALIGN_RIGHT);
                    AddPdfCell(table, exports[i].ToString("N0"), fontNormal, false, iTextSharp.text.Element.ALIGN_RIGHT);
                }

                table.SpacingAfter = 20;
                doc.Add(table);

                // Charts
                // Capture Image from PictureBoxes again to be sure (or reuse if possible, but recreating bitmap is safer)
                // Note: The PictureBox.Image is already a Bitmap with the chart drawn
                if (pictureBoxImport.Image != null)
                {
                    var imgImport = iTextSharp.text.Image.GetInstance(pictureBoxImport.Image, System.Drawing.Imaging.ImageFormat.Png);
                    imgImport.ScaleToFit(doc.PageSize.Width - 40, (doc.PageSize.Height - 100) / 2);
                    imgImport.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    imgImport.SpacingAfter = 10;
                    doc.Add(imgImport);
                }
                
                if (pictureBoxExport.Image != null)
                {
                    var imgExport = iTextSharp.text.Image.GetInstance(pictureBoxExport.Image, System.Drawing.Imaging.ImageFormat.Png);
                    imgExport.ScaleToFit(doc.PageSize.Width - 40, (doc.PageSize.Height - 100) / 2);
                    imgExport.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    doc.Add(imgExport);
                }
            }
            finally
            {
                doc.Close();
            }
        }

        private void AddPdfCell(iTextSharp.text.pdf.PdfPTable table, string text, iTextSharp.text.Font font, bool isHeader = false, int alignment = iTextSharp.text.Element.ALIGN_CENTER)
        {
            var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(text, font));
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            if (isHeader)
            {
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            }
            table.AddCell(cell);
        }

        private void LoadReport()
        {
            try
            {
                if (dtpAnchorDate == null)
                {
                    MessageBox.Show("DateTimePicker chưa được khởi tạo!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DateTime anchorDate = dtpAnchorDate.Value;
                var dailyData = chartService.GetImportExportByDay(anchorDate);

                // Clear data lists
                days.Clear();
                imports.Clear();
                exports.Clear();
                maxImport = 0;
                maxExport = 0;

                var displayList = new List<dynamic>();
                foreach (var dayEntry in dailyData)
                {
                    string day = dayEntry.Key;
                    decimal importValue = dayEntry.Value["Import"];
                    decimal exportValue = dayEntry.Value["Export"];

                    days.Add(day);
                    imports.Add(importValue);
                    exports.Add(exportValue);

                    if (importValue > maxImport) maxImport = importValue;
                    if (exportValue > maxExport) maxExport = exportValue;

                    displayList.Add(new { Day = day, Import = importValue.ToString("N0"), Export = exportValue.ToString("N0") });
                }

                // Set DataSource (no need to clear Rows when using DataSource)
                dgvReport.DataSource = null;
                dgvReport.DataSource = displayList;

                if (pictureBoxImport.Width > 0 && pictureBoxImport.Height > 0)
                {
                    DrawCharts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawCharts()
        {
            DrawImportChart();
            DrawExportChart();
        }

        private void DrawImportChart()
        {
            try
            {
                if (pictureBoxImport.Width <= 0 || pictureBoxImport.Height <= 0 || days.Count == 0 || maxImport <= 0)
                    return;

                Bitmap bitmap = new Bitmap(pictureBoxImport.Width, pictureBoxImport.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                int leftMargin = 50;
                int rightMargin = 10;
                int topMargin = 45;
                int bottomMargin = 35;
                
                int chartWidth = pictureBoxImport.Width - leftMargin - rightMargin;
                int chartHeight = pictureBoxImport.Height - topMargin - bottomMargin;
                
                double scaleFactor = 0.85;
                
                int totalBars = days.Count;
                int spacing = 3;
                int barWidth = Math.Max(8, (chartWidth - (spacing * (totalBars - 1))) / totalBars);

                Pen gridPen = new Pen(Color.FromArgb(230, 230, 230), 1);
                Font labelFont = new Font("Segoe UI", 8);
                int gridLines = 5;
                for (int i = 0; i <= gridLines; i++)
                {
                    int y = pictureBoxImport.Height - bottomMargin - (chartHeight * i / gridLines);
                    g.DrawLine(gridPen, leftMargin, y, pictureBoxImport.Width - rightMargin, y);
                    
                    decimal value = maxImport * i / gridLines;
                    string label = value >= 1000000 ? $"{value / 1000000:F1}M" : value >= 1000 ? $"{value / 1000:F0}K" : $"{value:F0}";
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, Brushes.Gray, leftMargin - labelSize.Width - 5, y - labelSize.Height / 2);
                }

                int xPos = leftMargin;
                Color startColor = Color.FromArgb(76, 175, 80);
                Color endColor = Color.FromArgb(129, 199, 132);

                for (int i = 0; i < days.Count; i++)
                {
                    int barHeight = maxImport > 0 ? (int)(((double)imports[i] / (double)maxImport) * chartHeight * scaleFactor) : 0;
                    int y = pictureBoxImport.Height - bottomMargin - barHeight;
                    
                    if (barHeight > 0)
                    {
                        System.Drawing.Drawing2D.LinearGradientBrush gradientBrush = 
                            new System.Drawing.Drawing2D.LinearGradientBrush(
                                new Rectangle(xPos, y, barWidth, barHeight),
                                startColor,
                                endColor,
                                System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                        
                        int radius = Math.Min(4, barWidth / 2);
                        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                        path.AddLine(xPos, y + barHeight, xPos, y + radius);
                        path.AddArc(xPos, y, radius * 2, radius * 2, 180, 90);
                        path.AddLine(xPos + radius, y, xPos + barWidth - radius, y);
                        path.AddArc(xPos + barWidth - radius * 2, y, radius * 2, radius * 2, 270, 90);
                        path.AddLine(xPos + barWidth, y + radius, xPos + barWidth, y + barHeight);
                        path.CloseFigure();
                        
                        g.FillPath(gradientBrush, path);
                        gradientBrush.Dispose();
                    }

                    if (i % Math.Max(1, days.Count / 10) == 0 || i == days.Count - 1)
                    {
                        string dayLabel = days[i].Substring(5);
                        SizeF daySize = g.MeasureString(dayLabel, labelFont);
                        g.DrawString(dayLabel, labelFont, Brushes.Gray, xPos + (barWidth - daySize.Width) / 2, pictureBoxImport.Height - bottomMargin + 5);
                    }

                    xPos += barWidth + spacing;
                }

                pictureBoxImport.Image = bitmap;
                g.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TransactionReportForm] DrawImportChart ERROR: {ex.Message}");
            }
        }

        private void DrawExportChart()
        {
            try
            {
                if (pictureBoxExport.Width <= 0 || pictureBoxExport.Height <= 0 || days.Count == 0 || maxExport <= 0)
                    return;

                Bitmap bitmap = new Bitmap(pictureBoxExport.Width, pictureBoxExport.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                int leftMargin = 50;
                int rightMargin = 10;
                int topMargin = 45;
                int bottomMargin = 35;
                
                int chartWidth = pictureBoxExport.Width - leftMargin - rightMargin;
                int chartHeight = pictureBoxExport.Height - topMargin - bottomMargin;
                
                double scaleFactor = 0.85;
                
                int totalBars = days.Count;
                int spacing = 3;
                int barWidth = Math.Max(8, (chartWidth - (spacing * (totalBars - 1))) / totalBars);

                Pen gridPen = new Pen(Color.FromArgb(230, 230, 230), 1);
                Font labelFont = new Font("Segoe UI", 8);
                int gridLines = 5;
                for (int i = 0; i <= gridLines; i++)
                {
                    int y = pictureBoxExport.Height - bottomMargin - (chartHeight * i / gridLines);
                    g.DrawLine(gridPen, leftMargin, y, pictureBoxExport.Width - rightMargin, y);
                    
                    decimal value = maxExport * i / gridLines;
                    string label = value >= 1000000 ? $"{value / 1000000:F1}M" : value >= 1000 ? $"{value / 1000:F0}K" : $"{value:F0}";
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, Brushes.Gray, leftMargin - labelSize.Width - 5, y - labelSize.Height / 2);
                }

                int xPos = leftMargin;
                Color startColor = Color.FromArgb(244, 67, 54);
                Color endColor = Color.FromArgb(239, 154, 154);

                for (int i = 0; i < days.Count; i++)
                {
                    int barHeight = maxExport > 0 ? (int)(((double)exports[i] / (double)maxExport) * chartHeight * scaleFactor) : 0;
                    int y = pictureBoxExport.Height - bottomMargin - barHeight;
                    
                    if (barHeight > 0)
                    {
                        System.Drawing.Drawing2D.LinearGradientBrush gradientBrush = 
                            new System.Drawing.Drawing2D.LinearGradientBrush(
                                new Rectangle(xPos, y, barWidth, barHeight),
                                startColor,
                                endColor,
                                System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                        
                        int radius = Math.Min(4, barWidth / 2);
                        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                        path.AddLine(xPos, y + barHeight, xPos, y + radius);
                        path.AddArc(xPos, y, radius * 2, radius * 2, 180, 90);
                        path.AddLine(xPos + radius, y, xPos + barWidth - radius, y);
                        path.AddArc(xPos + barWidth - radius * 2, y, radius * 2, radius * 2, 270, 90);
                        path.AddLine(xPos + barWidth, y + radius, xPos + barWidth, y + barHeight);
                        path.CloseFigure();
                        
                        g.FillPath(gradientBrush, path);
                        gradientBrush.Dispose();
                    }

                    if (i % Math.Max(1, days.Count / 10) == 0 || i == days.Count - 1)
                    {
                        string dayLabel = days[i].Substring(5);
                        SizeF daySize = g.MeasureString(dayLabel, labelFont);
                        g.DrawString(dayLabel, labelFont, Brushes.Gray, xPos + (barWidth - daySize.Width) / 2, pictureBoxExport.Height - bottomMargin + 5);
                    }

                    xPos += barWidth + spacing;
                }

                pictureBoxExport.Image = bitmap;
                g.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TransactionReportForm] DrawExportChart ERROR: {ex.Message}");
            }
        }
    }
}
