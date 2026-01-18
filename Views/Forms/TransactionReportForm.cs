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
    /// Hiển thị biểu đồ cột đôi và bảng dữ liệu
    /// </summary>
    public class TransactionReportForm : Form
    {
        private PictureBox pictureBox;
        private DataGridView dgvReport;
        private ChartService chartService;
        private List<string> days;
        private List<decimal> imports;
        private List<decimal> exports;
        private decimal maxValue;
        private CustomDateTimePicker dtpAnchorDate;
        private CustomButton btnExportReport;

        public TransactionReportForm()
        {
            chartService = new ChartService();
            days = new List<string>();
            imports = new List<decimal>();
            exports = new List<decimal>();
            Console.WriteLine("[TransactionReportForm] Constructor started");
            InitializeComponent();
            Console.WriteLine("[TransactionReportForm] Constructor completed");
            
            // Apply theme
            ThemeManager.Instance.ApplyThemeToForm(this);
        }

        private void InitializeComponent()
        {
            try
            {
                Console.WriteLine("[INFO] InitializeComponent: Bắt đầu");

                // Form settings
                Text = $"{UIConstants.Icons.Chart} Báo Cáo Nhập/Xuất";
                Width = 1000;
                Height = 700;
                StartPosition = FormStartPosition.CenterParent;
                MaximizeBox = true;
                MinimizeBox = true;
                BackColor = ThemeManager.Instance.BackgroundLight;

                // Panel nút (trên cùng)
                CustomPanel buttonPanel = new CustomPanel
                {
                    Dock = DockStyle.Top,
                    Height = 65,
                    BackColor = ThemeManager.Instance.BackgroundLight,
                    ShowBorder = false,
                    Padding = new Padding(UIConstants.Spacing.Padding.Medium)
                };

                Label lblAnchorDate = new Label
                {
                    Text = $"{UIConstants.Icons.Calendar} Chọn ngày:",
                    Left = 15,
                    Top = 18,
                    Width = 95,
                    Height = 25,
                    Font = ThemeManager.Instance.FontRegular,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                buttonPanel.Controls.Add(lblAnchorDate);

                dtpAnchorDate = new CustomDateTimePicker
                {
                    Left = 115,
                    Top = 15,
                    Width = 160,
                    Value = DateTime.Now,
                    CustomFormat = "dd/MM/yyyy",
                    BorderRadius = UIConstants.Borders.RadiusMedium
                };
                dtpAnchorDate.ValueChanged += (s, e) => LoadReport();
                buttonPanel.Controls.Add(dtpAnchorDate);

                btnExportReport = new CustomButton
                {
                    Text = $"{UIConstants.Icons.Export} Xuất Báo Cáo",
                    Left = 290,
                    Top = 15,
                    Width = 150,
                    ButtonStyleType = ButtonStyle.FilledNoOutline
                };
                btnExportReport.Click += BtnExportReport_Click;
                buttonPanel.Controls.Add(btnExportReport);

                // PictureBox cho biểu đồ
                pictureBox = new PictureBox
                {
                    Dock = DockStyle.Top,
                    Height = 350,
                    BackColor = ThemeManager.Instance.BackgroundDefault,
                    BorderStyle = BorderStyle.FixedSingle
                };
                pictureBox.Resize += PictureBox_Resize;

                // DataGridView cho bảng
                dgvReport = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    BackgroundColor = ThemeManager.Instance.BackgroundDefault,
                    BorderStyle = BorderStyle.FixedSingle,
                    RowHeadersVisible = false,
                    Font = ThemeManager.Instance.FontRegular
                };

                dgvReport.Columns.Add("Day", "Ngày");
                dgvReport.Columns.Add("Import", "Tổng Nhập Kho");
                dgvReport.Columns.Add("Export", "Tổng Xuất Kho");

                Controls.Add(dgvReport);
                Controls.Add(pictureBox);
                Controls.Add(buttonPanel);

                Load += TransactionReportForm_Load;

                Console.WriteLine("[INFO] InitializeComponent: Hoàn thành");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] InitializeComponent: {ex.Message}");
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi khởi tạo form: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                // Header
                writer.WriteLine("Ngày,Tổng Nhập Kho,Tổng Xuất Kho");

                // Data
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
                // Sử dụng Open XML để tạo file Excel
                using (var spreadsheet = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    using (var writer = new System.IO.StreamWriter(spreadsheet))
                    {
                        // Viết header XML của Excel
                        writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        writer.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                        writer.WriteLine("<Worksheet ss:Name=\"BaoCao\">");
                        writer.WriteLine("<Table>");

                        // Header row
                        writer.WriteLine("<Row>");
                        writer.WriteLine("<Cell><Data ss:Type=\"String\">Ngày</Data></Cell>");
                        writer.WriteLine("<Cell><Data ss:Type=\"String\">Tổng Nhập Kho</Data></Cell>");
                        writer.WriteLine("<Cell><Data ss:Type=\"String\">Tổng Xuất Kho</Data></Cell>");
                        writer.WriteLine("</Row>");

                        // Data rows
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
                // Fallback to CSV if Excel export fails
                ExportToCSV(filePath.Replace(".xlsx", ".csv"));
            }
        }

        private void PictureBox_Resize(object sender, EventArgs e)
        {
            if (pictureBox.Width > 0 && pictureBox.Height > 0 && days.Count > 0)
            {
                Console.WriteLine($"[INFO] PictureBox_Resize: {pictureBox.Width}x{pictureBox.Height}");
                DrawChart();
            }
        }

        private void LoadReport()
        {
            try
            {
                Console.WriteLine("[TransactionReportForm] LoadReport: Started");

                // Use the selected anchor date or default to today
                DateTime anchorDate = dtpAnchorDate != null ? dtpAnchorDate.Value : DateTime.Now;
                var dailyData = chartService.GetImportExportByDay(anchorDate);
                Console.WriteLine($"[TransactionReportForm] LoadReport: Got {dailyData.Count} days");

                // Xóa dữ liệu cũ
                dgvReport.Rows.Clear();
                days.Clear();
                imports.Clear();
                exports.Clear();
                maxValue = 0;

                // Tính toán
                foreach (var dayEntry in dailyData)
                {
                    string day = dayEntry.Key;
                    decimal importValue = dayEntry.Value["Import"];
                    decimal exportValue = dayEntry.Value["Export"];

                    Console.WriteLine($"[TransactionReportForm] {day} -> Import={importValue}, Export={exportValue}");

                    days.Add(day);
                    imports.Add(importValue);
                    exports.Add(exportValue);

                    if (importValue > maxValue) maxValue = importValue;
                    if (exportValue > maxValue) maxValue = exportValue;

                    dgvReport.Rows.Add(day, importValue.ToString("N0"), exportValue.ToString("N0"));
                }

                Console.WriteLine($"[TransactionReportForm] LoadReport: Max value = {maxValue}, days.Count = {days.Count}");

                // Vẽ biểu đồ nếu PictureBox đã có kích thước
                if (pictureBox.Width > 0 && pictureBox.Height > 0)
                {
                    Console.WriteLine($"[TransactionReportForm] PictureBox ready ({pictureBox.Width}x{pictureBox.Height}), calling DrawChart");
                    DrawChart();
                }
                else
                {
                    Console.WriteLine($"[ReportForm] PictureBox NOT ready ({pictureBox.Width}x{pictureBox.Height})");
                }

                Console.WriteLine("[ReportForm] LoadReport: Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportForm] LoadReport ERROR: {ex.Message}");
                Console.WriteLine($"[ReportForm] {ex.StackTrace}");
                MessageBox.Show($"{UIConstants.Icons.Error} Lỗi tải báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawChart()
        {
            try
            {
                Console.WriteLine("[ReportForm] DrawChart: Started");
                Console.WriteLine($"[ReportForm] DrawChart: Size = {pictureBox.Width}x{pictureBox.Height}");
                Console.WriteLine($"[ReportForm] DrawChart: Days = {days.Count}, MaxValue = {maxValue}");

                if (pictureBox.Width <= 0 || pictureBox.Height <= 0)
                {
                    Console.WriteLine($"[ReportForm] DrawChart: PictureBox size invalid ({pictureBox.Width}x{pictureBox.Height})");
                    return;
                }

                if (maxValue <= 0 || days.Count == 0)
                {
                    Console.WriteLine($"[ReportForm] DrawChart: No data (maxValue={maxValue}, days={days.Count})");
                    return;
                }

                Console.WriteLine("[ReportForm] DrawChart: Creating bitmap");

                Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int margin = 50;
                int chartWidth = pictureBox.Width - (margin * 2);
                int chartHeight = pictureBox.Height - (margin * 2) - 30;
                // Tính toán để fit 30 ngày: mỗi ngày có 2 cột (nhập + xuất) + khoảng cách nhỏ
                int barWidth = Math.Max(2, chartWidth / (days.Count * 2 + 10)); // Giảm từ *3 xuống *2
                int spacing = 2; // Khoảng cách giữa 2 cột
                int daySpacing = 3; // Khoảng cách giữa các ngày

                Console.WriteLine($"[ReportForm] DrawChart: chartWidth={chartWidth}, chartHeight={chartHeight}, barWidth={barWidth}");

                // Tiêu đề
                Font titleFont = new Font("Arial", 12, FontStyle.Bold);
                g.DrawString("Báo Cáo Nhập/Xuất Theo Ngày (30 ngày gần nhất)", titleFont, Brushes.Black, margin, 5);

                // Trục
                Pen axisPen = new Pen(Color.Black, 2);
                g.DrawLine(axisPen, margin, pictureBox.Height - margin, pictureBox.Width - margin, pictureBox.Height - margin);
                g.DrawLine(axisPen, margin, 30, margin, pictureBox.Height - margin);

                // Vẽ cột
                int xPos = margin + 5;
                Brush greenBrush = new SolidBrush(Color.Green);
                Brush redBrush = new SolidBrush(Color.Red);
                Font monthFont = new Font("Arial", 5); // Giảm font size để fit 30 ngày

                for (int i = 0; i < days.Count; i++)
                {
                    int importHeight = maxValue > 0 ? (int)((imports[i] / maxValue) * chartHeight) : 0;
                    int exportHeight = maxValue > 0 ? (int)((exports[i] / maxValue) * chartHeight) : 0;

                    // Nhập (xanh)
                    int y1 = pictureBox.Height - margin - importHeight;
                    g.FillRectangle(greenBrush, xPos, y1, barWidth, Math.Max(1, importHeight));
                    if (barWidth > 1)
                        g.DrawRectangle(new Pen(Color.DarkGreen, 1), xPos, y1, barWidth, Math.Max(1, importHeight));

                    // Xuất (đỏ) - vẽ bên cạnh cột nhập
                    int xPos2 = xPos + barWidth + spacing;
                    int y2 = pictureBox.Height - margin - exportHeight;
                    g.FillRectangle(redBrush, xPos2, y2, barWidth, Math.Max(1, exportHeight));
                    if (barWidth > 1)
                        g.DrawRectangle(new Pen(Color.DarkRed, 1), xPos2, y2, barWidth, Math.Max(1, exportHeight));

                    // Ngày - hiển thị mỗi 5 ngày để không bị chồng chéo
                    if (i % 5 == 0)
                    {
                        string dayLabel = days[i].Substring(8); // Lấy phần ngày từ yyyy-MM-dd
                        g.DrawString(dayLabel, monthFont, Brushes.Black, xPos - 3, pictureBox.Height - margin + 3);
                    }

                    xPos += (barWidth * 2) + spacing + daySpacing;
                }

                Console.WriteLine("[ReportForm] DrawChart: Drawing bars completed");

                // Legend
                int legendX = pictureBox.Width - 150;
                int legendY = 35;
                g.FillRectangle(greenBrush, legendX, legendY, 15, 15);
                g.DrawString("Nhập", new Font("Arial", 9), Brushes.Black, legendX + 20, legendY);

                g.FillRectangle(redBrush, legendX, legendY + 20, 15, 15);
                g.DrawString("Xuất", new Font("Arial", 9), Brushes.Black, legendX + 20, legendY + 20);

                pictureBox.Image = bitmap;
                g.Dispose();

                Console.WriteLine("[ReportForm] DrawChart: Chart set to PictureBox");

                Console.WriteLine("[ReportForm] DrawChart: Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportForm] DrawChart ERROR: {ex.Message}");
                Console.WriteLine($"[ReportForm] {ex.StackTrace}");
            }
        }
    }
}