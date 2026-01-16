using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Services;

namespace WarehouseManagement.Views
{
    /// <summary>
    /// Panel báo cáo Nhập/Xuất theo ngày (30 ngày gần nhất)
    /// Hiển thị biểu đồ cột đôi và bảng dữ liệu
    /// </summary>
    public class ReportPanel : Panel
    {
        private PictureBox pictureBox;
        private DataGridView dgvReport;
        private ChartService chartService;
        private List<string> days;
        private List<decimal> imports;
        private List<decimal> exports;
        private decimal maxValue;

        public ReportPanel()
        {
            chartService = new ChartService();
            days = new List<string>();
            imports = new List<decimal>();
            exports = new List<decimal>();
            Console.WriteLine("[ReportPanel] Constructor started");
            InitializeComponent();
            Console.WriteLine("[ReportPanel] Constructor completed");
        }

        private void InitializeComponent()
        {
            try
            {
                Console.WriteLine("[INFO] InitializeComponent: Bắt đầu");

                // PictureBox cho biểu đồ
                pictureBox = new PictureBox
                {
                    Dock = DockStyle.Top,
                    Height = 350,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.Fixed3D
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
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.Fixed3D
                };

                dgvReport.Columns.Add("Day", "Ngày");
                dgvReport.Columns.Add("Import", "Tổng Nhập Kho");
                dgvReport.Columns.Add("Export", "Tổng Xuất Kho");

                Controls.Add(pictureBox);
                Controls.Add(dgvReport);

                Dock = DockStyle.Fill;

                HandleCreated += (s, e) => LoadReport();

                Console.WriteLine("[INFO] InitializeComponent: Hoàn thành");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] InitializeComponent: {ex.Message}");
                MessageBox.Show($"Lỗi khởi tạo panel: {ex.Message}");
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
                Console.WriteLine("[ReportPanel] LoadReport: Started");

                var dailyData = chartService.GetImportExportByDay();
                Console.WriteLine($"[ReportPanel] LoadReport: Got {dailyData.Count} days");

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

                    Console.WriteLine($"[ReportPanel] {day} -> Import={importValue}, Export={exportValue}");

                    days.Add(day);
                    imports.Add(importValue);
                    exports.Add(exportValue);

                    if (importValue > maxValue) maxValue = importValue;
                    if (exportValue > maxValue) maxValue = exportValue;

                    dgvReport.Rows.Add(day, importValue.ToString("C"), exportValue.ToString("C"));
                }

                Console.WriteLine($"[ReportPanel] LoadReport: Max value = {maxValue}, days.Count = {days.Count}");

                // Vẽ biểu đồ nếu PictureBox đã có kích thước
                if (pictureBox.Width > 0 && pictureBox.Height > 0)
                {
                    Console.WriteLine($"[ReportPanel] PictureBox ready ({pictureBox.Width}x{pictureBox.Height}), calling DrawChart");
                    DrawChart();
                }
                else
                {
                    Console.WriteLine($"[ReportPanel] PictureBox NOT ready ({pictureBox.Width}x{pictureBox.Height})");
                }

                Console.WriteLine("[ReportPanel] LoadReport: Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportPanel] LoadReport ERROR: {ex.Message}");
                Console.WriteLine($"[ReportPanel] {ex.StackTrace}");
                MessageBox.Show($"Lỗi tải báo cáo: {ex.Message}");
            }
        }

        private void DrawChart()
        {
            try
            {
                Console.WriteLine("[ReportPanel] DrawChart: Started");
                Console.WriteLine($"[ReportPanel] DrawChart: Size = {pictureBox.Width}x{pictureBox.Height}");
                Console.WriteLine($"[ReportPanel] DrawChart: Days = {days.Count}, MaxValue = {maxValue}");

                if (pictureBox.Width <= 0 || pictureBox.Height <= 0)
                {
                    Console.WriteLine($"[ReportPanel] DrawChart: PictureBox size invalid ({pictureBox.Width}x{pictureBox.Height})");
                    return;
                }

                if (maxValue <= 0 || days.Count == 0)
                {
                    Console.WriteLine($"[ReportPanel] DrawChart: No data (maxValue={maxValue}, days={days.Count})");
                    return;
                }

                Console.WriteLine("[ReportPanel] DrawChart: Creating bitmap");

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

                Console.WriteLine($"[ReportPanel] DrawChart: chartWidth={chartWidth}, chartHeight={chartHeight}, barWidth={barWidth}");

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

                Console.WriteLine("[ReportPanel] DrawChart: Drawing bars completed");

                // Legend
                int legendX = pictureBox.Width - 150;
                int legendY = 35;
                g.FillRectangle(greenBrush, legendX, legendY, 15, 15);
                g.DrawString("Nhập", new Font("Arial", 9), Brushes.Black, legendX + 20, legendY);

                g.FillRectangle(redBrush, legendX, legendY + 20, 15, 15);
                g.DrawString("Xuất", new Font("Arial", 9), Brushes.Black, legendX + 20, legendY + 20);

                pictureBox.Image = bitmap;
                g.Dispose();

                Console.WriteLine("[ReportPanel] DrawChart: Chart set to PictureBox");

                Console.WriteLine("[ReportPanel] DrawChart: Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportPanel] DrawChart ERROR: {ex.Message}");
                Console.WriteLine($"[ReportPanel] {ex.StackTrace}");
            }
        }
    }
}
