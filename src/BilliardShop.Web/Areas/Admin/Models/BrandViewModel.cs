using System.ComponentModel.DataAnnotations;

namespace BilliardShop.Web.Areas.Admin.Models;

public class BrandViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên thương hiệu là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên thương hiệu không được vượt quá 100 ký tự")]
    [Display(Name = "Tên thương hiệu")]
    public string TenThuongHieu { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [StringLength(255, ErrorMessage = "Website không được vượt quá 255 ký tự")]
    [Url(ErrorMessage = "Website không hợp lệ")]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    [StringLength(100, ErrorMessage = "Quốc gia không được vượt quá 100 ký tự")]
    [Display(Name = "Quốc gia")]
    public string? QuocGia { get; set; }

    [Display(Name = "Trạng thái hoạt động")]
    public bool TrangThaiHoatDong { get; set; } = true;

    [Display(Name = "Logo hiện tại")]
    public string? CurrentLogo { get; set; }
}
