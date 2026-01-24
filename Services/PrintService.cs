using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.Controllers;
using System.Drawing.Drawing2D;

namespace WarehouseManagement.Services
{
    public class PrintService
    {
        // Font setup
        // Fonts for Professional Transaction Print (Times New Roman)
        private Font _titleFont = new Font("Times New Roman", 20, FontStyle.Bold);
        private Font _headerFont = new Font("Times New Roman", 13, FontStyle.Bold);
        private Font _headerRegularFont = new Font("Times New Roman", 13, FontStyle.Regular);
        private Font _contentFont = new Font("Times New Roman", 12, FontStyle.Regular);
        private Font _contentBoldFont = new Font("Times New Roman", 12, FontStyle.Bold);
        private Font _smallFont = new Font("Times New Roman", 10, FontStyle.Italic);
        private Font _companyNameFont = new Font("Times New Roman", 14, FontStyle.Bold);

        // Fonts for Legacy Inventory Check Print (Arial) - "Very Good" style
        private Font _arialTitleFont = new Font("Arial", 18, FontStyle.Bold);
        private Font _arialHeaderFont = new Font("Arial", 12, FontStyle.Bold);
        private Font _arialContentFont = new Font("Arial", 10, FontStyle.Regular);
        private Font _arialContentBoldFont = new Font("Arial", 10, FontStyle.Bold);

        private Brush _brush = Brushes.Black;
        private Pen _pen = new Pen(Color.Black, 1);
        private Pen _gridPen = new Pen(Color.Black, 1);

        // Transaction Data
        private Transaction _transactionToPrint;
        private SupplierController _supController;
        private CustomerController _cusController;
        private UserController _userController;

        // Inventory Check Data
        private InventoryCheck _checkToPrint;
        // Also support printing list of details before creating check (New Check)
        private System.Collections.Generic.List<InventoryCheckDetail> _pendingDetails;
        private string _pendingNote;
        private int _pendingUserId;

        public PrintService()
        {
            _supController = new SupplierController();
            _cusController = new CustomerController();
            _userController = new UserController();
        }

        public void PrintTransaction(Transaction transaction)
        {
            _transactionToPrint = transaction;
            _checkToPrint = null;
            _pendingDetails = null;
            ShowPrintDialog();
        }

        public void PrintInventoryCheck(InventoryCheck check)
        {
            _checkToPrint = check;
            _transactionToPrint = null;
            _pendingDetails = null;
            ShowPrintDialog();
        }

        public void PrintPendingCheck(System.Collections.Generic.List<InventoryCheckDetail> details, string note, int userId)
        {
            _pendingDetails = details;
            _pendingNote = note;
            _pendingUserId = userId;
            _checkToPrint = null;
            _transactionToPrint = null;
            ShowPrintDialog();
        }

        public void PrintPendingTransaction(string type, System.Collections.Generic.List<TransactionDetail> details, string note, int supplierId, int customerId, int userId)
        {
            // Create dummy Transaction object
            var t = new Transaction
            {
                TransactionID = 0, // 0 for Draft
                Type = type,
                DateCreated = DateTime.Now,
                CreatedByUserID = userId,
                Note = note,
                SupplierID = supplierId > 0 ? (int?)supplierId : null,
                CustomerID = customerId > 0 ? (int?)customerId : null,
                Details = details 
            };
            
            // Calculate Total
            decimal total = 0;
            foreach(var d in details) total += d.SubTotal;
            t.FinalAmount = total;

            _transactionToPrint = t;
            _checkToPrint = null;
            _pendingDetails = null;
            ShowPrintDialog();
        }



        private void ShowPrintPreview()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;
            pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169); // A4 Size

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = pd;
            preview.Width = 1000;
            preview.Height = 800;
            preview.StartPosition = FormStartPosition.CenterScreen;
            ((Form)preview).Text = "Xem trước khi in";
            
            preview.ShowDialog();
        }

        private void ShowPrintDialog()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            printDialog.UseEXDialog = true;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try 
                {
                    pd.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi in: " + ex.Message);
                }
            }
        }

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_transactionToPrint != null)
                PrintTransactionPage(e);
            else if (_checkToPrint != null)
                PrintCheckPage(e);
            else if (_pendingDetails != null)
                PrintPendingCheckPage(e);
        }

        private void PrintTransactionPage(PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float y = 50;
            float leftMargin = 50;
            float rightMargin = e.PageBounds.Width - 50;
            float width = rightMargin - leftMargin;

            // Title - Match Inventory Check Style
            string typeName = _transactionToPrint.Type == "Import" ? "PHIẾU NHẬP KHO" : "PHIẾU XUẤT KHO";
            SizeF titleSize = g.MeasureString(typeName, _arialTitleFont);
            g.DrawString(typeName, _arialTitleFont, _brush, (e.PageBounds.Width - titleSize.Width) / 2, y);
            y += 40;

            // Info Block - Match Inventory Check Style (Simple lines)
            string idStr = _transactionToPrint.TransactionID == 0 ? "(NHÁP)" : ("#" + _transactionToPrint.TransactionID);
            g.DrawString($"Mã giao dịch: {idStr}", _arialContentBoldFont, _brush, leftMargin, y);
             string dateStr = _transactionToPrint.DateCreated.ToString("dd/MM/yyyy HH:mm");
            g.DrawString($"Ngày tạo: {dateStr}", _arialContentFont, _brush, leftMargin + width / 2, y);
            y += 20;

            // Partner
            string partnerName = "N/A";
            if (_transactionToPrint.Type == "Import" && _transactionToPrint.SupplierID.HasValue)
            {
                var sup = _supController.GetSupplierById(_transactionToPrint.SupplierID.Value);
                partnerName = sup != null ? sup.SupplierName : "N/A";
            }
            else if (_transactionToPrint.Type == "Export" && _transactionToPrint.CustomerID.HasValue)
            {
                var cus = _cusController.GetCustomerById(_transactionToPrint.CustomerID.Value);
                partnerName = cus != null ? cus.CustomerName : "N/A";
            }
            g.DrawString($"Đối tác: {partnerName}", _arialContentFont, _brush, leftMargin, y);
            y += 20;

             // Creator
            string creator = "User #" + _transactionToPrint.CreatedByUserID;
            try {
                var u = _userController.GetUserById(_transactionToPrint.CreatedByUserID);
                if (u != null) creator = u.FullName;
            } catch {}
            g.DrawString($"Người lập: {creator}", _arialContentFont, _brush, leftMargin, y);
            y += 20;

             // Note
             string note = string.IsNullOrEmpty(_transactionToPrint.Note) ? "(Không có)" : _transactionToPrint.Note;
             g.DrawString($"Ghi chú: {note}", _arialContentFont, _brush, leftMargin, y);
             y += 30;

            // Grid - Match Inventory Check Style (Simple Rectangles)
            float col1 = leftMargin; // STT
            float col2 = leftMargin + 40; // Ten SP
            float col3 = leftMargin + 350; // SL
            float col4 = leftMargin + 430; // Don Gia
            float col5 = leftMargin + 580; // Thanh Tien
            
            float rowHeight = 25;

            g.DrawRectangle(_gridPen, leftMargin, y, width, rowHeight);
            g.DrawString("STT", _arialContentBoldFont, _brush, col1 + 5, y + 5);
            g.DrawString("Tên Sản Phẩm", _arialContentBoldFont, _brush, col2 + 5, y + 5);
            g.DrawString("SL", _arialContentBoldFont, _brush, col3 + 5, y + 5);
            g.DrawString("Đơn Giá", _arialContentBoldFont, _brush, col4 + 5, y + 5);
            g.DrawString("Thành Tiền", _arialContentBoldFont, _brush, col5 + 5, y + 5);
            y += rowHeight;

            // Content
            if (_transactionToPrint.Details != null)
            {
                int stt = 1;
                foreach (var item in _transactionToPrint.Details)
                {
                    g.DrawRectangle(_gridPen, leftMargin, y, width, rowHeight);
                    
                    g.DrawString(stt.ToString(), _arialContentFont, _brush, col1 + 5, y + 5);
                    g.DrawString(item.ProductName, _arialContentFont, _brush, col2 + 5, y + 5);
                    
                    string qty = item.Quantity.ToString("N0");
                    string price = item.UnitPrice.ToString("N0");
                    string sub = item.SubTotal.ToString("N0");

                    g.DrawString(qty, _arialContentFont, _brush, col3 + 5, y + 5);
                    g.DrawString(price, _arialContentFont, _brush, col4 + 5, y + 5);
                    g.DrawString(sub, _arialContentFont, _brush, col5 + 5, y + 5);

                    y += rowHeight;
                    stt++;
                }
            }

            y += 20; // Spacing

            // Total
            string totalStr = $"Tổng tiền: {_transactionToPrint.FinalAmount:N0} VND";
            // Use Arial font for total
             SizeF totalSize = g.MeasureString(totalStr, _arialHeaderFont);
            g.DrawString(totalStr, _arialHeaderFont, _brush, rightMargin - totalSize.Width, y);
            
             y += 50;
             // Signatures - Simple style
             g.DrawString("Người lập phiếu", _arialContentFont, _brush, leftMargin + 50, y);
             g.DrawString("Người nhận/giao", _arialContentFont, _brush, rightMargin - 150, y);
        }

        private void PrintCheckPage(PrintPageEventArgs e)
        {
            // Restore exact previous styles (Arial)
            Graphics g = e.Graphics;
            float y = 50;
            float leftMargin = 50;
            float rightMargin = e.PageBounds.Width - 50;
            float width = rightMargin - leftMargin;

            string title = "PHIẾU KIỂM KÊ KHO";
            SizeF titleSize = g.MeasureString(title, _arialTitleFont);
            g.DrawString(title, _arialTitleFont, _brush, (e.PageBounds.Width - titleSize.Width) / 2, y);
            y += 40;

            g.DrawString($"Mã phiếu: #{_checkToPrint.CheckID}", _arialContentBoldFont, _brush, leftMargin, y);
            
            string dateStr = _checkToPrint.CheckDate.ToString("dd/MM/yyyy HH:mm");
            // Right align date? Or center based on previous logic?
            // Previous logic: g.DrawString($"Ngày tạo: ...", ..., leftMargin + width / 2, y);
            g.DrawString($"Ngày tạo: {dateStr}", _arialContentFont, _brush, leftMargin + width / 2, y);
            y += 20;
            
            string status = _checkToPrint.Status == "Pending" ? "Đang chờ duyệt" : (_checkToPrint.Status == "Approved" ? "Đã duyệt" : _checkToPrint.Status);
            g.DrawString($"Trạng thái: {status}", _arialContentFont, _brush, leftMargin, y);
            y += 20;

            string note = string.IsNullOrEmpty(_checkToPrint.Note) ? "(Không có)" : _checkToPrint.Note;
            g.DrawString($"Ghi chú: {note}", _arialContentFont, _brush, leftMargin, y);
            y += 30;

            // Grid
            float col1 = leftMargin; // STT
            float col2 = leftMargin + 40; // ID SP
            float col3 = leftMargin + 100; // Ten SP
            float col4 = leftMargin + 400; // Ton May
            float col5 = leftMargin + 480; // Ton Thuc
            float col6 = leftMargin + 560; // Chenh Lech
            
            float rowHeight = 25;

            g.DrawRectangle(_gridPen, leftMargin, y, width, rowHeight);
            g.DrawString("STT", _arialContentBoldFont, _brush, col1 + 5, y + 5);
            g.DrawString("ID", _arialContentBoldFont, _brush, col2 + 5, y + 5);
            g.DrawString("Tên Sản Phẩm", _arialContentBoldFont, _brush, col3 + 5, y + 5);
            g.DrawString("Máy", _arialContentBoldFont, _brush, col4 + 5, y + 5);
            g.DrawString("Thực", _arialContentBoldFont, _brush, col5 + 5, y + 5);
            g.DrawString("Lệch", _arialContentBoldFont, _brush, col6 + 5, y + 5);
            y += rowHeight;

             if (_checkToPrint.Details != null)
            {
                int stt = 1;
                ProductController pc = new ProductController();
                var products = pc.GetAllProducts(); 
                
                foreach (var item in _checkToPrint.Details)
                {
                    g.DrawRectangle(_gridPen, leftMargin, y, width, rowHeight);
                    
                    var pName = products.FirstOrDefault(p => p.ProductID == item.ProductID)?.ProductName ?? "Unknown";
                    
                    g.DrawString(stt.ToString(), _arialContentFont, _brush, col1 + 5, y + 5);
                    g.DrawString(item.ProductID.ToString(), _arialContentFont, _brush, col2 + 5, y + 5);
                    g.DrawString(pName, _arialContentFont, _brush, col3 + 5, y + 5);
                    g.DrawString(item.SystemQuantity.ToString(), _arialContentFont, _brush, col4 + 5, y + 5);
                    g.DrawString(item.ActualQuantity.ToString(), _arialContentFont, _brush, col5 + 5, y + 5);
                    g.DrawString(item.Difference.ToString(), _arialContentFont, _brush, col6 + 5, y + 5);

                    y += rowHeight;
                    stt++;
                }
            }
             
             y += 50;
             g.DrawString("Người kiểm kê", _arialContentFont, _brush, leftMargin + 50, y);
             g.DrawString("Người duyệt", _arialContentFont, _brush, rightMargin - 150, y);
        }

        private void PrintPendingCheckPage(PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float y = 50;
            float leftMargin = 50;
            float rightMargin = e.PageBounds.Width - 50;
            float width = rightMargin - leftMargin;

            string title = "PHIẾU KIỂM KÊ (NHÁP)";
            SizeF titleSize = g.MeasureString(title, _arialTitleFont);
            g.DrawString(title, _arialTitleFont, _brush, (e.PageBounds.Width - titleSize.Width) / 2, y);
            y += 40;

            g.DrawString($"Ngày lập: {DateTime.Now:dd/MM/yyyy HH:mm}", _arialContentFont, _brush, leftMargin + width / 2, y);
            y += 20;

            string note = string.IsNullOrEmpty(_pendingNote) ? "(Không có)" : _pendingNote;
            g.DrawString($"Ghi chú: {note}", _arialContentFont, _brush, leftMargin, y);
            y += 30;

             // Grid
            float col1 = leftMargin; // STT
            float col2 = leftMargin + 40; // ID SP
            float col3 = leftMargin + 100; // Ten SP
            float col4 = leftMargin + 400; // Ton May
            float col5 = leftMargin + 480; // Ton Thuc
            float col6 = leftMargin + 560; // Chenh Lech
            
            float rowHeight = 25;

            g.DrawRectangle(_gridPen, leftMargin, y, width, rowHeight);
            g.DrawString("STT", _arialContentBoldFont, _brush, col1 + 5, y + 5);
            g.DrawString("ID", _arialContentBoldFont, _brush, col2 + 5, y + 5);
            g.DrawString("Tên Sản Phẩm", _arialContentBoldFont, _brush, col3 + 5, y + 5);
            g.DrawString("Máy", _arialContentBoldFont, _brush, col4 + 5, y + 5);
            g.DrawString("Thực", _arialContentBoldFont, _brush, col5 + 5, y + 5);
            g.DrawString("Lệch", _arialContentBoldFont, _brush, col6 + 5, y + 5);
            y += rowHeight;
            
            ProductController pc = new ProductController();
            var products = pc.GetAllProducts(); 

             if (_pendingDetails != null)
            {
                int stt = 1;
                foreach (var item in _pendingDetails)
                {
                    g.DrawRectangle(_gridPen, leftMargin, y, width, rowHeight);
                    
                    var pName = products.FirstOrDefault(p => p.ProductID == item.ProductID)?.ProductName ?? "Unknown";
                    
                    g.DrawString(stt.ToString(), _arialContentFont, _brush, col1 + 5, y + 5);
                    g.DrawString(item.ProductID.ToString(), _arialContentFont, _brush, col2 + 5, y + 5);
                    g.DrawString(pName, _arialContentFont, _brush, col3 + 5, y + 5);
                    g.DrawString(item.SystemQuantity.ToString(), _arialContentFont, _brush, col4 + 5, y + 5);
                    g.DrawString(item.ActualQuantity.ToString(), _arialContentFont, _brush, col5 + 5, y + 5);
                    g.DrawString(item.Difference.ToString(), _arialContentFont, _brush, col6 + 5, y + 5);

                    y += rowHeight;
                    stt++;
                }
            }
             
             y += 50;
             g.DrawString("Người lập phiếu", _arialContentFont, _brush, leftMargin + 50, y);
        }

        private string NumberToVietnameseText(decimal number)
        {
            string s = number.ToString("#");
            string[] digits = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] units = { "", "nghìn", "triệu", "tỷ" };
            string result = "";
            int i = 0;
            
            while (s.Length > 0)
            {
                // Retrieve last 3 digits
                int len = s.Length;
                string chunkStr = len > 3 ? s.Substring(len - 3) : s;
                s = len > 3 ? s.Substring(0, len - 3) : "";
                
                int chunk = int.Parse(chunkStr);
                
                if (chunk > 0)
                {
                    string chunkText = ReadChunk(chunk, s.Length == 0); // IsLeading = true if s is empty
                    result = chunkText + " " + units[i] + " " + result;
                }
                i++;
            }
            // Fix spaces
            result = System.Text.RegularExpressions.Regex.Replace(result.Trim(), @"\s+", " ");
            return result;
        }

        private string ReadChunk(int number, bool isLeadingGroup)
        {
            string[] digits = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string result = "";
            
            int hundreds = number / 100;
            int remainder = number % 100;
            int tens = remainder / 10;
            int ones = remainder % 10;
            
            if (hundreds > 0 || !isLeadingGroup)
            {
                result += digits[hundreds] + " trăm";
                if (remainder > 0 && remainder < 10) result += " lẻ";
            }
            
            if (tens > 1)
            {
                result += " " + digits[tens] + " mươi";
                if (ones == 1) result += " mốt";
                else if (ones == 5) result += " lăm";
                else if (ones > 0) result += " " + digits[ones];
            }
            else if (tens == 1)
            {
                result += " mười";
                if (ones == 1) result += " một";
                else if (ones == 5) result += " lăm";
                else if (ones > 0) result += " " + digits[ones];
            }
            else if (ones > 0)
            {
                // tens == 0
                result += " " + digits[ones]; 
            }
            
            return result.Trim();
        }
    }
}
