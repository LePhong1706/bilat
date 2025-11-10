using BilliardShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BilliardShop.Infrastructure.Data.SeedData;

/// <summary>
/// Seeder để tạo hoặc cập nhật tài khoản admin mặc định
/// </summary>
public static class AdminUserSeeder
{
    public static async Task SeedAdminUserAsync(BilliardShopDbContext context)
    {
        // 1. Đảm bảo vai trò "Admin" tồn tại
        var adminRole = await context.VaiTroNguoiDungs.FirstOrDefaultAsync(r => r.TenVaiTro == "Admin");
        if (adminRole == null)
        {
            adminRole = new VaiTroNguoiDung
            {
                TenVaiTro = "Admin",
                MoTa = "Quản trị viên hệ thống - Có toàn quyền truy cập",
                TrangThaiHoatDong = true,
                NgayTao = DateTime.Now
            };
            context.VaiTroNguoiDungs.Add(adminRole);
            await context.SaveChangesAsync();

            var allPermissions = await context.Quyens.ToListAsync();
            foreach (var permission in allPermissions)
            {
                context.QuyenVaiTros.Add(new QuyenVaiTro
                {
                    MaVaiTro = adminRole.Id,
                    MaQuyen = permission.Id,
                    NgayGan = DateTime.Now
                });
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Đã tạo vai trò Admin với {allPermissions.Count} quyền");
        }

        // 2. Tìm hoặc tạo user admin
        var adminUser = await context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == "LuckyBoiz");

        if (adminUser == null)
        {
            // Tạo mới nếu chưa có
            var (hashedPassword, salt) = HashPassword("Admin@123");
            adminUser = new NguoiDung
            {
                TenDangNhap = "LuckyBoiz",
                Email = "admin@billiardshop.com",
                MatKhauMaHoa = hashedPassword,
                MuoiMatKhau = salt,
                Ho = "System",
                Ten = "Administrator",
                MaVaiTro = adminRole.Id,
                TrangThaiHoatDong = true,
                DaXacThucEmail = true,
                NgayTao = DateTime.Now
            };
            context.NguoiDungs.Add(adminUser);
            await context.SaveChangesAsync();

            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("📋 TÀI KHOẢN ADMIN ĐÃ ĐƯỢC TẠO");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("🔑 Username: LuckyBoiz");
            Console.WriteLine("🔒 Password: Admin@123");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("⚠️  Vui lòng đổi mật khẩu sau khi đăng nhập lần đầu!");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Hash mật khẩu với salt sử dụng PBKDF2 (RFC 2898)
    /// </summary>
    private static (string hashedPassword, string salt) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        var hashedPassword = Convert.ToBase64String(hash);

        return (hashedPassword, salt);
    }
}
