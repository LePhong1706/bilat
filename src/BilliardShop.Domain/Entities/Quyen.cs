using BilliardShop.Domain.Common;

namespace BilliardShop.Domain.Entities;

/// <summary>
/// Entity đại diện cho quyền trong hệ thống
/// Quản lý quyền theo trang và hành động
/// </summary>
public class Quyen : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Mã định danh quyền (VD: SanPham.Xem, SanPham.Them, DonHang.CapNhat)
    /// </summary>
    public string MaQuyen { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị của quyền
    /// </summary>
    public string TenQuyen { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả chi tiết về quyền
    /// </summary>
    public string? MoTa { get; set; }

    /// <summary>
    /// Nhóm quyền (VD: SanPham, DonHang, NguoiDung)
    /// </summary>
    public string NhomQuyen { get; set; } = string.Empty;

    /// <summary>
    /// Hành động (VD: Xem, Them, Sua, Xoa, CapNhat)
    /// </summary>
    public string HanhDong { get; set; } = string.Empty;

    /// <summary>
    /// Thứ tự sắp xếp
    /// </summary>
    public int ThuTuSapXep { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool TrangThaiHoatDong { get; set; } = true;

    // Audit fields
    public DateTime NgayTao { get; set; }
    public DateTime? NgayCapNhatCuoi { get; set; }
    public int? NguoiTao { get; set; }
    public int? NguoiCapNhatCuoi { get; set; }

    // Navigation properties
    public virtual ICollection<QuyenVaiTro> QuyenVaiTros { get; set; } = new List<QuyenVaiTro>();
}
