using System.ComponentModel.DataAnnotations;

namespace BilliardShop.Web.Areas.Admin.Models;

public class RoleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
    [Display(Name = "Tên vai trò")]
    public string TenVaiTro { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Trạng thái hoạt động")]
    public bool TrangThaiHoatDong { get; set; } = true;
}
