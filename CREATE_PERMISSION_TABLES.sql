-- =====================================================
-- Script tạo bảng Quyền và QuyenVaiTro cho hệ thống Admin
-- =====================================================

-- BƯỚC 0: XÓA CÁC BẢNG CŨ (NẾU TỒN TẠI)
PRINT 'Bắt đầu xóa các bảng cũ...';

-- Xóa bảng QuyenVaiTros (bảng con, phải xóa trước)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'QuyenVaiTros')
BEGIN
    DROP TABLE [dbo].[QuyenVaiTros];
    PRINT '✓ Đã xóa bảng QuyenVaiTros (cũ)';
END

-- Xóa bảng Quyens (bảng cha)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Quyens')
BEGIN
    DROP TABLE [dbo].[Quyens];
    PRINT '✓ Đã xóa bảng Quyens (cũ)';
END

-- Xóa bảng QuyenVaiTro (nếu đã tồn tại từ lần chạy trước)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'QuyenVaiTro')
BEGIN
    DROP TABLE [dbo].[QuyenVaiTro];
    PRINT '✓ Đã xóa bảng QuyenVaiTro (nếu có)';
END

-- Xóa bảng Quyen (nếu đã tồn tại từ lần chạy trước)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Quyen')
BEGIN
    DROP TABLE [dbo].[Quyen];
    PRINT '✓ Đã xóa bảng Quyen (nếu có)';
END

PRINT '';
PRINT 'Hoàn thành xóa bảng cũ. Bắt đầu tạo bảng mới...';
PRINT '';
GO

-- 1. Tạo bảng Quyen (Permissions) - TÊN BẢNG PHẢI KHỚP VỚI EF CONFIGURATION
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Quyen')
BEGIN
    CREATE TABLE [dbo].[Quyen] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaQuyen] NVARCHAR(100) NOT NULL,
        [TenQuyen] NVARCHAR(200) NOT NULL,
        [MoTa] NVARCHAR(500) NULL,
        [NhomQuyen] NVARCHAR(100) NOT NULL,
        [HanhDong] NVARCHAR(50) NOT NULL,
        [ThuTuSapXep] INT NOT NULL DEFAULT 0,
        [TrangThaiHoatDong] BIT NOT NULL DEFAULT 1,
        [NgayTao] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [NgayCapNhatCuoi] DATETIME2 NULL DEFAULT GETDATE(),
        [NguoiTao] INT NULL,
        [NguoiCapNhatCuoi] INT NULL
    );

    CREATE UNIQUE INDEX [IX_Quyen_MaQuyen] ON [dbo].[Quyen] ([MaQuyen]);

    PRINT '✓ Đã tạo bảng Quyen';
END
ELSE
BEGIN
    PRINT '⚠ Bảng Quyen đã tồn tại';
END
GO

-- 2. Tạo bảng QuyenVaiTro (Role-Permission mapping) - TÊN BẢNG PHẢI KHỚP VỚI EF CONFIGURATION
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'QuyenVaiTro')
BEGIN
    CREATE TABLE [dbo].[QuyenVaiTro] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaVaiTro] INT NOT NULL,
        [MaQuyen] INT NOT NULL,
        [NgayGan] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_QuyenVaiTro_VaiTroNguoiDung]
            FOREIGN KEY ([MaVaiTro]) REFERENCES [dbo].[VaiTroNguoiDung]([MaVaiTro]) ON DELETE CASCADE,
        CONSTRAINT [FK_QuyenVaiTro_Quyen]
            FOREIGN KEY ([MaQuyen]) REFERENCES [dbo].[Quyen]([Id]) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX [IX_QuyenVaiTro_MaVaiTro_MaQuyen] ON [dbo].[QuyenVaiTro] ([MaVaiTro], [MaQuyen]);
    CREATE INDEX [IX_QuyenVaiTro_MaVaiTro] ON [dbo].[QuyenVaiTro] ([MaVaiTro]);
    CREATE INDEX [IX_QuyenVaiTro_MaQuyen] ON [dbo].[QuyenVaiTro] ([MaQuyen]);

    PRINT '✓ Đã tạo bảng QuyenVaiTro';
END
ELSE
BEGIN
    PRINT '⚠ Bảng QuyenVaiTro đã tồn tại';
END
GO

-- 3. Cập nhật bảng VaiTroNguoiDung nếu thiếu cột NgayCapNhatCuoi
IF NOT EXISTS (SELECT * FROM sys.columns
               WHERE object_id = OBJECT_ID('VaiTroNguoiDung')
               AND name = 'NgayCapNhatCuoi')
BEGIN
    ALTER TABLE [dbo].[VaiTroNguoiDung]
    ADD [NgayCapNhatCuoi] DATETIME2 NULL;

    PRINT '✓ Đã thêm cột NgayCapNhatCuoi vào VaiTroNguoiDung';
END
GO

PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT '✅ Hoàn thành tạo bảng cho hệ thống phân quyền';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT '';
PRINT 'Các bước tiếp theo:';
PRINT '1. Chạy ứng dụng: dotnet run';
PRINT '2. Permissions sẽ được seed tự động (65+ quyền)';
PRINT '3. Tài khoản admin sẽ được tạo tự động';
PRINT '4. Truy cập: https://localhost:5001/Admin/Auth/Login';
PRINT '5. Đăng nhập với:';
PRINT '   - Username: admin';
PRINT '   - Password: Admin@123';
PRINT '';
GO
