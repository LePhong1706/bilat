using BilliardShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilliardShop.Infrastructure.Data.SeedData;

/// <summary>
/// Class để seed dữ liệu quyền mặc định vào database
/// </summary>
public static class PermissionSeeder
{
    public static async Task SeedPermissionsAsync(BilliardShopDbContext context)
    {
        // Kiểm tra xem đã có quyền chưa
        if (await context.Quyens.AnyAsync())
        {
            return; // Đã có dữ liệu rồi
        }

        var permissions = new List<Quyen>();
        int order = 1;

        // DASHBOARD
        permissions.Add(CreatePermission("Dashboard", "Index", "Xem dashboard", order++));

        // QUẢN LÝ NGƯỜI DÙNG
        permissions.Add(CreatePermission("User", "Index", "Xem danh sách người dùng", order++));
        permissions.Add(CreatePermission("User", "Create", "Thêm người dùng", order++));
        permissions.Add(CreatePermission("User", "Edit", "Sửa người dùng", order++));
        permissions.Add(CreatePermission("User", "Delete", "Xóa người dùng", order++));
        permissions.Add(CreatePermission("User", "ResetPassword", "Reset mật khẩu", order++));

        // QUẢN LÝ VAI TRÒ & PHÂN QUYỀN
        permissions.Add(CreatePermission("Role", "Index", "Xem danh sách vai trò", order++));
        permissions.Add(CreatePermission("Role", "Create", "Thêm vai trò", order++));
        permissions.Add(CreatePermission("Role", "Edit", "Sửa vai trò", order++));
        permissions.Add(CreatePermission("Role", "Delete", "Xóa vai trò", order++));
        permissions.Add(CreatePermission("Role", "Permissions", "Phân quyền cho vai trò", order++));

        // QUẢN LÝ SẢN PHẨM
        permissions.Add(CreatePermission("Product", "Index", "Xem danh sách sản phẩm", order++));
        permissions.Add(CreatePermission("Product", "Create", "Thêm sản phẩm", order++));
        permissions.Add(CreatePermission("Product", "Edit", "Sửa sản phẩm", order++));
        permissions.Add(CreatePermission("Product", "Delete", "Xóa sản phẩm", order++));
        permissions.Add(CreatePermission("Product", "Details", "Xem chi tiết sản phẩm", order++));

        // QUẢN LÝ DANH MỤC
        permissions.Add(CreatePermission("Category", "Index", "Xem danh sách danh mục", order++));
        permissions.Add(CreatePermission("Category", "Create", "Thêm danh mục", order++));
        permissions.Add(CreatePermission("Category", "Edit", "Sửa danh mục", order++));
        permissions.Add(CreatePermission("Category", "Delete", "Xóa danh mục", order++));

        // QUẢN LÝ THƯƠNG HIỆU
        permissions.Add(CreatePermission("Brand", "Index", "Xem danh sách thương hiệu", order++));
        permissions.Add(CreatePermission("Brand", "Create", "Thêm thương hiệu", order++));
        permissions.Add(CreatePermission("Brand", "Edit", "Sửa thương hiệu", order++));
        permissions.Add(CreatePermission("Brand", "Delete", "Xóa thương hiệu", order++));

        // QUẢN LÝ ĐƠN HÀNG
        permissions.Add(CreatePermission("Order", "Index", "Xem danh sách đơn hàng", order++));
        permissions.Add(CreatePermission("Order", "Details", "Xem chi tiết đơn hàng", order++));
        permissions.Add(CreatePermission("Order", "UpdateStatus", "Cập nhật trạng thái đơn hàng", order++));
        permissions.Add(CreatePermission("Order", "Cancel", "Hủy đơn hàng", order++));
        permissions.Add(CreatePermission("Order", "Print", "In đơn hàng", order++));

        // QUẢN LÝ KHO HÀNG
        permissions.Add(CreatePermission("Inventory", "Index", "Xem tồn kho", order++));
        permissions.Add(CreatePermission("Inventory", "Import", "Nhập kho", order++));
        permissions.Add(CreatePermission("Inventory", "Export", "Xuất kho", order++));
        permissions.Add(CreatePermission("Inventory", "Adjust", "Điều chỉnh kho", order++));
        permissions.Add(CreatePermission("Inventory", "History", "Lịch sử biến động kho", order++));

        // QUẢN LÝ NHÀ CUNG CẤP
        permissions.Add(CreatePermission("Supplier", "Index", "Xem danh sách nhà cung cấp", order++));
        permissions.Add(CreatePermission("Supplier", "Create", "Thêm nhà cung cấp", order++));
        permissions.Add(CreatePermission("Supplier", "Edit", "Sửa nhà cung cấp", order++));
        permissions.Add(CreatePermission("Supplier", "Delete", "Xóa nhà cung cấp", order++));

        // QUẢN LÝ MÃ GIẢM GIÁ
        permissions.Add(CreatePermission("Discount", "Index", "Xem danh sách mã giảm giá", order++));
        permissions.Add(CreatePermission("Discount", "Create", "Thêm mã giảm giá", order++));
        permissions.Add(CreatePermission("Discount", "Edit", "Sửa mã giảm giá", order++));
        permissions.Add(CreatePermission("Discount", "Delete", "Xóa mã giảm giá", order++));
        permissions.Add(CreatePermission("Discount", "Usage", "Xem lịch sử sử dụng", order++));

        // QUẢN LÝ ĐÁNH GIÁ
        permissions.Add(CreatePermission("Review", "Index", "Xem danh sách đánh giá", order++));
        permissions.Add(CreatePermission("Review", "Approve", "Duyệt đánh giá", order++));
        permissions.Add(CreatePermission("Review", "Reject", "Từ chối đánh giá", order++));
        permissions.Add(CreatePermission("Review", "Delete", "Xóa đánh giá", order++));

        // QUẢN LÝ BÀI VIẾT
        permissions.Add(CreatePermission("Article", "Index", "Xem danh sách bài viết", order++));
        permissions.Add(CreatePermission("Article", "Create", "Thêm bài viết", order++));
        permissions.Add(CreatePermission("Article", "Edit", "Sửa bài viết", order++));
        permissions.Add(CreatePermission("Article", "Delete", "Xóa bài viết", order++));
        permissions.Add(CreatePermission("Article", "Publish", "Xuất bản bài viết", order++));

        // CÀI ĐẶT HỆ THỐNG
        permissions.Add(CreatePermission("Settings", "Index", "Xem cài đặt", order++));
        permissions.Add(CreatePermission("Settings", "Update", "Cập nhật cài đặt", order++));
        permissions.Add(CreatePermission("Settings", "Payment", "Cài đặt thanh toán", order++));
        permissions.Add(CreatePermission("Settings", "Shipping", "Cài đặt vận chuyển", order++));

        // BÁO CÁO & THỐNG KÊ
        permissions.Add(CreatePermission("Report", "Revenue", "Báo cáo doanh thu", order++));
        permissions.Add(CreatePermission("Report", "Sales", "Báo cáo bán hàng", order++));
        permissions.Add(CreatePermission("Report", "Inventory", "Báo cáo tồn kho", order++));
        permissions.Add(CreatePermission("Report", "Customer", "Báo cáo khách hàng", order++));

        await context.Quyens.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
    }

    private static Quyen CreatePermission(string controller, string action, string description, int order)
    {
        return new Quyen
        {
            MaQuyen = $"{controller}.{action}",
            TenQuyen = description,
            MoTa = $"{description} trong hệ thống",
            NhomQuyen = controller,
            HanhDong = action,
            ThuTuSapXep = order,
            TrangThaiHoatDong = true,
            NgayTao = DateTime.Now
        };
    }
}
