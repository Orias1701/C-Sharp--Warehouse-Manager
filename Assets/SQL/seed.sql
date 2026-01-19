USE QL_KhoHang;

-- =================================================================================
-- PHẦN 1: LÀM SẠCH VÀ TẠO DỮ LIỆU NỀN
-- =================================================================================

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE TransactionDetails;
TRUNCATE TABLE Transactions;
TRUNCATE TABLE InventoryCheckDetails;
TRUNCATE TABLE InventoryChecks;
TRUNCATE TABLE Suppliers;
TRUNCATE TABLE Customers;
TRUNCATE TABLE Products;
TRUNCATE TABLE Categories;
TRUNCATE TABLE Users;

SET FOREIGN_KEY_CHECKS = 1;

-- Seed Users
INSERT INTO Users (Username, Password, FullName, Role) VALUES
('admin', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'Quản trị viên', 'Admin'),
('staff', '8d969eef6ecad3c29a3a873fba6aa3285c080e8ad31a9ef313c52f53f7f0df9c', 'Nhân viên kho', 'Staff');

-- Thêm dữ liệu Nhà Cung Cấp
INSERT INTO Suppliers (SupplierName, Phone, Address, Email) VALUES 
('Apple Distribution VN', '0909123456', 'Q1, TP.HCM', 'contact@apple.vn'),
('Samsung Vina', '0909888999', 'KCN Yên Bình, Thái Nguyên', 'sales@samsung.com'),
('Công ty May Việt Tiến', '02838640800', 'Tân Bình, TP.HCM', 'viettien@corp.vn'),
('Honda Việt Nam', '18008001', 'Phúc Thắng, Vĩnh Phúc', 'cr@honda.com.vn'),
('Nhà Phân Phối Digiworld', '02839290059', 'Q3, TP.HCM', 'support@dgw.com.vn');

-- Thêm dữ liệu Khách Hàng
INSERT INTO Customers (CustomerName, Phone, Address, Email) VALUES 
('Nguyễn Văn A', '0912345678', 'Ba Đình, Hà Nội', 'anv@gmail.com'),
('Trần Thị B', '0987654321', 'Cầu Giấy, Hà Nội', 'btran@yahoo.com'),
('Công ty TNHH ABC', '0243777888', 'Đống Đa, Hà Nội', 'admin@abc.com'),
('Nguyễn Văn An', '0912345678', 'Ba Đình, Hà Nội', 'anv@gmail.com'),
('Trần Thị Bích', '0987654321', 'Cầu Giấy, Hà Nội', 'btran@yahoo.com'),
('Công ty TNHH Thương Mại ABC', '0243777888', 'Đống Đa, Hà Nội', 'admin@abc.com'),
('Lê Hoàng Nam', '0933444555', 'Hải Châu, Đà Nẵng', 'namlh@outlook.com');

-- Thêm 10 Danh mục
INSERT INTO Categories (CategoryName, Description) VALUES 
('Điện thoại & Phụ kiện', 'Điện thoại thông minh, phụ kiện, sạc, cáp, ốp lưng, kính cường lực'),
('Máy tính & Laptop', 'Máy tính xách tay, máy tính để bàn, thiết bị ngoại vi, màn hình, bàn phím, chuột'),
('Thiết bị gia dụng', 'Tủ lạnh, máy giặt, máy hút bụi, nồi chiên không dầu, máy lọc không khí'),
('Thời trang Nam', 'Áo sơ mi, quần âu, giày da, thắt lưng, đồng hồ, phụ kiện thời trang nam'),
('Thời trang Nữ', 'Đầm, áo khoác, quần tây, giày cao gót, túi xách, kính mát, phụ kiện nữ'),
('Mỹ phẩm & Làm đẹp', 'Son môi, nước hoa, kem dưỡng, máy rửa mặt, bộ trang điểm chuyên nghiệp'),
('Sách & Văn phòng phẩm', 'Sách, truyện, bút ký, máy tính cầm tay, sổ tay, bàn cắt giấy'),
('Thể thao & Du lịch', 'Máy chạy bộ, xe đạp, lều cắm trại, vợt tennis, vali, ba lô du lịch'),
('Đồ chơi & Mẹ bé', 'Đồ chơi LEGO, xe đẩy em bé, máy hút sữa, ghế ngồi trẻ em, nhà banh'),
('Ô tô & Xe máy', 'Xe máy, mũ bảo hiểm, lốp xe, camera hành trình, phụ kiện ô tô');

-- Thêm 50 Sản phẩm
INSERT INTO Products (ProductName, CategoryID, Price, Quantity, MinThreshold, InventoryValue) VALUES 
('iPhone 15 Pro Max 1TB', 1, 45000000, 999, 5, 0), 
('Samsung Galaxy S24 Ultra', 1, 30000000, 999, 5, 0),
('Xiaomi 14 Ultra', 1, 25000000, 999, 5, 0), 
('Ốp lưng MagSafe Cao cấp', 1, 500000, 999, 20, 0),
('Sạc dự phòng Anker 20000mAh', 1, 1200000, 999, 10, 0), 
('MacBook Pro 16 inch M3 Max', 2, 90000000, 999, 2, 0),
('Dell XPS 15 9530', 2, 55000000, 999, 3, 0), 
('Màn hình LG UltraGear 27"', 2, 8500000, 999, 5, 0),
('Bàn phím cơ Keychron Q1', 2, 4500000, 999, 5, 0), 
('Chuột Logitech MX Master 3S', 2, 2500000, 999, 10, 0),
('Tủ lạnh Hitachi Inverter 569L', 3, 35000000, 999, 2, 0), 
('Máy giặt Electrolux 10kg', 3, 12000000, 999, 5, 0),
('Robot hút bụi Roborock S8', 3, 18000000, 999, 5, 0), 
('Nồi chiên không dầu Philips', 3, 3500000, 999, 10, 0),
('Máy lọc không khí Dyson', 3, 15000000, 999, 5, 0), 
('Áo sơ mi Pierre Cardin', 4, 1500000, 999, 20, 0),
('Quần âu Việt Tiến', 4, 800000, 999, 20, 0), 
('Giày da nam Bitis Hunter', 4, 1200000, 999, 15, 0),
('Thắt lưng da cá sấu thật', 4, 5000000, 999, 5, 0), 
('Đồng hồ Rolex Submariner Date', 4, 350000000, 999, 1, 0),
('Đầm dạ hội thiết kế', 5, 5000000, 999, 5, 0), 
('Túi xách Hermes Birkin 30', 5, 850000000, 999, 1, 0),
('Giày cao gót Louboutin', 5, 25000000, 999, 2, 0), 
('Áo dài lụa tơ tằm', 5, 8000000, 999, 5, 0),
('Kính mát Gucci chính hãng', 5, 10000000, 999, 5, 0), 
('Son môi Tom Ford', 6, 1500000, 999, 20, 0),
('Nước hoa Chanel No.5 100ml', 6, 4500000, 999, 10, 0), 
('Kem dưỡng ẩm La Mer', 6, 12000000, 999, 5, 0),
('Máy rửa mặt Foreo Luna 4', 6, 5000000, 999, 10, 0), 
('Bộ trang điểm chuyên nghiệp MAC', 6, 8000000, 999, 5, 0),
('Bộ sách Harry Potter (Full)', 7, 3000000, 999, 5, 0), 
('Bút ký Parker Sonnet', 7, 5000000, 999, 5, 0),
('Máy tính Casio FX-580VN X', 7, 800000, 999, 50, 0), 
('Sổ tay Moleskine Classic', 7, 1200000, 999, 20, 0),
('Bàn cắt giấy công nghiệp A3', 7, 2500000, 999, 3, 0), 
('Máy chạy bộ Kingsport BK', 8, 15000000, 999, 2, 0),
('Xe đạp địa hình Giant ATX', 8, 12000000, 999, 3, 0), 
('Lều cắm trại 4 người Naturehike', 8, 3500000, 999, 5, 0),
('Vợt Tennis Wilson Pro Staff', 8, 4500000, 999, 5, 0), 
('Vali Samsonite size L', 8, 10000000, 999, 5, 0),
('Xe đẩy em bé Combi', 9, 12000000, 999, 3, 0), 
('Máy hút sữa Medela Pump', 9, 8000000, 999, 5, 0),
('Bộ LEGO Technic Bugatti Chiron', 9, 10000000, 999, 2, 0), 
('Ghế ngồi ô tô cho bé Joie', 9, 6000000, 999, 5, 0),
('Nhà banh cầu trượt liên hoàn', 9, 2500000, 999, 5, 0), 
('Xe máy Honda SH 350i', 10, 150000000, 999, 1, 0),
('Xe mô tô Ducati Panigale V4', 10, 890000000, 999, 1, 0), 
('Mũ bảo hiểm AGV Pista GP', 10, 35000000, 999, 2, 0),
('Lốp xe Michelin Pilot Road 6', 10, 4500000, 999, 10, 0), 
('Camera hành trình Vietmap SpeedMap', 10, 5000000, 999, 10, 0);


-- =================================================================================
-- PHẦN 2: TẠO 200 GIAO DỊCH BẰNG STORED PROCEDURE
-- =================================================================================

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
    DECLARE v_supplier INT;
    DECLARE v_customer INT;
    DECLARE v_total DECIMAL(18,2);
    DECLARE v_discount DECIMAL(18,2);
    
    SET v_date = '2025-12-09 08:00:00';

    WHILE i < 200 DO
        IF i > 0 AND i % 5 = 0 THEN
            SET v_date = DATE_ADD(v_date, INTERVAL 1 DAY);
            SET v_date = DATE_ADD(DATE(v_date), INTERVAL 8 HOUR);
        ELSE
            SET v_date = DATE_ADD(v_date, INTERVAL 2 HOUR);
        END IF;

        SET v_supplier = NULL;
        SET v_customer = NULL;

        IF i % 2 = 0 THEN
            SET v_type = 'Import';
            SET v_note = CONCAT('Phiếu nhập tự động #', i + 1);
            SET v_user = 1;
            SET v_supplier = FLOOR(1 + RAND() * 5);
        ELSE
            SET v_type = 'Export';
            SET v_note = CONCAT('Phiếu xuất tự động #', i + 1);
            SET v_user = 2;
            SET v_customer = FLOOR(1 + RAND() * 4);
        END IF;

        INSERT INTO Transactions (Type, DateCreated, CreatedByUserID, Note, SupplierID, CustomerID, TotalAmount, Discount, FinalAmount) 
        VALUES (v_type, v_date, v_user, v_note, v_supplier, v_customer, 0, 0, 0);
        SET v_tid = LAST_INSERT_ID();

        INSERT INTO TransactionDetails (TransactionID, ProductID, ProductName, Quantity, UnitPrice)
        SELECT v_tid, ProductID, ProductName, FLOOR(5 + RAND() * 20), Price
        FROM Products 
        ORDER BY RAND() 
        LIMIT 3;

        SELECT SUM(Quantity * UnitPrice) INTO v_total 
        FROM TransactionDetails 
        WHERE TransactionID = v_tid;

        IF (RAND() < 0.3) THEN
            SET v_discount = v_total * (RAND() * 0.05);
        ELSE
            SET v_discount = 0;
        END IF;

        SET v_total = ROUND(v_total, -3);
        SET v_discount = ROUND(v_discount, -3);
        UPDATE Transactions 
        SET TotalAmount = v_total,
            Discount = v_discount,
            FinalAmount = v_total - v_discount
        WHERE TransactionID = v_tid;
        SET i = i + 1;
    END WHILE;
END$$
DELIMITER ;
CALL GenerateTransactions();


-- =================================================================================
-- PHẦN 3: CẬP NHẬT TỒN KHO CUỐI CÙNG
-- =================================================================================

DROP PROCEDURE IF EXISTS StockUpdate;
DELIMITER $$
CREATE PROCEDURE StockUpdate()
BEGIN
    SET SQL_SAFE_UPDATES = 0;

    UPDATE Products p
    SET 
        p.Quantity = (
            IFNULL((
                SELECT SUM(td.Quantity)
                FROM TransactionDetails td
                JOIN Transactions st ON td.TransactionID = st.TransactionID
                WHERE st.Type = 'Import' AND td.ProductID = p.ProductID
            ), 0)
            -
            IFNULL((
                SELECT SUM(td.Quantity)
                FROM TransactionDetails td
                JOIN Transactions st ON td.TransactionID = st.TransactionID
                WHERE st.Type = 'Export' AND td.ProductID = p.ProductID
            ), 0)
        ),
        p.Quantity = (CASE 
            WHEN p.Quantity < 0 THEN p.Quantity * (-1) 
            ELSE p.Quantity 
        END),
        p.InventoryValue = p.Quantity * p.Price;
    SET SQL_SAFE_UPDATES = 1;
END$$
DELIMITER ;
CALL StockUpdate();


-- =================================================================================
-- PHẦN 4: SEED DATA KIỂM KÊ
-- =================================================================================

DROP PROCEDURE IF EXISTS SeedInventoryChecks;
DELIMITER $$
CREATE PROCEDURE SeedInventoryChecks()
BEGIN
    DECLARE v_checkID INT;
    INSERT INTO InventoryChecks (CheckDate, CreatedByUserID, Status, Note)
    VALUES ('2025-12-31 17:00:00', 1, 'Completed', 'Kiểm kê chốt sổ cuối năm 2025');
    
    SET v_checkID = LAST_INSERT_ID();

    INSERT INTO InventoryCheckDetails (CheckID, ProductID, SystemQuantity, ActualQuantity, Reason)
    SELECT v_checkID, ProductID, Quantity, Quantity, 'Khớp số liệu cuối năm'
    FROM Products ORDER BY RAND() LIMIT 5;

    INSERT INTO InventoryChecks (CheckDate, CreatedByUserID, Status, Note)
    VALUES ('2026-01-19 10:00:00', 1, 'Completed', 'Kiểm kê định kỳ Tháng 01/2026');
    
    SET v_checkID = LAST_INSERT_ID();
    
    INSERT INTO InventoryCheckDetails (CheckID, ProductID, SystemQuantity, ActualQuantity, Reason)
    SELECT v_checkID, ProductID, Quantity, Quantity, 'Khớp số liệu định kỳ'
    FROM Products ORDER BY RAND() LIMIT 10;

END$$
DELIMITER ;
CALL SeedInventoryChecks();