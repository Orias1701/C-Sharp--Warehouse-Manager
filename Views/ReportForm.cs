using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.Controllers;
using WarehouseManagement.Models;
using WarehouseManagement.Services;

namespace WarehouseManagement.Views
{
    /// <summary>
    /// Form hiển thị báo cáo và biểu đồ
    /// </summary>
    public partial class ReportForm : Form
    {
        private InventoryController _inventoryController;
        private ProductController _productController;
        private TabControl tabControl;
        private Label lblStockInfo, lblValueInfo, lblChartStock, lblChartValue;
        private DataGridView dgvImportExport;
        private Label lblImportExportChart;
        private ChartService _chartService;

        public ReportForm()
        {
            InitializeComponent();
            Text = "Báo Cáo & Biểu Đồ";
            _inventoryController = new InventoryController();
            _productController = new ProductController();
            _chartService = new ChartService();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // TabControl
            tabControl = new TabControl { Dock = DockStyle.Fill };

            // Tab 1: Tồn kho
            TabPage tabStock = new TabPage("Tồn Kho");
            lblChartStock = new Label
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10),
                Font = new Font("Courier New", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft
            };
            
            lblStockInfo = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.LightBlue,
                Padding = new Padding(10),
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            
            tabStock.Controls.Add(lblStockInfo);
            tabStock.Controls.Add(lblChartStock);
            tabControl.TabPages.Add(tabStock);

            // Tab 2: Giá trị tồn kho
            TabPage tabValue = new TabPage("Giá Trị");
            lblChartValue = new Label
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10),
                Font = new Font("Courier New", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft
            };

            lblValueInfo = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.LightGreen,
                Padding = new Padding(10),
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            
            tabValue.Controls.Add(lblValueInfo);
            tabValue.Controls.Add(lblChartValue);
            tabControl.TabPages.Add(tabValue);

            // Tab 3: Nhập/Xuất theo tháng
            TabPage tabImportExport = new TabPage("Nhập/Xuất");
            
            // Biểu đồ cột đôi
            lblImportExportChart = new Label
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10),
                Font = new Font("Courier New", 9),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft
            };
            
            // DataGridView hiển thị bảng
            dgvImportExport = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            
            dgvImportExport.Columns.Add("Month", "Tháng");
            dgvImportExport.Columns.Add("Import", "Tổng Nhập Kho");
            dgvImportExport.Columns.Add("Export", "Tổng Xuất Kho");
            
            tabImportExport.Controls.Add(dgvImportExport);
            tabImportExport.Controls.Add(lblImportExportChart);
            tabControl.TabPages.Add(tabImportExport);

            // Tab 4: Thống kê
            TabPage tabStats = new TabPage("Thống Kê");
            Label statsLabel = new Label { Dock = DockStyle.Fill, Font = new Font("Arial", 12), Padding = new Padding(15) };
            tabStats.Controls.Add(statsLabel);
            tabControl.TabPages.Add(tabStats);

            Controls.Add(tabControl);
            Load += ReportForm_Load;
            ResumeLayout(false);
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            LoadStockChart();
            LoadValueChart();
            LoadImportExportReport();
            LoadStatistics();
        }

        private void LoadStockChart()
        {
            try
            {
                List<Product> products = _productController.GetAllProducts();
                int totalLowStock = 0;
                string chart = "BIỂU ĐỒ TỒN KHO\n" + new string('=', 50) + "\n";

                foreach (var product in products)
                {
                    int barLength = Math.Min(product.Quantity / 10, 40);
                    string bar = new string('█', barLength);
                    chart += $"{product.ProductName,-25} {bar} {product.Quantity}\n";
                    if (product.IsLowStock) totalLowStock++;
                }

                lblChartStock.Text = chart;
                lblStockInfo.Text = $"Tổng sản phẩm: {products.Count} | Sản phẩm cảnh báo: {totalLowStock} | Tổng tồn kho: {GetTotalQuantity(products)} đơn vị";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void LoadValueChart()
        {
            try
            {
                List<Product> products = _productController.GetAllProducts();
                string chart = "BIỂU ĐỒ GIÁ TRỊ TỒN KHO\n" + new string('=', 50) + "\n";

                decimal totalValue = 0;
                foreach (var product in products)
                {
                    decimal value = product.Price * product.Quantity;
                    int barLength = Math.Min((int)(value / 1000000), 40);
                    string bar = new string('█', Math.Max(barLength, 1));
                    chart += $"{product.ProductName,-25} {bar} {value:C0}\n";
                    totalValue += value;
                }

                lblChartValue.Text = chart;
                lblValueInfo.Text = $"Tổng giá trị tồn kho: {totalValue:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Tải báo cáo nhập/xuất theo tháng
        /// </summary>
        private void LoadImportExportReport()
        {
            try
            {
                var monthlyData = _chartService.GetImportExportByMonth();

                // Xóa dữ liệu cũ
                dgvImportExport.Rows.Clear();

                decimal maxValue = 0;
                decimal totalImport = 0;
                decimal totalExport = 0;

                // Thêm dữ liệu vào bảng
                foreach (var monthEntry in monthlyData)
                {
                    string month = monthEntry.Key;
                    decimal importValue = monthEntry.Value["Import"];
                    decimal exportValue = monthEntry.Value["Export"];

                    dgvImportExport.Rows.Add(month, importValue.ToString("C"), exportValue.ToString("C"));

                    totalImport += importValue;
                    totalExport += exportValue;
                    if (importValue > maxValue) maxValue = importValue;
                    if (exportValue > maxValue) maxValue = exportValue;
                }

                // Tạo biểu đồ cột đôi
                string chart = DrawDoubleBarChart(monthlyData, maxValue);
                lblImportExportChart.Text = chart;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo nhập/xuất: " + ex.Message);
            }
        }

        /// <summary>
        /// Vẽ biểu đồ cột đôi nhập/xuất theo tháng
        /// </summary>
        private string DrawDoubleBarChart(Dictionary<string, Dictionary<string, decimal>> data, decimal maxValue)
        {
            string chart = "BIỂU ĐỒ NHẬP/XUẤT THEO THÁNG (12 tháng gần nhất)\n";
            chart += new string('=', 80) + "\n";
            chart += "Xanh = Nhập kho  |  Đỏ = Xuất kho\n";
            chart += new string('=', 80) + "\n\n";

            if (maxValue == 0) maxValue = 1; // Tránh chia cho 0

            int chartWidth = 40; // Độ rộng của biểu đồ

            foreach (var monthEntry in data)
            {
                string month = monthEntry.Key;
                decimal importValue = monthEntry.Value["Import"];
                decimal exportValue = monthEntry.Value["Export"];

                // Tính độ dài các thanh
                int importBarLength = (int)((importValue / maxValue) * chartWidth);
                int exportBarLength = (int)((exportValue / maxValue) * chartWidth);

                // Thanh Import (xanh lá)
                string importBar = new string('█', importBarLength);
                // Thanh Export (đỏ)
                string exportBar = new string('█', exportBarLength);

                chart += $"{month}  Nhập: {importBar} {importValue:C0}\n";
                chart += $"       Xuất: {exportBar} {exportValue:C0}\n\n";
            }

            return chart;
        }

        private void LoadStatistics()
        {
            try
            {
                List<Product> products = _productController.GetAllProducts();
                List<Product> lowStockProducts = _inventoryController.GetLowStockProducts();
                decimal totalValue = _inventoryController.GetTotalInventoryValue();

                int totalProducts = products.Count;
                int totalQuantity = GetTotalQuantity(products);
                int lowStockCount = lowStockProducts.Count;

                string stats = $@"
📊 THỐNG KÊ KHO HÀNG

Tổng số sản phẩm: {totalProducts}
Tổng tồn kho: {totalQuantity} đơn vị
Tổng giá trị: {totalValue:C}

⚠️ CẢNH BÁO TỒN KHO
Số sản phẩm cần nhập: {lowStockCount}";

                if (lowStockProducts.Count > 0)
                {
                    stats += "\n\nSản phẩm có tồn kho thấp:";
                    foreach (var p in lowStockProducts)
                    {
                        stats += $"\n  • {p.ProductName}: {p.Quantity}/{p.MinThreshold} đơn vị";
                    }
                }

                var label = tabControl.TabPages[3].Controls[0] as Label;
                label.Text = stats;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private int GetTotalQuantity(List<Product> products)
        {
            int total = 0;
            foreach (var p in products)
                total += p.Quantity;
            return total;
        }
    }
}
