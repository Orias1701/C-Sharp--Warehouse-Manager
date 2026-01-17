USE QL_KhoHang;

-- =================================================================================
-- PHẦN 1: LÀM SẠCH VÀ TẠO DỮ LIỆU NỀN
-- =================================================================================

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE TransactionDetails;
TRUNCATE TABLE StockTransactions;
TRUNCATE TABLE Products;
TRUNCATE TABLE Categories;
SET FOREIGN_KEY_CHECKS = 1;

-- 1. Thêm 10 Danh mục
INSERT INTO Categories (CategoryName) VALUES 
('Điện thoại & Phụ kiện'), ('Máy tính & Laptop'), ('Thiết bị gia dụng'), 
('Thời trang Nam'), ('Thời trang Nữ'), ('Mỹ phẩm & Làm đẹp'), 
('Sách & Văn phòng phẩm'), ('Thể thao & Du lịch'), ('Đồ chơi & Mẹ bé'), ('Ô tô & Xe máy');

-- 2. Thêm 50 Sản phẩm
INSERT INTO Products (ProductName, CategoryID, Price, Quantity, MinThreshold, InventoryValue) VALUES 
('iPhone 15 Pro Max 1TB', 1, 45000000, 0, 5, 0), ('Samsung Galaxy S24 Ultra', 1, 30000000, 0, 5, 0),
('Xiaomi 14 Ultra', 1, 25000000, 0, 5, 0), ('Ốp lưng MagSafe Cao cấp', 1, 500000, 0, 20, 0),
('Sạc dự phòng Anker 20000mAh', 1, 1200000, 0, 10, 0), ('MacBook Pro 16 inch M3 Max', 2, 90000000, 0, 2, 0),
('Dell XPS 15 9530', 2, 55000000, 0, 3, 0), ('Màn hình LG UltraGear 27"', 2, 8500000, 0, 5, 0),
('Bàn phím cơ Keychron Q1', 2, 4500000, 0, 5, 0), ('Chuột Logitech MX Master 3S', 2, 2500000, 0, 10, 0),
('Tủ lạnh Hitachi Inverter 569L', 3, 35000000, 0, 2, 0), ('Máy giặt Electrolux 10kg', 3, 12000000, 0, 5, 0),
('Robot hút bụi Roborock S8', 3, 18000000, 0, 5, 0), ('Nồi chiên không dầu Philips', 3, 3500000, 0, 10, 0),
('Máy lọc không khí Dyson', 3, 15000000, 0, 5, 0), ('Áo sơ mi Pierre Cardin', 4, 1500000, 0, 20, 0),
('Quần âu Việt Tiến', 4, 800000, 0, 20, 0), ('Giày da nam Biti\'s Hunter', 4, 1200000, 0, 15, 0),
('Thắt lưng da cá sấu thật', 4, 5000000, 0, 5, 0), ('Đồng hồ Rolex Submariner Date', 4, 350000000, 0, 1, 0),
('Đầm dạ hội thiết kế', 5, 5000000, 0, 5, 0), ('Túi xách Hermes Birkin 30', 5, 850000000, 0, 1, 0),
('Giày cao gót Louboutin', 5, 25000000, 0, 2, 0), ('Áo dài lụa tơ tằm', 5, 8000000, 0, 5, 0),
('Kính mát Gucci chính hãng', 5, 10000000, 0, 5, 0), ('Son môi Tom Ford', 6, 1500000, 0, 20, 0),
('Nước hoa Chanel No.5 100ml', 6, 4500000, 0, 10, 0), ('Kem dưỡng ẩm La Mer', 6, 12000000, 0, 5, 0),
('Máy rửa mặt Foreo Luna 4', 6, 5000000, 0, 10, 0), ('Bộ trang điểm chuyên nghiệp MAC', 6, 8000000, 0, 5, 0),
('Bộ sách Harry Potter (Full)', 7, 3000000, 0, 5, 0), ('Bút ký Parker Sonnet', 7, 5000000, 0, 5, 0),
('Máy tính Casio FX-580VN X', 7, 800000, 0, 50, 0), ('Sổ tay Moleskine Classic', 7, 1200000, 0, 20, 0),
('Bàn cắt giấy công nghiệp A3', 7, 2500000, 0, 3, 0), ('Máy chạy bộ Kingsport BK', 8, 15000000, 0, 2, 0),
('Xe đạp địa hình Giant ATX', 8, 12000000, 0, 3, 0), ('Lều cắm trại 4 người Naturehike', 8, 3500000, 0, 5, 0),
('Vợt Tennis Wilson Pro Staff', 8, 4500000, 0, 5, 0), ('Vali Samsonite size L', 8, 10000000, 0, 5, 0),
('Xe đẩy em bé Combi', 9, 12000000, 0, 3, 0), ('Máy hút sữa Medela Pump', 9, 8000000, 0, 5, 0),
('Bộ LEGO Technic Bugatti Chiron', 9, 10000000, 0, 2, 0), ('Ghế ngồi ô tô cho bé Joie', 9, 6000000, 0, 5, 0),
('Nhà banh cầu trượt liên hoàn', 9, 2500000, 0, 5, 0), ('Xe máy Honda SH 350i', 10, 150000000, 0, 1, 0),
('Xe mô tô Ducati Panigale V4', 10, 890000000, 0, 1, 0), ('Mũ bảo hiểm AGV Pista GP', 10, 35000000, 0, 2, 0),
('Lốp xe Michelin Pilot Road 6', 10, 4500000, 0, 10, 0), ('Camera hành trình Vietmap SpeedMap', 10, 5000000, 0, 10, 0);

-- =====================================================
-- PHẦN 2: TẠO 200 GIAO DỊCH BẰNG STORED PROCEDURE
-- =====================================================

DROP PROCEDURE IF EXISTS GenerateTransactions;

DELIMITER $$

CREATE PROCEDURE GenerateTransactions()
BEGIN
    DECLARE i INT DEFAULT 0;
    DECLARE v_date DATETIME;
    DECLARE v_type VARCHAR(10);
    DECLARE v_note VARCHAR(100);
    DECLARE v_user INT;
    DECLARE v_tid INT;
    
    SET v_date = '2025-12-09 08:00:00';

    WHILE i < 200 DO
        IF i > 0 AND i % 5 = 0 THEN
            SET v_date = DATE_ADD(v_date, INTERVAL 1 DAY);
            SET v_date = DATE_ADD(DATE(v_date), INTERVAL 8 HOUR); 
        ELSE
            SET v_date = DATE_ADD(v_date, INTERVAL 2 HOUR);
        END IF;

        IF i % 2 = 0 THEN
            SET v_type = 'Import';
            SET v_note = CONCAT('Phiếu nhập tự động #', i + 1);
            SET v_user = 1; -- Admin nhập
        ELSE
            SET v_type = 'Export';
            SET v_note = CONCAT('Phiếu xuất tự động #', i + 1);
            SET v_user = 2; -- Staff xuất
        END IF;

        INSERT INTO StockTransactions (Type, DateCreated, CreatedByUserID, Note) 
        VALUES (v_type, v_date, v_user, v_note);
        
        SET v_tid = LAST_INSERT_ID();

        IF v_type = 'Import' THEN
            INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice)
            SELECT v_tid, ProductID, ProductName, FLOOR(5 + RAND() * 20), Price * 0.95
            FROM Products 
            ORDER BY RAND() 
            LIMIT 3;
            INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice)
            SELECT v_tid, ProductID, ProductName, FLOOR(1 + RAND() * 5), Price * 1.2
            FROM Products 
            ORDER BY RAND() 
            LIMIT 3;
        END IF;

        UPDATE StockTransactions 
        SET TotalValue = (SELECT SUM(Quantity * UnitPrice) FROM TransactionDetails WHERE TransactionID = v_tid)
        WHERE TransactionID = v_tid;

        SET i = i + 1;
    END WHILE;
END$$

DELIMITER ;

-- Gọi thủ tục để sinh dữ liệu
CALL GenerateTransactions();

-- Xóa thủ tục sau khi dùng xong
DROP PROCEDURE IF EXISTS GenerateTransactions;

-- =================================================================================
-- PHẦN 3: CẬP NHẬT TỒN KHO CUỐI CÙNG (CHẠY 1 LẦN DUY NHẤT)
-- =================================================================================

-- Tắt chế độ Safe Update để cho phép update nhiều dòng
SET SQL_SAFE_UPDATES = 0;

UPDATE Products p
SET 
    p.Quantity = (
        IFNULL((
            SELECT SUM(td.Quantity)
            FROM TransactionDetails td
            JOIN StockTransactions st ON td.TransactionID = st.TransactionID
            WHERE st.Type = 'Import' AND td.ProductID = p.ProductID
        ), 0)
        -
        IFNULL((
            SELECT SUM(td.Quantity)
            FROM TransactionDetails td
            JOIN StockTransactions st ON td.TransactionID = st.TransactionID
            WHERE st.Type = 'Export' AND td.ProductID = p.ProductID
        ), 0)
    ),
    p.InventoryValue = p.Quantity * p.Price;

-- Bật lại chế độ an toàn (tùy chọn)
SET SQL_SAFE_UPDATES = 1;