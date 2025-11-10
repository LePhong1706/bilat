using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces;
using BilliardShop.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class BrandController : BaseAdminController
{
    private readonly IWebHostEnvironment _environment;

    public BrandController(
        IUnitOfWork unitOfWork,
        ILogger<BrandController> logger,
        IWebHostEnvironment environment)
        : base(unitOfWork, logger)
    {
        _environment = environment;
    }

    // GET: Admin/Brand
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        try
        {
            int pageSize = 20;
            var query = _unitOfWork.ThuongHieuRepository.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.TenThuongHieu.Contains(search));
            }

            var (brandList, totalCount) = await _unitOfWork.ThuongHieuRepository.GetPagedAsync(
                page, pageSize,
                b => search != null ? b.TenThuongHieu.Contains(search) : true,
                query => query.OrderBy(b => b.TenThuongHieu)
            );
            var brands = brandList;

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentPage = page;

            return View(brands);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading brands");
            ShowErrorMessage("Có lỗi xảy ra khi tải danh sách thương hiệu");
            return View(new List<ThuongHieu>());
        }
    }

    // GET: Admin/Brand/Create
    public IActionResult Create()
    {
        return View(new BrandViewModel());
    }

    // POST: Admin/Brand/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BrandViewModel model, IFormFile? logo)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var brand = new ThuongHieu
            {
                TenThuongHieu = model.TenThuongHieu,
                DuongDanThuongHieu = GenerateSlug(model.TenThuongHieu),
                MoTa = model.MoTa,
                Website = model.Website,
                QuocGia = model.QuocGia,
                TrangThaiHoatDong = model.TrangThaiHoatDong,
                NgayTao = DateTime.Now,
                NguoiTao = GetCurrentUserId()
            };

            // Upload logo nếu có
            if (logo != null && logo.Length > 0)
            {
                brand.LogoThuongHieu = await SaveImageAsync(logo, "brands");
            }

            await _unitOfWork.ThuongHieuRepository.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã thêm thương hiệu '{brand.TenThuongHieu}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating brand");
            ShowErrorMessage("Có lỗi xảy ra khi thêm thương hiệu");
            return View(model);
        }
    }

    // GET: Admin/Brand/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var brand = await _unitOfWork.ThuongHieuRepository.GetByIdAsync(id);
            if (brand == null)
            {
                ShowErrorMessage("Không tìm thấy thương hiệu");
                return RedirectToAction(nameof(Index));
            }

            var model = new BrandViewModel
            {
                Id = brand.Id,
                TenThuongHieu = brand.TenThuongHieu,
                MoTa = brand.MoTa,
                Website = brand.Website,
                QuocGia = brand.QuocGia,
                TrangThaiHoatDong = brand.TrangThaiHoatDong,
                CurrentLogo = brand.LogoThuongHieu
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading brand {BrandId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi tải thông tin thương hiệu");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Brand/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BrandViewModel model, IFormFile? logo)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var brand = await _unitOfWork.ThuongHieuRepository.GetByIdAsync(id);
            if (brand == null)
            {
                ShowErrorMessage("Không tìm thấy thương hiệu");
                return RedirectToAction(nameof(Index));
            }

            brand.TenThuongHieu = model.TenThuongHieu;
            brand.MoTa = model.MoTa;
            brand.Website = model.Website;
            brand.QuocGia = model.QuocGia;
            brand.TrangThaiHoatDong = model.TrangThaiHoatDong;
            brand.NgayCapNhatCuoi = DateTime.Now;
            brand.NguoiCapNhatCuoi = GetCurrentUserId();

            // Upload logo mới nếu có
            if (logo != null && logo.Length > 0)
            {
                if (!string.IsNullOrEmpty(brand.LogoThuongHieu))
                {
                    DeleteImage(brand.LogoThuongHieu);
                }
                brand.LogoThuongHieu = await SaveImageAsync(logo, "brands");
            }

            _unitOfWork.ThuongHieuRepository.Update(brand);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã cập nhật thương hiệu '{brand.TenThuongHieu}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating brand {BrandId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi cập nhật thương hiệu");
            return View(model);
        }
    }

    // POST: Admin/Brand/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var brand = await _unitOfWork.ThuongHieuRepository.GetByIdAsync(id);
            if (brand == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thương hiệu" });
            }

            // Kiểm tra có sản phẩm không
            var hasProducts = await _unitOfWork.SanPhamRepository.CountAsync(p => p.MaThuongHieu == id);
            if (hasProducts > 0)
            {
                return Json(new { success = false, message = $"Không thể xóa thương hiệu vì có {hasProducts} sản phẩm" });
            }

            _unitOfWork.ThuongHieuRepository.Remove(brand);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(brand.LogoThuongHieu))
            {
                DeleteImage(brand.LogoThuongHieu);
            }

            return Json(new { success = true, message = $"Đã xóa thương hiệu '{brand.TenThuongHieu}' thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting brand {BrandId}", id);
            return Json(new { success = false, message = "Có lỗi xảy ra khi xóa thương hiệu" });
        }
    }

    #region Helper Methods

    private async Task<string> SaveImageAsync(IFormFile image, string folder)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return $"/uploads/{folder}/{fileName}";
    }

    private void DeleteImage(string imagePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ImagePath}", imagePath);
        }
    }

    private static string GenerateSlug(string text)
    {
        return text.ToLower()
            .Replace(" ", "-")
            .Replace("đ", "d")
            .Replace("à", "a").Replace("á", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
            .Replace("ă", "a").Replace("ằ", "a").Replace("ắ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
            .Replace("â", "a").Replace("ầ", "a").Replace("ấ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
            .Replace("è", "e").Replace("é", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
            .Replace("ê", "e").Replace("ề", "e").Replace("ế", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
            .Replace("ì", "i").Replace("í", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
            .Replace("ò", "o").Replace("ó", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
            .Replace("ô", "o").Replace("ồ", "o").Replace("ố", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
            .Replace("ơ", "o").Replace("ờ", "o").Replace("ớ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
            .Replace("ù", "u").Replace("ú", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
            .Replace("ư", "u").Replace("ừ", "u").Replace("ứ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
            .Replace("ỳ", "y").Replace("ý", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y");
    }

    #endregion
}
