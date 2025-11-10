using System.ComponentModel.DataAnnotations;

namespace BilliardShop.Web.Areas.Admin.Models;

public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
    [Display(Name = "Tên danh mục")]
    public string TenDanhMuc { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Danh mục cha")]
    public int? MaDanhMucCha { get; set; }

    [Display(Name = "Thứ tự sắp xếp")]
    public int ThuTuSapXep { get; set; } = 0;

    [Display(Name = "Trạng thái hoạt động")]
    public bool TrangThaiHoatDong { get; set; } = true;

    [Display(Name = "Hình ảnh hiện tại")]
    public string? CurrentImage { get; set; }
}
