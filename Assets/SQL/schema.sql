DROP DATABASE IF EXISTS QL_KhoHang;
CREATE DATABASE IF NOT EXISTS QL_KhoHang CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE QL_KhoHang;

-- 0. Người dùng (Phân quyền)
CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã người dùng',
    Username VARCHAR(50) NOT NULL UNIQUE COMMENT 'Tên đăng nhập',
    Password VARCHAR(255) NOT NULL COMMENT 'Mật khẩu',
    FullName VARCHAR(100) COMMENT 'Họ tên',
    Role ENUM('Admin', 'Staff') DEFAULT 'Staff' COMMENT 'Quyền: Admin/Staff',
    IsActive BOOLEAN DEFAULT TRUE COMMENT 'Trạng thái hoạt động',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo'
) COMMENT = 'Người dùng';

-- 1. Danh mục
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã danh mục',
    CategoryName VARCHAR(100) NOT NULL COMMENT 'Tên danh mục'
) COMMENT = 'Danh mục SP';

-- 2. Sản phẩm
CREATE TABLE Products (
    ProductID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã SP',
    ProductName VARCHAR(255) NOT NULL COMMENT 'Tên SP',
    CategoryID INT COMMENT 'Mã danh mục',
    Price DECIMAL(18, 2) DEFAULT 0 COMMENT 'Giá bán',
    Quantity INT DEFAULT 0 COMMENT 'Tồn kho',
    MinThreshold INT DEFAULT 10 COMMENT 'Ngưỡng báo',
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
) COMMENT = 'Hàng hóa';

-- 3. Phiếu Nhập/Xuất
CREATE TABLE StockTransactions (
    TransactionID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã phiếu',
    Type ENUM('Import', 'Export') NOT NULL COMMENT 'Loại: Nhập/Xuất',
    DateCreated DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày lập',
    CreatedByUserID INT COMMENT 'Người tạo phiếu',
    Note TEXT COMMENT 'Ghi chú',
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
) COMMENT = 'Phiếu kho';

-- 4. Chi tiết phiếu
CREATE TABLE TransactionDetails (
    DetailID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã CT',
    TransactionID INT COMMENT 'Mã phiếu',
    ProductID INT COMMENT 'Mã SP',
    ProductName VARCHAR(255) COMMENT 'Tên SP (snapshot)',
    Quantity INT NOT NULL COMMENT 'Số lượng',
    UnitPrice DECIMAL(18, 2) COMMENT 'Đơn giá',
    FOREIGN KEY (TransactionID) REFERENCES StockTransactions(TransactionID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
) COMMENT = 'Chi tiết phiếu';

-- 5. Nhật ký (Hỗ trợ Undo)
CREATE TABLE ActionLogs (
    LogID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Mã log',
    ActionType VARCHAR(50) COMMENT 'Hành động',
    Descriptions TEXT COMMENT 'Mô tả',
    DataBefore JSON COMMENT 'Dữ liệu cũ',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian'
) COMMENT = 'Nhật ký';

-- SEED DATA
INSERT INTO Categories (CategoryName) VALUES 
('Thực phẩm'),
('Điện tử'),
('Quần áo'),
('Khác');

-- Seed default users (password: 123, 456)
-- admin: username=admin, password=123
-- staff: username=staff, password=456
INSERT INTO Users (Username, Password, FullName, Role) VALUES
('admin', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'Quản trị viên', 'Admin'),
('staff', '8d969eef6ecad3c29a3a873fba6aa3285c080e8ad31a9ef313c52f53f7f0df9c', 'Nhân viên kho', 'Staff');
