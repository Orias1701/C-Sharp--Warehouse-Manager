DROP DATABASE IF EXISTS QL_KhoHang;
CREATE DATABASE IF NOT EXISTS QL_KhoHang CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE QL_KhoHang;

-- =====================================================
-- 1. NHÓM ĐỐI TƯỢNG (MỚI)
-- =====================================================

-- 1.1 Nhà cung cấp
CREATE TABLE Suppliers (
    SupplierID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã NCC',
    SupplierName VARCHAR(255) NOT NULL COMMENT 'Tên NCC',
    Phone VARCHAR(20) COMMENT 'SĐT',
    Address TEXT COMMENT 'Địa chỉ',
    Email VARCHAR(100) COMMENT 'Email',
    Visible BOOLEAN DEFAULT TRUE COMMENT 'Hiển thị'
) COMMENT = 'Nhà cung cấp';

-- 1.2 Khách hàng
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã KH',
    CustomerName VARCHAR(255) NOT NULL COMMENT 'Tên KH',
    Phone VARCHAR(20) COMMENT 'SĐT',
    Address TEXT COMMENT 'Địa chỉ',
    Email VARCHAR(100) COMMENT 'Email',
    Visible BOOLEAN DEFAULT TRUE COMMENT 'Hiển thị'
) COMMENT = 'Khách hàng';

-- =====================================================
-- 2. NHÓM QUẢN TRỊ & HÀNG HÓA
-- =====================================================

-- 2.1 Người dùng
CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã người dùng',
    Username VARCHAR(50) NOT NULL UNIQUE COMMENT 'Tên đăng nhập',
    Password VARCHAR(255) NOT NULL COMMENT 'Mật khẩu',
    FullName VARCHAR(100) COMMENT 'Họ tên',
    Role ENUM('Admin', 'Staff') DEFAULT 'Staff' COMMENT 'Quyền',
    IsActive BOOLEAN DEFAULT TRUE COMMENT 'Trạng thái',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo',
    Visible BOOLEAN DEFAULT TRUE COMMENT 'Hiển thị'
) COMMENT = 'Người dùng';

-- 2.2 Danh mục
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã danh mục',
    CategoryName VARCHAR(100) NOT NULL COMMENT 'Tên danh mục',
    Description TEXT COMMENT 'Mô tả',
    Visible BOOLEAN DEFAULT TRUE COMMENT 'Hiển thị'
) COMMENT = 'Danh mục SP';

-- 2.3 Sản phẩm
CREATE TABLE Products (
    ProductID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã SP',
    ProductName VARCHAR(255) NOT NULL COMMENT 'Tên SP',
    CategoryID INT COMMENT 'Mã danh mục',
    Price DECIMAL(18, 2) DEFAULT 0 COMMENT 'Giá bán hiện tại',
    Quantity INT DEFAULT 0 COMMENT 'Tồn kho',
    MinThreshold INT DEFAULT 10 COMMENT 'Ngưỡng báo động',
    InventoryValue DECIMAL(18, 2) DEFAULT 0 COMMENT 'Giá trị tồn kho',
    Visible BOOLEAN DEFAULT TRUE COMMENT 'Hiển thị',
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
) COMMENT = 'Hàng hóa';

-- =====================================================
-- 3. NHÓM GIAO DỊCH (CẬP NHẬT)
-- =====================================================

-- 3.1 Phiếu Nhập/Xuất (Header)
CREATE TABLE Transactions (
    TransactionID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã phiếu',
    Type ENUM('Import', 'Export') NOT NULL COMMENT 'Loại phiếu',
    DateCreated DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày lập',
    CreatedByUserID INT COMMENT 'Người lập phiếu',
    -- CẬP NHẬT MỚI: Đối tượng giao dịch
    SupplierID INT NULL COMMENT 'Nhà cung cấp (Nếu là Nhập)',
    CustomerID INT NULL COMMENT 'Khách hàng (Nếu là Xuất)',
    -- CẬP NHẬT MỚI: Tài chính
    TotalAmount DECIMAL(18, 2) DEFAULT 0 COMMENT 'Tổng tiền hàng',
    Discount DECIMAL(18, 2) DEFAULT 0 COMMENT 'Chiết khấu (VNĐ)',
    FinalAmount DECIMAL(18, 2) DEFAULT 0 COMMENT 'Thành tiền sau CK',
    Note TEXT COMMENT 'Ghi chú',
    Visible BOOLEAN DEFAULT TRUE COMMENT 'Hiển thị',
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
) COMMENT = 'Phiếu kho';

-- 3.2 Chi tiết phiếu (Details)
CREATE TABLE TransactionDetails (
    DetailID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã CT',
    TransactionID INT COMMENT 'Mã phiếu',
    ProductID INT COMMENT 'Mã SP',
    ProductName VARCHAR(255) COMMENT 'Tên SP snapshot',
    Quantity INT NOT NULL COMMENT 'Số lượng',
    UnitPrice DECIMAL(18, 2) COMMENT 'Đơn giá tại thời điểm GD',
    SubTotal DECIMAL(18, 2) GENERATED ALWAYS AS (Quantity * UnitPrice) STORED COMMENT 'Thành tiền dòng',
    Visible BOOLEAN DEFAULT TRUE COMMENT 'Hiển thị',
    FOREIGN KEY (TransactionID) REFERENCES Transactions(TransactionID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
) COMMENT = 'Chi tiết phiếu';

-- =====================================================
-- 4. NHÓM KIỂM KÊ (MỚI)
-- =====================================================

-- 4.1 Phiếu kiểm kê
CREATE TABLE InventoryChecks (
    CheckID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã phiếu kiểm',
    CheckDate DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày kiểm',
    CreatedByUserID INT COMMENT 'Người kiểm',
    Status ENUM('Pending', 'Completed', 'Cancelled') DEFAULT 'Pending' COMMENT 'Trạng thái',
    Note TEXT COMMENT 'Ghi chú',
    Visible BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
) COMMENT = 'Phiếu kiểm kê';

-- 4.2 Chi tiết kiểm kê
CREATE TABLE InventoryCheckDetails (
    DetailID INT PRIMARY KEY AUTO_INCREMENT,
    CheckID INT,
    ProductID INT,
    SystemQuantity INT DEFAULT 0 COMMENT 'Tồn trên phần mềm',
    ActualQuantity INT DEFAULT 0 COMMENT 'Tồn thực tế đếm được',
    Difference INT GENERATED ALWAYS AS (ActualQuantity - SystemQuantity) STORED COMMENT 'Chênh lệch',
    Reason VARCHAR(255) COMMENT 'Lý do chênh lệch',
    FOREIGN KEY (CheckID) REFERENCES InventoryChecks(CheckID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
) COMMENT = 'Chi tiết kiểm kê';