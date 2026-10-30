/* ============================================================
   BÀI TẬP 5.3 - 5.4 - 5.5
   Script tạo 3 cơ sở dữ liệu: CafePOSDB, HotelDB, InventoryDB
   Chạy toàn bộ file bằng SQL Server Management Studio.
   ============================================================ */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ============================================================
   1. CAFE POS DATABASE
   ============================================================ */
IF DB_ID(N'CafePOSDB') IS NULL
BEGIN
    CREATE DATABASE CafePOSDB;
END
GO
USE CafePOSDB;
GO

IF OBJECT_ID(N'dbo.BanAn', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BanAn
    (
        MaBan       INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_BanAn PRIMARY KEY,
        TenBan      NVARCHAR(50) NOT NULL
            CONSTRAINT UQ_BanAn_TenBan UNIQUE,
        TrangThai   NVARCHAR(20) NOT NULL
            CONSTRAINT DF_BanAn_TrangThai DEFAULT N'Trống'
            CONSTRAINT CK_BanAn_TrangThai CHECK (TrangThai IN (N'Trống', N'Có khách'))
    );
END
GO

IF OBJECT_ID(N'dbo.DanhMuc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhMuc
    (
        MaDanhMuc   INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_DanhMuc PRIMARY KEY,
        TenDanhMuc  NVARCHAR(100) NOT NULL
            CONSTRAINT UQ_DanhMuc_TenDanhMuc UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.MonAn', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MonAn
    (
        MaMon       INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_MonAn PRIMARY KEY,
        TenMon      NVARCHAR(100) NOT NULL,
        MaDanhMuc   INT NOT NULL,
        DonGia      DECIMAL(18,2) NOT NULL
            CONSTRAINT CK_MonAn_DonGia CHECK (DonGia >= 0),
        CONSTRAINT FK_MonAn_DanhMuc FOREIGN KEY (MaDanhMuc)
            REFERENCES dbo.DanhMuc(MaDanhMuc)
    );
END
GO

IF OBJECT_ID(N'dbo.HoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoaDon
    (
        MaHoaDon    INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_HoaDon PRIMARY KEY,
        MaBan       INT NOT NULL,
        GioVao      DATETIME NOT NULL
            CONSTRAINT DF_HoaDon_GioVao DEFAULT GETDATE(),
        GioRa       DATETIME NULL,
        TongTien    DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_HoaDon_TongTien DEFAULT 0
            CONSTRAINT CK_HoaDon_TongTien CHECK (TongTien >= 0),
        TrangThai   BIT NOT NULL
            CONSTRAINT DF_HoaDon_TrangThai DEFAULT 0, -- 0: chưa thanh toán, 1: đã thanh toán
        CONSTRAINT FK_HoaDon_BanAn FOREIGN KEY (MaBan)
            REFERENCES dbo.BanAn(MaBan),
        CONSTRAINT CK_HoaDon_GioRa CHECK (GioRa IS NULL OR GioRa >= GioVao)
    );
END
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_HoaDon_BanDangMo'
      AND object_id = OBJECT_ID(N'dbo.HoaDon')
)
BEGIN
    CREATE UNIQUE INDEX UX_HoaDon_BanDangMo
        ON dbo.HoaDon(MaBan)
        WHERE TrangThai = 0;
END
GO

IF OBJECT_ID(N'dbo.ChiTietHoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChiTietHoaDon
    (
        MaHoaDon    INT NOT NULL,
        MaMon       INT NOT NULL,
        SoLuong     INT NOT NULL
            CONSTRAINT CK_ChiTietHoaDon_SoLuong CHECK (SoLuong > 0),
        DonGia      DECIMAL(18,2) NOT NULL
            CONSTRAINT CK_ChiTietHoaDon_DonGia CHECK (DonGia >= 0),
        CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHoaDon, MaMon),
        CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (MaHoaDon)
            REFERENCES dbo.HoaDon(MaHoaDon) ON DELETE CASCADE,
        CONSTRAINT FK_ChiTietHoaDon_MonAn FOREIGN KEY (MaMon)
            REFERENCES dbo.MonAn(MaMon)
    );
END
GO

/* Dữ liệu mẫu CafePOSDB */
IF NOT EXISTS (SELECT 1 FROM dbo.BanAn)
BEGIN
    INSERT INTO dbo.BanAn (TenBan, TrangThai) VALUES
        (N'Bàn 01', N'Trống'),
        (N'Bàn 02', N'Trống'),
        (N'Bàn 03', N'Trống'),
        (N'Bàn 04', N'Trống'),
        (N'Bàn 05', N'Trống'),
        (N'Bàn 06', N'Trống'),
        (N'Bàn 07', N'Trống'),
        (N'Bàn 08', N'Trống'),
        (N'Bàn 09', N'Trống'),
        (N'Bàn 10', N'Trống'),
        (N'Bàn 11', N'Trống'),
        (N'Bàn 12', N'Trống');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DanhMuc)
BEGIN
    INSERT INTO dbo.DanhMuc (TenDanhMuc) VALUES
        (N'Cà phê'),
        (N'Trà sữa'),
        (N'Đồ ăn nhẹ'),
        (N'Nước giải khát');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MonAn)
BEGIN
    INSERT INTO dbo.MonAn (TenMon, MaDanhMuc, DonGia)
    SELECT v.TenMon, dm.MaDanhMuc, v.DonGia
    FROM (VALUES
        (N'Cà phê đen',        N'Cà phê',        25000),
        (N'Cà phê sữa đá',     N'Cà phê',        30000),
        (N'Bạc xỉu',           N'Cà phê',        35000),
        (N'Trà sữa trân châu', N'Trà sữa',       45000),
        (N'Trà sữa khoai môn', N'Trà sữa',       45000),
        (N'Trà đào cam sả',    N'Trà sữa',       40000),
        (N'Bánh mì chả',       N'Đồ ăn nhẹ',     30000),
        (N'Khoai tây chiên',   N'Đồ ăn nhẹ',     35000),
        (N'Bánh croissant',    N'Đồ ăn nhẹ',     28000),
        (N'Nước suối',         N'Nước giải khát',10000),
        (N'Coca Cola',         N'Nước giải khát',20000),
        (N'Nước cam ép',       N'Nước giải khát',35000)
    ) AS v(TenMon, TenDanhMuc, DonGia)
    JOIN dbo.DanhMuc AS dm ON dm.TenDanhMuc = v.TenDanhMuc;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.HoaDon)
BEGIN
    DECLARE @HoaDon03 INT;
    DECLARE @HoaDon05 INT;
    DECLARE @HoaDonPaid INT;

    -- Hóa đơn đang mở của Bàn 03
    INSERT INTO dbo.HoaDon (MaBan, GioVao, TongTien, TrangThai)
    SELECT MaBan, DATEADD(MINUTE, -45, GETDATE()), 0, 0
    FROM dbo.BanAn WHERE TenBan = N'Bàn 03';
    SET @HoaDon03 = SCOPE_IDENTITY();

    INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, MaMon, SoLuong, DonGia)
    SELECT @HoaDon03, m.MaMon, v.SoLuong, m.DonGia
    FROM (VALUES (N'Cà phê sữa đá', 2), (N'Bánh croissant', 1)) AS v(TenMon, SoLuong)
    JOIN dbo.MonAn AS m ON m.TenMon = v.TenMon;

    UPDATE dbo.BanAn SET TrangThai = N'Có khách' WHERE TenBan = N'Bàn 03';

    -- Hóa đơn đang mở của Bàn 05
    INSERT INTO dbo.HoaDon (MaBan, GioVao, TongTien, TrangThai)
    SELECT MaBan, DATEADD(MINUTE, -20, GETDATE()), 0, 0
    FROM dbo.BanAn WHERE TenBan = N'Bàn 05';
    SET @HoaDon05 = SCOPE_IDENTITY();

    INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, MaMon, SoLuong, DonGia)
    SELECT @HoaDon05, m.MaMon, v.SoLuong, m.DonGia
    FROM (VALUES (N'Trà sữa trân châu', 2), (N'Khoai tây chiên', 1)) AS v(TenMon, SoLuong)
    JOIN dbo.MonAn AS m ON m.TenMon = v.TenMon;

    UPDATE dbo.BanAn SET TrangThai = N'Có khách' WHERE TenBan = N'Bàn 05';

    -- Một hóa đơn đã thanh toán để minh họa lịch sử
    INSERT INTO dbo.HoaDon (MaBan, GioVao, GioRa, TongTien, TrangThai)
    SELECT MaBan, DATEADD(HOUR, -5, GETDATE()), DATEADD(HOUR, -4, GETDATE()), 90000, 1
    FROM dbo.BanAn WHERE TenBan = N'Bàn 01';
    SET @HoaDonPaid = SCOPE_IDENTITY();

    INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, MaMon, SoLuong, DonGia)
    SELECT @HoaDonPaid, m.MaMon, v.SoLuong, m.DonGia
    FROM (VALUES (N'Cà phê sữa đá', 2), (N'Bánh mì chả', 1)) AS v(TenMon, SoLuong)
    JOIN dbo.MonAn AS m ON m.TenMon = v.TenMon;
END
GO

/* ============================================================
   2. HOTEL DATABASE
   ============================================================ */
IF DB_ID(N'HotelDB') IS NULL
BEGIN
    CREATE DATABASE HotelDB;
END
GO
USE HotelDB;
GO

IF OBJECT_ID(N'dbo.Phong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Phong
    (
        SoPhong     VARCHAR(10) NOT NULL
            CONSTRAINT PK_Phong PRIMARY KEY,
        LoaiPhong   NVARCHAR(50) NOT NULL,
        GiaTheoGio  DECIMAL(18,2) NOT NULL
            CONSTRAINT CK_Phong_GiaTheoGio CHECK (GiaTheoGio >= 0),
        TrangThai   NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Phong_TrangThai DEFAULT N'Trống'
            CONSTRAINT CK_Phong_TrangThai CHECK (TrangThai IN (N'Trống', N'Đang ở', N'Đang dọn dẹp'))
    );
END
GO

IF OBJECT_ID(N'dbo.KhachHang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KhachHang
    (
        MaKH         INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_KhachHang PRIMARY KEY,
        HoTen        NVARCHAR(100) NOT NULL,
        CCCD         VARCHAR(20) NOT NULL
            CONSTRAINT UQ_KhachHang_CCCD UNIQUE,
        SoDienThoai  VARCHAR(15) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.DichVu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DichVu
    (
        MaDV    INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_DichVu PRIMARY KEY,
        TenDV   NVARCHAR(100) NOT NULL,
        DonGia  DECIMAL(18,2) NOT NULL
            CONSTRAINT CK_DichVu_DonGia CHECK (DonGia >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.PhieuDatPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhieuDatPhong
    (
        MaPhieu             INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_PhieuDatPhong PRIMARY KEY,
        SoPhong             VARCHAR(10) NOT NULL,
        MaKH                INT NOT NULL,
        NgayNhanPhong       DATETIME NOT NULL
            CONSTRAINT DF_PhieuDatPhong_NgayNhan DEFAULT GETDATE(),
        NgayTraPhong        DATETIME NULL,
        TongTienThanhToan   DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_PhieuDatPhong_TongTien DEFAULT 0
            CONSTRAINT CK_PhieuDatPhong_TongTien CHECK (TongTienThanhToan >= 0),
        CONSTRAINT FK_PhieuDatPhong_Phong FOREIGN KEY (SoPhong)
            REFERENCES dbo.Phong(SoPhong),
        CONSTRAINT FK_PhieuDatPhong_KhachHang FOREIGN KEY (MaKH)
            REFERENCES dbo.KhachHang(MaKH),
        CONSTRAINT CK_PhieuDatPhong_Ngay CHECK (NgayTraPhong IS NULL OR NgayTraPhong >= NgayNhanPhong)
    );

    -- Mỗi phòng chỉ có tối đa một phiếu đang lưu trú (chưa trả phòng)
    CREATE UNIQUE INDEX UX_PhieuDatPhong_PhongDangO
        ON dbo.PhieuDatPhong(SoPhong)
        WHERE NgayTraPhong IS NULL;
END
GO

IF OBJECT_ID(N'dbo.SuDungDichVu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SuDungDichVu
    (
        MaPhieu    INT NOT NULL,
        MaDV       INT NOT NULL,
        SoLuong    INT NOT NULL
            CONSTRAINT CK_SuDungDichVu_SoLuong CHECK (SoLuong > 0),
        ThoiGian   DATETIME NOT NULL
            CONSTRAINT DF_SuDungDichVu_ThoiGian DEFAULT GETDATE(),
        CONSTRAINT PK_SuDungDichVu PRIMARY KEY (MaPhieu, MaDV),
        CONSTRAINT FK_SuDungDichVu_Phieu FOREIGN KEY (MaPhieu)
            REFERENCES dbo.PhieuDatPhong(MaPhieu) ON DELETE CASCADE,
        CONSTRAINT FK_SuDungDichVu_DichVu FOREIGN KEY (MaDV)
            REFERENCES dbo.DichVu(MaDV)
    );
END
GO

/* Dữ liệu mẫu HotelDB */
IF NOT EXISTS (SELECT 1 FROM dbo.Phong)
BEGIN
    INSERT INTO dbo.Phong (SoPhong, LoaiPhong, GiaTheoGio, TrangThai) VALUES
        ('P101', N'Đơn', 120000, N'Trống'),
        ('P102', N'Đôi', 180000, N'Đang ở'),
        ('P103', N'VIP', 300000, N'Trống'),
        ('P201', N'Đơn', 130000, N'Trống'),
        ('P202', N'Đôi', 190000, N'Đang ở'),
        ('P203', N'VIP', 320000, N'Đang dọn dẹp'),
        ('P301', N'Đơn', 140000, N'Trống'),
        ('P302', N'Đôi', 200000, N'Trống'),
        ('P303', N'VIP', 350000, N'Trống'),
        ('P401', N'Đôi', 210000, N'Trống');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.KhachHang)
BEGIN
    INSERT INTO dbo.KhachHang (HoTen, CCCD, SoDienThoai) VALUES
        (N'Nguyễn Văn An', '001203004567', '0901234567'),
        (N'Trần Thị Bình', '079198007654', '0912345678'),
        (N'Lê Hoàng Cường', '031200009876', '0987654321');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DichVu)
BEGIN
    INSERT INTO dbo.DichVu (TenDV, DonGia) VALUES
        (N'Nước suối', 10000),
        (N'Cà phê', 25000),
        (N'Giặt ủi', 50000),
        (N'Suất ăn', 45000),
        (N'Bia lon', 20000);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PhieuDatPhong)
BEGIN
    DECLARE @MaKHAn INT = (SELECT MaKH FROM dbo.KhachHang WHERE CCCD = '001203004567');
    DECLARE @MaKHBinh INT = (SELECT MaKH FROM dbo.KhachHang WHERE CCCD = '079198007654');
    DECLARE @PhieuP102 INT;
    DECLARE @PhieuP202 INT;

    INSERT INTO dbo.PhieuDatPhong (SoPhong, MaKH, NgayNhanPhong, NgayTraPhong, TongTienThanhToan)
    VALUES ('P102', @MaKHAn, DATEADD(HOUR, -3, GETDATE()), NULL, 0);
    SET @PhieuP102 = SCOPE_IDENTITY();

    INSERT INTO dbo.PhieuDatPhong (SoPhong, MaKH, NgayNhanPhong, NgayTraPhong, TongTienThanhToan)
    VALUES ('P202', @MaKHBinh, DATEADD(MINUTE, -90, GETDATE()), NULL, 0);
    SET @PhieuP202 = SCOPE_IDENTITY();

    INSERT INTO dbo.SuDungDichVu (MaPhieu, MaDV, SoLuong, ThoiGian)
    SELECT @PhieuP102, dv.MaDV, v.SoLuong, DATEADD(MINUTE, -30, GETDATE())
    FROM (VALUES (N'Nước suối', 2), (N'Cà phê', 1)) AS v(TenDV, SoLuong)
    JOIN dbo.DichVu AS dv ON dv.TenDV = v.TenDV;

    INSERT INTO dbo.SuDungDichVu (MaPhieu, MaDV, SoLuong, ThoiGian)
    SELECT @PhieuP202, dv.MaDV, v.SoLuong, DATEADD(MINUTE, -20, GETDATE())
    FROM (VALUES (N'Suất ăn', 1)) AS v(TenDV, SoLuong)
    JOIN dbo.DichVu AS dv ON dv.TenDV = v.TenDV;
END
GO

/* ============================================================
   3. INVENTORY DATABASE
   ============================================================ */
IF DB_ID(N'InventoryDB') IS NULL
BEGIN
    CREATE DATABASE InventoryDB;
END
GO
USE InventoryDB;
GO

IF OBJECT_ID(N'dbo.NhaCungCap', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhaCungCap
    (
        MaNCC       INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_NhaCungCap PRIMARY KEY,
        TenNCC      NVARCHAR(150) NOT NULL,
        DienThoai   VARCHAR(15) NULL
            CONSTRAINT UQ_NhaCungCap_DienThoai UNIQUE,
        DiaChi      NVARCHAR(255) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.VatTu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.VatTu
    (
        MaVatTu     VARCHAR(20) NOT NULL
            CONSTRAINT PK_VatTu PRIMARY KEY,
        TenVatTu    NVARCHAR(150) NOT NULL,
        DVT         NVARCHAR(20) NOT NULL,
        SoLuongTon  DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_VatTu_SoLuongTon DEFAULT 0
            CONSTRAINT CK_VatTu_SoLuongTon CHECK (SoLuongTon >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.PhieuKho', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhieuKho
    (
        MaPhieu    INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_PhieuKho PRIMARY KEY,
        LoaiPhieu  NVARCHAR(10) NOT NULL
            CONSTRAINT CK_PhieuKho_LoaiPhieu CHECK (LoaiPhieu IN ('NHAP', 'XUAT')),
        NgayLap    DATETIME NOT NULL
            CONSTRAINT DF_PhieuKho_NgayLap DEFAULT GETDATE(),
        MaNCC      INT NULL,
        TongTien   DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_PhieuKho_TongTien DEFAULT 0
            CONSTRAINT CK_PhieuKho_TongTien CHECK (TongTien >= 0),
        CONSTRAINT FK_PhieuKho_NhaCungCap FOREIGN KEY (MaNCC)
            REFERENCES dbo.NhaCungCap(MaNCC),
        CONSTRAINT CK_PhieuKho_NhaCungCap CHECK
        (
            (LoaiPhieu = N'NHAP' AND MaNCC IS NOT NULL)
            OR (LoaiPhieu = N'XUAT' AND MaNCC IS NULL)
        )
    );
END
GO

IF OBJECT_ID(N'dbo.ChiTietPhieuKho', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChiTietPhieuKho
    (
        MaPhieu   INT NOT NULL,
        MaVatTu   VARCHAR(20) NOT NULL,
        SoLuong   DECIMAL(18,2) NOT NULL
            CONSTRAINT CK_ChiTietPhieuKho_SoLuong CHECK (SoLuong > 0),
        DonGia    DECIMAL(18,2) NOT NULL
            CONSTRAINT CK_ChiTietPhieuKho_DonGia CHECK (DonGia >= 0),
        CONSTRAINT PK_ChiTietPhieuKho PRIMARY KEY (MaPhieu, MaVatTu),
        CONSTRAINT FK_ChiTietPhieuKho_PhieuKho FOREIGN KEY (MaPhieu)
            REFERENCES dbo.PhieuKho(MaPhieu) ON DELETE CASCADE,
        CONSTRAINT FK_ChiTietPhieuKho_VatTu FOREIGN KEY (MaVatTu)
            REFERENCES dbo.VatTu(MaVatTu)
    );
END
GO

/* Dữ liệu mẫu InventoryDB */
IF NOT EXISTS (SELECT 1 FROM dbo.NhaCungCap)
BEGIN
    INSERT INTO dbo.NhaCungCap (TenNCC, DienThoai, DiaChi) VALUES
        (N'Công ty TNHH Minh Anh', '0901234567', N'Số 12 Nguyễn Trãi, Hà Nội'),
        (N'Công ty Vật tư Hòa Bình', '0912345678', N'45 Lê Lợi, Đà Nẵng'),
        (N'Thực phẩm Sài Gòn', '0987654321', N'88 Hai Bà Trưng, TP. Hồ Chí Minh');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.VatTu)
BEGIN
    INSERT INTO dbo.VatTu (MaVatTu, TenVatTu, DVT, SoLuongTon) VALUES
        ('VT001', N'Gạo ST25', 'Kg', 120),
        ('VT002', N'Đường cát trắng', 'Kg', 8),
        ('VT003', N'Cà phê hạt rang', 'Kg', 15),
        ('VT004', N'Sữa đặc', 'Thùng', 4),
        ('VT005', N'Ly nhựa', 'Cái', 500),
        ('VT006', N'Trà đen', 'Kg', 30),
        ('VT007', N'Bột cacao', 'Kg', 6),
        ('VT008', N'Khăn giấy', 'Thùng', 2);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PhieuKho)
BEGIN
    DECLARE @PhieuNhap INT;
    DECLARE @PhieuXuat INT;

    INSERT INTO dbo.PhieuKho (LoaiPhieu, NgayLap, MaNCC, TongTien)
    SELECT 'NHAP', DATEADD(DAY, -2, GETDATE()), MaNCC, 1640000
    FROM dbo.NhaCungCap WHERE TenNCC = N'Công ty Vật tư Hòa Bình';
    SET @PhieuNhap = SCOPE_IDENTITY();

    INSERT INTO dbo.ChiTietPhieuKho (MaPhieu, MaVatTu, SoLuong, DonGia) VALUES
        (@PhieuNhap, 'VT002', 20, 22000),
        (@PhieuNhap, 'VT007', 10, 120000);

    INSERT INTO dbo.PhieuKho (LoaiPhieu, NgayLap, MaNCC, TongTien)
    VALUES ('XUAT', DATEADD(DAY, -1, GETDATE()), NULL, 620000);
    SET @PhieuXuat = SCOPE_IDENTITY();

    INSERT INTO dbo.ChiTietPhieuKho (MaPhieu, MaVatTu, SoLuong, DonGia) VALUES
        (@PhieuXuat, 'VT003', 5, 100000),
        (@PhieuXuat, 'VT004', 1, 120000);
END
GO

PRINT N'Đã tạo xong 3 cơ sở dữ liệu CafePOSDB, HotelDB, InventoryDB.';
GO
