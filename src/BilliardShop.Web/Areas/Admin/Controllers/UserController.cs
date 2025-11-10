using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces;
using BilliardShop.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Text;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController : BaseAdminController
{
    public UserController(IUnitOfWork unitOfWork, ILogger<UserController> logger)
        : base(unitOfWork, logger)
    {
    }

    // GET: Admin/User
    public async Task<IActionResult> Index(string? search, int? roleId, int page = 1)
    {
        try
        {
            int pageSize = 20;
            var query = _unitOfWork.NguoiDungRepository.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.TenDangNhap.Contains(search) ||
                                        u.Email.Contains(search) ||
                                        (u.Ho + " " + u.Ten).Contains(search));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.MaVaiTro == roleId.Value);
            }

            // Lọc theo điều kiện đã áp dụng trong query
            var (userList, totalCount) = await _unitOfWork.NguoiDungRepository.GetPagedAsync(
                page, pageSize,
                u => true,
                query => query.OrderByDescending(u => u.NgayTao),
                u => u.VaiTro!
            );
            var users = userList;

            var roles = await _unitOfWork.VaiTroNguoiDungRepository.GetAllAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "TenVaiTro", roleId);
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentPage = page;

            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            ShowErrorMessage("Có lỗi xảy ra khi tải danh sách người dùng");
            return View(new List<NguoiDung>());
        }
    }

    // GET: Admin/User/Create
    public async Task<IActionResult> Create()
    {
        await LoadRoles();
        return View(new UserViewModel());
    }

    // POST: Admin/User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadRoles();
            return View(model);
        }

        try
        {
            // Kiểm tra username đã tồn tại
            var existingUsers = await _unitOfWork.NguoiDungRepository.FindAsync(u => u.TenDangNhap == model.TenDangNhap);
            var existingUser = existingUsers.FirstOrDefault();
            if (existingUser != null)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                await LoadRoles();
                return View(model);
            }

            // Kiểm tra email đã tồn tại
            var existingEmail = await _unitOfWork.NguoiDungRepository.GetByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng");
                await LoadRoles();
                return View(model);
            }

            // Tạo salt và hash password
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(model.Password, salt);

            var user = new NguoiDung
            {
                TenDangNhap = model.TenDangNhap,
                Email = model.Email,
                MatKhauMaHoa = hashedPassword,
                MuoiMatKhau = salt,
                Ho = model.Ho,
                Ten = model.Ten,
                SoDienThoai = model.SoDienThoai,
                NgaySinh = model.NgaySinh,
                GioiTinh = model.GioiTinh,
                MaVaiTro = model.MaVaiTro,
                TrangThaiHoatDong = model.TrangThaiHoatDong,
                NgayTao = DateTime.Now,
                NguoiTao = GetCurrentUserId()
            };

            await _unitOfWork.NguoiDungRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã thêm người dùng '{user.TenDangNhap}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            ShowErrorMessage("Có lỗi xảy ra khi thêm người dùng");
            await LoadRoles();
            return View(model);
        }
    }

    // GET: Admin/User/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var user = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (user == null)
            {
                ShowErrorMessage("Không tìm thấy người dùng");
                return RedirectToAction(nameof(Index));
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                TenDangNhap = user.TenDangNhap,
                Email = user.Email,
                Ho = user.Ho,
                Ten = user.Ten,
                SoDienThoai = user.SoDienThoai,
                NgaySinh = user.NgaySinh,
                GioiTinh = user.GioiTinh,
                MaVaiTro = user.MaVaiTro,
                TrangThaiHoatDong = user.TrangThaiHoatDong,
                NgayTao = user.NgayTao,
                NgayCapNhatCuoi = user.NgayCapNhatCuoi
            };

            await LoadRoles();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user {UserId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi tải thông tin người dùng");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/User/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        // Remove password validation for edit
        ModelState.Remove("Password");
        ModelState.Remove("ConfirmPassword");

        if (!ModelState.IsValid)
        {
            await LoadRoles();
            return View(model);
        }

        try
        {
            var user = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (user == null)
            {
                ShowErrorMessage("Không tìm thấy người dùng");
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra email nếu thay đổi
            if (user.Email != model.Email)
            {
                var existingEmail = await _unitOfWork.NguoiDungRepository.GetByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    await LoadRoles();
                    return View(model);
                }
            }

            user.Email = model.Email;
            user.Ho = model.Ho;
            user.Ten = model.Ten;
            user.SoDienThoai = model.SoDienThoai;
            user.NgaySinh = model.NgaySinh;
            user.GioiTinh = model.GioiTinh;
            user.MaVaiTro = model.MaVaiTro;
            user.TrangThaiHoatDong = model.TrangThaiHoatDong;
            user.NgayCapNhatCuoi = DateTime.Now;
            user.NguoiCapNhatCuoi = GetCurrentUserId();

            _unitOfWork.NguoiDungRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã cập nhật người dùng '{user.TenDangNhap}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi cập nhật người dùng");
            await LoadRoles();
            return View(model);
        }
    }

    // POST: Admin/User/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng" });
            }

            // Không cho xóa chính mình
            if (user.Id == GetCurrentUserId())
            {
                return Json(new { success = false, message = "Không thể xóa tài khoản của chính bạn" });
            }

            // Soft delete
            user.TrangThaiHoatDong = false;
            _unitOfWork.NguoiDungRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã xóa người dùng '{user.TenDangNhap}' thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return Json(new { success = false, message = "Có lỗi xảy ra khi xóa người dùng" });
        }
    }

    // GET: Admin/User/ResetPassword/5
    public async Task<IActionResult> ResetPassword(int id)
    {
        try
        {
            var user = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (user == null)
            {
                ShowErrorMessage("Không tìm thấy người dùng");
                return RedirectToAction(nameof(Index));
            }

            var model = new ResetPasswordViewModel
            {
                UserId = user.Id,
                TenDangNhap = user.TenDangNhap
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reset password for user {UserId}", id);
            ShowErrorMessage("Có lỗi xảy ra");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/User/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var user = await _unitOfWork.NguoiDungRepository.GetByIdAsync(model.UserId);
            if (user == null)
            {
                ShowErrorMessage("Không tìm thấy người dùng");
                return RedirectToAction(nameof(Index));
            }

            // Tạo salt mới và hash password
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(model.NewPassword, salt);

            user.MuoiMatKhau = salt;
            user.MatKhauMaHoa = hashedPassword;
            user.NgayCapNhatCuoi = DateTime.Now;

            _unitOfWork.NguoiDungRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã reset mật khẩu cho người dùng '{user.TenDangNhap}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId}", model.UserId);
            ShowErrorMessage("Có lỗi xảy ra khi reset mật khẩu");
            return View(model);
        }
    }

    #region Helper Methods

    private async Task LoadRoles()
    {
        var roles = await _unitOfWork.VaiTroNguoiDungRepository.GetAllAsync();
        ViewBag.Roles = new SelectList(roles.Where(r => r.TrangThaiHoatDong), "Id", "TenVaiTro");
    }

    private static string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private static string HashPassword(string password, string salt)
    {
        // Sử dụng PBKDF2 giống như NguoiDungService
        var saltBytes = Convert.FromBase64String(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hash);
    }

    #endregion
}
