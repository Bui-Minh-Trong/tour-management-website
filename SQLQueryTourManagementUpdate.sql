CREATE DATABASE QLyTourDuLich;
GO
USE QLyTourDuLich;
GO

-- =========================================================
-- PHẦN 1: NHÓM BẢNG HỆ THỐNG VÀ NHÂN SỰ
-- =========================================================

-- Bảng Vai trò (Để phân quyền trong ứng dụng)
CREATE TABLE VAI_TRO (
    MaVaiTro INT PRIMARY KEY IDENTITY(1,1),
    TenVaiTro NVARCHAR(50) NOT NULL UNIQUE 
);
GO

-- Bảng phân quyền
CREATE TABLE PHAN_QUYEN (
    MaVaiTro INT NOT NULL,
    MaChucNang VARCHAR(100) NOT NULL, 
    CoTheXem BIT NOT NULL DEFAULT 0,
    CoTheThem BIT NOT NULL DEFAULT 0,
    CoTheSua BIT NOT NULL DEFAULT 0,
    CoTheXoa BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (MaVaiTro, MaChucNang),
    FOREIGN KEY (MaVaiTro) REFERENCES VAI_TRO(MaVaiTro) ON DELETE CASCADE 
);
GO

-- Bảng Nhân viên
CREATE TABLE NHAN_VIEN (
    MaNV INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) NOT NULL,
    SDT VARCHAR(10) NOT NULL UNIQUE CHECK (PATINDEX('0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]', SDT) > 0),
    Email NVARCHAR(100) UNIQUE CHECK (PATINDEX('%_@_%._%', Email) > 0),
    NgayVaoLam DATE NOT NULL
);
GO

-- Bảng Tài khoản người dùng (Đã tối ưu kiểu MatKhau cho web MVC)
CREATE TABLE TAI_KHOAN (
    TenDangNhap VARCHAR(50) PRIMARY KEY,
    MatKhau VARCHAR(255) NOT NULL, -- Đổi thành VARCHAR để dễ lưu chuỗi mã hóa BCrypt/MD5 từ C#
    MaNV INT NOT NULL UNIQUE FOREIGN KEY REFERENCES NHAN_VIEN(MaNV),
    MaVaiTro INT NOT NULL FOREIGN KEY REFERENCES VAI_TRO(MaVaiTro),
    TrangThai BIT NOT NULL DEFAULT 1 
);
GO

-- =========================================================
-- PHẦN 2: NHÓM BẢNG KHÁCH HÀNG & KHUYẾN MÃI
-- =========================================================

CREATE TABLE KHACH_HANG (
    MaKH INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10) CHECK (GioiTinh IN (N'Nam', N'Nữ')),
    NgaySinh DATE,
    SDT VARCHAR(10) NOT NULL UNIQUE CHECK (PATINDEX('0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]', SDT) > 0),
    Email NVARCHAR(100) UNIQUE CHECK (PATINDEX('%_@_%._%', Email) > 0),
    CCCD VARCHAR(12) UNIQUE CHECK (PATINDEX('[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]', CCCD) > 0),
    DiaChi NVARCHAR(200)
);
GO

-- Bảng Khuyến mãi (Bổ sung để khớp với sơ đồ)
CREATE TABLE KHUYEN_MAI (
    MaKM INT PRIMARY KEY IDENTITY(1,1),
    Code VARCHAR(20) NOT NULL UNIQUE,
    PhanTramGiam DECIMAL(5,2) CHECK (PhanTramGiam > 0 AND PhanTramGiam <= 100),
    TienGiamTrucTiep DECIMAL(18,2) DEFAULT 0,
    NgayBatDau DATE NOT NULL,
    NgayHetHan DATE NOT NULL,
    SoLuongDaDung INT DEFAULT 0,
    CONSTRAINT CHK_NgayKM CHECK (NgayHetHan >= NgayBatDau)
);
GO

-- =========================================================
-- PHẦN 3: NHÓM BẢNG TOUR VÀ DỊCH VỤ LIÊN QUAN
-- =========================================================

-- Bảng Tour (Khuôn mẫu sản phẩm)
CREATE TABLE TOUR (
    MaTour INT PRIMARY KEY IDENTITY(1,1),
    TenTour NVARCHAR(100) NOT NULL,
    DiaDiem NVARCHAR(200) NOT NULL,
    ThoiGian INT NOT NULL CHECK (ThoiGian > 0), 
    PhuongTienChinh NVARCHAR(50) NOT NULL CHECK (PhuongTienChinh IN (N'Máy bay', N'Xe khách', N'Tàu hỏa', N'Tàu thủy')),
    MoTa NVARCHAR(MAX),
    GiaThamKhao DECIMAL(18,2) NOT NULL CHECK (GiaThamKhao > 0)
);
GO

-- Bảng Lịch trình chi tiết
CREATE TABLE LICH_TRINH (
    MaLichTrinh INT PRIMARY KEY IDENTITY(1,1),
    MaTour INT NOT NULL FOREIGN KEY REFERENCES TOUR(MaTour),
    SoNgay INT NOT NULL CHECK (SoNgay >= 1),
    DiaDiem NVARCHAR(100) NOT NULL,
    HoatDong NVARCHAR(MAX) NOT NULL
);
GO

-- Bảng Tài xế
CREATE TABLE TAI_XE (
    MaTaiXe INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    SDT VARCHAR(10) NOT NULL UNIQUE CHECK (PATINDEX('0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]', SDT) > 0),
    SoGPLX VARCHAR(12) NOT NULL UNIQUE,
    LoaiBangLai NVARCHAR(20) NOT NULL 
);
GO

-- Bảng Phương tiện
CREATE TABLE PHUONG_TIEN (
    MaPhuongTien INT PRIMARY KEY IDENTITY(1,1),
    BienSoXe VARCHAR(15) NOT NULL UNIQUE,
    LoaiXe NVARCHAR(50) NOT NULL, 
    SoChoNgoi INT CHECK (SoChoNgoi > 0),
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Hoạt động' CHECK (TrangThai IN (N'Hoạt động', N'Bảo trì', N'Hỏng hóc'))
);
GO

-- Bảng Khách sạn
CREATE TABLE KHACH_SAN (
    MaKS INT PRIMARY KEY IDENTITY(1,1),
    TenKS NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200) NOT NULL,
    SoSao INT NOT NULL CHECK (SoSao BETWEEN 1 AND 5),
    SDT VARCHAR(10) NOT NULL CHECK (PATINDEX('0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]', SDT) > 0)
);
GO

-- =========================================================
-- PHẦN 4: NHÓM BẢNG NGHIỆP VỤ VẬN HÀNH VÀ TÀI CHÍNH
-- =========================================================

-- Bảng Đoàn du lịch (Chuyến đi thực tế)
CREATE TABLE DOAN_DU_LICH (
    MaDoan INT PRIMARY KEY IDENTITY(1,1),
    TenDoan NVARCHAR(150) NOT NULL,
    MaTour INT NOT NULL FOREIGN KEY REFERENCES TOUR(MaTour),
    NgayKhoiHanh DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    MaNV_HuongDanVien INT NOT NULL FOREIGN KEY REFERENCES NHAN_VIEN(MaNV),
    SoLuongToiDa INT NOT NULL CHECK (SoLuongToiDa > 0),
    SoKhachHienTai INT NOT NULL DEFAULT 0 CHECK (SoKhachHienTai >= 0),
    TrangThaiDoan NVARCHAR(50) NOT NULL DEFAULT N'Lên kế hoạch' CHECK (TrangThaiDoan IN (N'Lên kế hoạch', N'Đang nhận khách', N'Đủ khách', N'Đã khởi hành', N'Đã kết thúc', N'Đã hủy')),
    CONSTRAINT CHK_NgayKetThuc_Doan CHECK (NgayKetThuc >= NgayKhoiHanh),
    CONSTRAINT CHK_SoKhach CHECK (SoKhachHienTai <= SoLuongToiDa)
);
GO

-- Bảng Phân công Vận chuyển
CREATE TABLE PHANCONG_VANCHUYEN (
    MaDoan INT NOT NULL,
    MaPhuongTien INT NOT NULL,
    MaTaiXe INT, 
    PRIMARY KEY (MaDoan, MaPhuongTien),
    FOREIGN KEY (MaDoan) REFERENCES DOAN_DU_LICH(MaDoan),
    FOREIGN KEY (MaPhuongTien) REFERENCES PHUONG_TIEN(MaPhuongTien),
    FOREIGN KEY (MaTaiXe) REFERENCES TAI_XE(MaTaiXe)
);
GO

-- Bảng Lưu trú
CREATE TABLE LUU_TRU (
    MaDoan INT NOT NULL,
    MaKS INT NOT NULL,
    NgayNhanPhong DATE NOT NULL,
    NgayTraPhong DATE NOT NULL,
    SoPhong INT NOT NULL DEFAULT 1 CHECK (SoPhong > 0),
    PRIMARY KEY (MaDoan, MaKS),
    FOREIGN KEY (MaDoan) REFERENCES DOAN_DU_LICH(MaDoan),
    FOREIGN KEY (MaKS) REFERENCES KHACH_SAN(MaKS),
    CONSTRAINT CHK_NgayTraPhong CHECK (NgayTraPhong >= NgayNhanPhong)
);
GO

-- Bảng Phiếu Đặt Tour (Đã thêm MaKM và đổi DATETIME)
CREATE TABLE KHACH_DI_TOUR (
    MaDangKy INT PRIMARY KEY IDENTITY(1,1),
    MaDoan INT NOT NULL FOREIGN KEY REFERENCES DOAN_DU_LICH(MaDoan),
    MaKH INT NOT NULL FOREIGN KEY REFERENCES KHACH_HANG(MaKH), 
    MaKM INT NULL FOREIGN KEY REFERENCES KHUYEN_MAI(MaKM), -- Bổ sung Khóa ngoại Khuyến mãi
    NgayDangKy DATETIME NOT NULL DEFAULT GETDATE(), -- Đổi thành DATETIME để lấy giờ phút giây
    SoNguoiDi INT NOT NULL DEFAULT 1 CHECK (SoNguoiDi > 0),
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia > 0), 
    SoTienGiam DECIMAL(18,2) DEFAULT 0 CHECK (SoTienGiam >= 0),
    TrangThaiThanhToan NVARCHAR(50) NOT NULL DEFAULT N'Chưa thanh toán' CHECK (TrangThaiThanhToan IN (N'Đã thanh toán', N'Chưa thanh toán', N'Đã cọc')),
    GhiChu NVARCHAR(255)
);
GO

-- Bảng Chi tiết hành khách
CREATE TABLE CHI_TIET_KHACH_DI_TOUR (
    MaDangKy INT NOT NULL FOREIGN KEY REFERENCES KHACH_DI_TOUR(MaDangKy),
    MaKH INT NOT NULL FOREIGN KEY REFERENCES KHACH_HANG(MaKH),
    LaNguoiDaiDien BIT NOT NULL DEFAULT 0, 
    PRIMARY KEY (MaDangKy, MaKH)
);
GO

-- Bảng Hóa đơn / Phiếu thu (Đổi DATETIME)
CREATE TABLE HOA_DON (
    MaHoaDon INT PRIMARY KEY IDENTITY(1,1),
    MaDangKy INT NOT NULL FOREIGN KEY REFERENCES KHACH_DI_TOUR(MaDangKy),
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(), -- Đổi thành DATETIME
    SoTien DECIMAL(18,2) NOT NULL CHECK (SoTien > 0), 
    LoaiHoaDon NVARCHAR(50) NOT NULL DEFAULT N'Toàn bộ' CHECK (LoaiHoaDon IN (N'Đặt cọc', N'Thanh toán nốt', N'Toàn bộ')),
    HinhThucTT NVARCHAR(50) NOT NULL CHECK (HinhThucTT IN (N'Tiền mặt', N'Chuyển khoản', N'Ví điện tử')),
    MaNV_Lap INT NOT NULL FOREIGN KEY REFERENCES NHAN_VIEN(MaNV)
);
GO

-- Bảng Chi phí / Phiếu chi (Đổi DATETIME)
CREATE TABLE CHI_PHI_DOAN (
    MaChiPhi INT PRIMARY KEY IDENTITY(1,1),
    MaDoan INT NOT NULL FOREIGN KEY REFERENCES DOAN_DU_LICH(MaDoan),
    NoiDungChi NVARCHAR(255) NOT NULL, 
    SoTien DECIMAL(18,2) NOT NULL CHECK (SoTien > 0),
    NgayChi DATETIME NOT NULL DEFAULT GETDATE(), -- Đổi thành DATETIME
    MaNV_Chi INT NOT NULL FOREIGN KEY REFERENCES NHAN_VIEN(MaNV)
);
GO