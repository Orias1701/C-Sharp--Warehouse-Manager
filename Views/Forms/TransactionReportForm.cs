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
        private const int FORM_HEIGHT = 900;
        private const int MARGIN = 75; // 2cm vùng an toàn
        private const int BUTTON_HEIGHT = 65;
        private const int GAP = 20;
        
        // Tính toán kích thước nội dung
        private const int CONTENT_WIDTH = FORM_WIDTH - (MARGIN * 2); // 1250px - margin trái phải
        // CONTENT_HEIGHT = Total height - Button height - Margin below button - Margin at bottom
        private const int CONTENT_HEIGHT = FORM_HEIGHT - BUTTON_HEIGHT - (MARGIN * 2); // 900 - 65 - 150 = 685px
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
                HeaderText = "Tổng Nhập Kho", 
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
                HeaderText = "Tổng Xuất Kho", 
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
                    Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv",
                    DefaultExt = "xlsx",
                    FileName = $"BaoCaoNhapXuat_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveDialog.FileName.EndsWith(".csv"))
                    {
                        ExportToCSV(saveDialog.FileName);
                    }
                    else
                    {
                        ExportToExcel(saveDialog.FileName);
                    }
                    MessageBox.Show($"{UIConstants.Icons.Success} Xuất báo cáo thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi xuất báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("Ngày,Tổng Nhập Kho,Tổng Xuất Kho");
                for (int i = 0; i < days.Count; i++)
                {
                    writer.WriteLine($"{days[i]},{imports[i]:N0},{exports[i]:N0}");
                }
            }
        }

        private void ExportToExcel(string filePath)
        {
            try
            {
                using (var spreadsheet = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    using (var writer = new System.IO.StreamWriter(spreadsheet))
                    {
                        writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        writer.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                        writer.WriteLine("<Worksheet ss:Name=\"BaoCao\">");
                        writer.WriteLine("<Table>");

                        writer.WriteLine("<Row>");
                        writer.WriteLine("<Cell><Data ss:Type=\"String\">Ngày</Data></Cell>");
                        writer.WriteLine("<Cell><Data ss:Type=\"String\">Tổng Nhập Kho</Data></Cell>");
                        writer.WriteLine("<Cell><Data ss:Type=\"String\">Tổng Xuất Kho</Data></Cell>");
                        writer.WriteLine("</Row>");

                        for (int i = 0; i < days.Count; i++)
                        {
                            writer.WriteLine("<Row>");
                            writer.WriteLine($"<Cell><Data ss:Type=\"String\">{days[i]}</Data></Cell>");
                            writer.WriteLine($"<Cell><Data ss:Type=\"Number\">{imports[i]}</Data></Cell>");
                            writer.WriteLine($"<Cell><Data ss:Type=\"Number\">{exports[i]}</Data></Cell>");
                            writer.WriteLine("</Row>");
                        }

                        writer.WriteLine("</Table>");
                        writer.WriteLine("</Worksheet>");
                        writer.WriteLine("</Workbook>");
                    }
                }
            }
            catch
            {
                ExportToCSV(filePath.Replace(".xlsx", ".csv"));
            }
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
