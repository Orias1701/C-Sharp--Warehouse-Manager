using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// Service xử lý dữ liệu thô thành dữ liệu biểu đồ
    /// </summary>
    public class ChartService
    {
        private readonly ProductRepository _productRepo;
        private readonly TransactionRepository _transactionRepo;

        public ChartService()
        {
            _productRepo = new ProductRepository();
            _transactionRepo = new TransactionRepository();
        }

        /// <summary>
        /// Lấy dữ liệu tồn kho theo sản phẩm (cho biểu đồ cột)
        /// </summary>
        public Dictionary<string, int> GetInventoryByProduct()
        {
            try
            {
                var products = _productRepo.GetAllProducts();
                return products.ToDictionary(p => p.ProductName, p => p.Quantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu tồn kho: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy dữ liệu giá trị tồn kho theo sản phẩm (cho biểu đồ cột)
        /// </summary>
        public Dictionary<string, decimal> GetInventoryValueByProduct()
        {
            try
            {
                var products = _productRepo.GetAllProducts();
                return products.ToDictionary(p => p.ProductName, p => p.Price * p.Quantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy giá trị tồn kho: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy dữ liệu Nhập/Xuất theo ngày
        /// </summary>
        public Dictionary<string, int> GetImportExportByDate(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var transactions = _transactionRepo.GetAllTransactions();
                var filtered = transactions.Where(t => t.DateCreated >= fromDate && t.DateCreated <= toDate).ToList();

                var result = new Dictionary<string, int>();
                foreach (var group in filtered.GroupBy(t => t.DateCreated.Date))
                {
                    int imports = group.Where(t => t.Type == "Import").Sum(t => t.Details.Sum(d => d.Quantity));
                    int exports = group.Where(t => t.Type == "Export").Sum(t => t.Details.Sum(d => d.Quantity));
                    result[group.Key.ToString("yyyy-MM-dd")] = imports - exports;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu nhập/xuất: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy dữ liệu Nhập/Xuất theo loại (cho biểu đồ tròn)
        /// </summary>
        public Dictionary<string, int> GetTransactionByType()
        {
            try
            {
                var transactions = _transactionRepo.GetAllTransactions();
                var imports = transactions.Count(t => t.Type == "Import");
                var exports = transactions.Count(t => t.Type == "Export");

                return new Dictionary<string, int>
                {
                    { "Nhập kho", imports },
                    { "Xuất kho", exports }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu loại phiếu: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quát
        /// </summary>
        public Dictionary<string, object> GetGeneralStatistics()
        {
            try
            {
                var products = _productRepo.GetAllProducts();
                var transactions = _transactionRepo.GetAllTransactions();

                return new Dictionary<string, object>
                {
                    { "TotalProducts", products.Count },
                    { "TotalQuantity", products.Sum(p => p.Quantity) },
                    { "TotalValue", products.Sum(p => p.Price * p.Quantity) },
                    { "LowStockCount", products.Count(p => p.IsLowStock) },
                    { "TotalTransactions", transactions.Count },
                    { "AvgProductPrice", products.Count > 0 ? products.Average(p => p.Price) : 0 }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy thống kê: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy dữ liệu Nhập/Xuất theo tháng (12 tháng gần nhất)
        /// Trả về Dictionary với key là tháng (yyyy-MM), value là {Import: giá trị, Export: giá trị}
        /// </summary>
        public Dictionary<string, Dictionary<string, decimal>> GetImportExportByMonth()
        {
            var result = new Dictionary<string, Dictionary<string, decimal>>();
            try
            {
                var transactions = _transactionRepo.GetAllTransactions();
                Console.WriteLine($"[ChartService] GetImportExportByMonth: Total transactions = {transactions.Count}");

                // Lấy 12 tháng gần nhất
                DateTime now = DateTime.Now;
                DateTime startDate = now.AddMonths(-11);

                // Tạo danh sách các tháng
                for (int i = 0; i < 12; i++)
                {
                    DateTime monthDate = startDate.AddMonths(i);
                    string monthKey = monthDate.ToString("yyyy-MM");
                    result[monthKey] = new Dictionary<string, decimal>
                    {
                        { "Import", 0 },
                        { "Export", 0 }
                    };
                }

                // Tính toán tổng giá trị nhập/xuất cho mỗi tháng
                int processedCount = 0;
                foreach (var transaction in transactions)
                {
                    string monthKey = transaction.DateCreated.ToString("yyyy-MM");
                    
                    // Chỉ xử lý các giao dịch trong 12 tháng gần nhất
                    if (!result.ContainsKey(monthKey))
                        continue;

                    decimal totalValue = 0;
                    if (transaction.Details != null && transaction.Details.Count > 0)
                    {
                        totalValue = transaction.Details.Sum(d => d.UnitPrice * (decimal)d.Quantity);
                        Console.WriteLine($"[ChartService]   {monthKey} {transaction.Type}: {transaction.Details.Count} items, Total={totalValue}");
                    }

                    if (transaction.Type == "Import")
                    {
                        result[monthKey]["Import"] += totalValue;
                    }
                    else if (transaction.Type == "Export")
                    {
                        result[monthKey]["Export"] += totalValue;
                    }

                    processedCount++;
                }

                Console.WriteLine($"[ChartService] Processed {processedCount} transactions");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChartService] ERROR: {ex.Message}\n{ex.StackTrace}");
                throw new Exception("Lỗi khi lấy dữ liệu nhập/xuất theo tháng: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy dữ liệu Nhập/Xuất theo ngày (30 ngày gần nhất)
        /// Trả về Dictionary với key là ngày (yyyy-MM-dd), value là {Import: giá trị, Export: giá trị}
        /// </summary>
        public Dictionary<string, Dictionary<string, decimal>> GetImportExportByDay()
        {
            var result = new Dictionary<string, Dictionary<string, decimal>>();
            try
            {
                var transactions = _transactionRepo.GetAllTransactions();
                Console.WriteLine($"[ChartService] GetImportExportByDay: Total transactions = {transactions.Count}");

                // Lấy 30 ngày gần nhất
                DateTime now = DateTime.Now;
                DateTime startDate = now.AddDays(-29);

                // Tạo danh sách các ngày
                for (int i = 0; i < 30; i++)
                {
                    DateTime dayDate = startDate.AddDays(i);
                    string dayKey = dayDate.ToString("yyyy-MM-dd");
                    result[dayKey] = new Dictionary<string, decimal>
                    {
                        { "Import", 0 },
                        { "Export", 0 }
                    };
                }

                // Tính toán tổng giá trị nhập/xuất cho mỗi ngày
                int processedCount = 0;
                foreach (var transaction in transactions)
                {
                    string dayKey = transaction.DateCreated.ToString("yyyy-MM-dd");
                    
                    // Chỉ xử lý các giao dịch trong 30 ngày gần nhất
                    if (!result.ContainsKey(dayKey))
                        continue;

                    decimal totalValue = 0;
                    if (transaction.Details != null && transaction.Details.Count > 0)
                    {
                        totalValue = transaction.Details.Sum(d => d.UnitPrice * (decimal)d.Quantity);
                        Console.WriteLine($"[ChartService]   {dayKey} {transaction.Type}: {transaction.Details.Count} items, Total={totalValue}");
                    }

                    if (transaction.Type == "Import")
                    {
                        result[dayKey]["Import"] += totalValue;
                    }
                    else if (transaction.Type == "Export")
                    {
                        result[dayKey]["Export"] += totalValue;
                    }

                    processedCount++;
                }

                Console.WriteLine($"[ChartService] Processed {processedCount} transactions for days");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChartService] ERROR: {ex.Message}\n{ex.StackTrace}");
                throw new Exception("Lỗi khi lấy dữ liệu nhập/xuất theo ngày: " + ex.Message);
            }
        }
    }
}
