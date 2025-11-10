using BilliardShop.Domain.Common;

namespace BilliardShop.Domain.Entities;

/// <summary>
/// Entity liên kết giữa Vai trò và Quyền
/// Quản lý quyền được gán cho từng vai trò
/// </summary>
public class QuyenVaiTro : BaseEntity
{
    /// <summary>
    /// Mã vai trò
    /// </summary>
    public int MaVaiTro { get; set; }

    /// <summary>
    /// Mã quyền
    /// </summary>
    public int MaQuyen { get; set; }

    /// <summary>
    /// Ngày gán quyền
    /// </summary>
    public DateTime NgayGan { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual VaiTroNguoiDung VaiTro { get; set; } = null!;
    public virtual Quyen Quyen { get; set; } = null!;
}
