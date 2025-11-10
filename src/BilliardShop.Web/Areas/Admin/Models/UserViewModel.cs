using System.ComponentModel.DataAnnotations;

namespace BilliardShop.Web.Areas.Admin.Models;

public class UserViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
    [Display(Name = "Họ")]
    public string? Ho { get; set; }

    [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
    [Display(Name = "Tên")]
    public string? Ten { get; set; }

    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(1)]
    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; } // M, F, K

    [Required(ErrorMessage = "Vai trò là bắt buộc")]
    [Display(Name = "Vai trò")]
    public int MaVaiTro { get; set; }

    [Display(Name = "Trạng thái hoạt động")]
    public bool TrangThaiHoatDong { get; set; } = true;

    [Display(Name = "Ngày tạo")]
    public DateTime NgayTao { get; set; }

    [Display(Name = "Ngày cập nhật cuối")]
    public DateTime? NgayCapNhatCuoi { get; set; }
}
