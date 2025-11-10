using BilliardShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilliardShop.Infrastructure.Data.SeedData;

/// <summary>
/// Class để seed dữ liệu phân quyền cho các vai trò
/// </summary>
public static class RolePermissionSeeder
{
    public static async Task SeedRolePermissionsAsync(BilliardShopDbContext context)
    {
        // Kiểm tra xem đã có phân quyền chưa
        if (await context.QuyenVaiTros.AnyAsync())
        {
            return; // Đã có dữ liệu rồi
        }

        // Lấy tất cả quyền
        var allPermissions = await context.Quyens.ToListAsync();
        if (!allPermissions.Any())
        {
            return; // Chưa có quyền nào
        }

        var rolePermissions = new List<QuyenVaiTro>();

        // 1. Gán TOÀN BỘ quyền cho QuanTriVien (ID = 1)
        foreach (var permission in allPermissions)
        {
            rolePermissions.Add(new QuyenVaiTro
            {
                MaVaiTro = 1, // QuanTriVien
                MaQuyen = permission.Id,
                NgayGan = DateTime.Now
            });
        }

        // 2. Gán TOÀN BỘ quyền cho Admin (ID = 1002)
        foreach (var permission in allPermissions)
        {
            rolePermissions.Add(new QuyenVaiTro
            {
                MaVaiTro = 1002, // Admin
                MaQuyen = permission.Id,
                NgayGan = DateTime.Now
            });
        }

        // 3. Gán một số quyền cho QuanLy (ID = 2)
        var managerPermissions = allPermissions.Where(p =>
            p.NhomQuyen == "Dashboard" ||
            p.NhomQuyen == "Product" ||
            p.NhomQuyen == "Category" ||
            p.NhomQuyen == "Brand" ||
            p.NhomQuyen == "Order" ||
            p.NhomQuyen == "Inventory" ||
            p.NhomQuyen == "Review" ||
            p.NhomQuyen == "Report"
        ).ToList();

        foreach (var permission in managerPermissions)
        {
            rolePermissions.Add(new QuyenVaiTro
            {
                MaVaiTro = 2, // QuanLy
                MaQuyen = permission.Id,
                NgayGan = DateTime.Now
            });
        }

        // 4. Gán một số quyền hạn chế cho NhanVien (ID = 3)
        var staffPermissions = allPermissions.Where(p =>
            p.NhomQuyen == "Dashboard" ||
            (p.NhomQuyen == "Product" && p.HanhDong == "Index") ||
            (p.NhomQuyen == "Product" && p.HanhDong == "Details") ||
            (p.NhomQuyen == "Order" && (p.HanhDong == "Index" || p.HanhDong == "Details" || p.HanhDong == "UpdateStatus")) ||
            (p.NhomQuyen == "Inventory" && (p.HanhDong == "Index" || p.HanhDong == "History"))
        ).ToList();

        foreach (var permission in staffPermissions)
        {
            rolePermissions.Add(new QuyenVaiTro
            {
                MaVaiTro = 3, // NhanVien
                MaQuyen = permission.Id,
                NgayGan = DateTime.Now
            });
        }

        await context.QuyenVaiTros.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
    }
}
