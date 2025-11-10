using System.ComponentModel.DataAnnotations;
using BilliardShop.Domain.Entities;

namespace BilliardShop.Web.Areas.Admin.Models;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
    [StringLength(50, ErrorMessage = "Mã sản phẩm không được vượt quá 50 ký tự")]
    [Display(Name = "Mã sản phẩm")]
    public string MaCodeSanPham { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự")]
    [Display(Name = "Tên sản phẩm")]
    public string TenSanPham { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả ngắn không được vượt quá 500 ký tự")]
    [Display(Name = "Mô tả ngắn")]
    public string? MoTaNgan { get; set; }

    [Display(Name = "Mô tả chi tiết")]
    public string? MoTaChiTiet { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    [Display(Name = "Danh mục")]
    public int MaDanhMuc { get; set; }

    [Display(Name = "Thương hiệu")]
    public int? MaThuongHieu { get; set; }

    [Required(ErrorMessage = "Giá gốc là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn 0")]
    [Display(Name = "Giá gốc")]
    public decimal GiaGoc { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá khuyến mãi phải lớn hơn 0")]
    [Display(Name = "Giá khuyến mãi")]
    public decimal? GiaKhuyenMai { get; set; }

    [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
    [Display(Name = "Số lượng tồn kho")]
    public int SoLuongTonKho { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tối thiểu phải lớn hơn hoặc bằng 0")]
    [Display(Name = "Số lượng tối thiểu")]
    public int SoLuongToiThieu { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "Trọng lượng phải lớn hơn 0")]
    [Display(Name = "Trọng lượng (kg)")]
    public decimal? TrongLuong { get; set; }

    [StringLength(50, ErrorMessage = "Kích thước không được vượt quá 50 ký tự")]
    [Display(Name = "Kích thước")]
    public string? KichThuoc { get; set; }

    [StringLength(100, ErrorMessage = "Chất liệu không được vượt quá 100 ký tự")]
    [Display(Name = "Chất liệu")]
    public string? ChatLieu { get; set; }

    [StringLength(50, ErrorMessage = "Màu sắc không được vượt quá 50 ký tự")]
    [Display(Name = "Màu sắc")]
    public string? MauSac { get; set; }

    [Display(Name = "Trạng thái hoạt động")]
    public bool TrangThaiHoatDong { get; set; } = true;

    [Display(Name = "Sản phẩm nổi bật")]
    public bool LaSanPhamNoiBat { get; set; } = false;

    [StringLength(255, ErrorMessage = "Tiêu đề SEO không được vượt quá 255 ký tự")]
    [Display(Name = "Tiêu đề SEO")]
    public string? TieuDeSEO { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả SEO không được vượt quá 500 ký tự")]
    [Display(Name = "Mô tả SEO")]
    public string? MoTaSEO { get; set; }

    [StringLength(255, ErrorMessage = "Từ khóa SEO không được vượt quá 255 ký tự")]
    [Display(Name = "Từ khóa SEO")]
    public string? TuKhoaSEO { get; set; }

    [Display(Name = "Hình ảnh hiện có")]
    public List<HinhAnhSanPham> ExistingImages { get; set; } = new List<HinhAnhSanPham>();
}
