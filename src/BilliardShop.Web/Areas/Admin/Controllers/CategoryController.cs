using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces;
using BilliardShop.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : BaseAdminController
{
    private readonly IWebHostEnvironment _environment;

    public CategoryController(
        IUnitOfWork unitOfWork,
        ILogger<CategoryController> logger,
        IWebHostEnvironment environment)
        : base(unitOfWork, logger)
    {
        _environment = environment;
    }

    // GET: Admin/Category
    public async Task<IActionResult> Index()
    {
        try
        {
            var categories = await _unitOfWork.DanhMucSanPhamRepository.GetAllAsync();

            // Tạo cây danh mục
            var rootCategories = categories.Where(c => c.MaDanhMucCha == null).OrderBy(c => c.ThuTuSapXep);

            return View(rootCategories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            ShowErrorMessage("Có lỗi xảy ra khi tải danh sách danh mục");
            return View(new List<DanhMucSanPham>());
        }
    }

    // GET: Admin/Category/Create
    public async Task<IActionResult> Create()
    {
        await LoadParentCategories();
        return View(new CategoryViewModel());
    }

    // POST: Admin/Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            await LoadParentCategories();
            return View(model);
        }

        try
        {
            var category = new DanhMucSanPham
            {
                TenDanhMuc = model.TenDanhMuc,
                DuongDanDanhMuc = GenerateSlug(model.TenDanhMuc),
                MoTa = model.MoTa,
                MaDanhMucCha = model.MaDanhMucCha,
                ThuTuSapXep = model.ThuTuSapXep,
                TrangThaiHoatDong = model.TrangThaiHoatDong,
                NgayTao = DateTime.Now,
                NguoiTao = GetCurrentUserId()
            };

            // Upload image nếu có
            if (image != null && image.Length > 0)
            {
                category.HinhAnhDaiDien = await SaveImageAsync(image, "categories");
            }

            await _unitOfWork.DanhMucSanPhamRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã thêm danh mục '{category.TenDanhMuc}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            ShowErrorMessage("Có lỗi xảy ra khi thêm danh mục");
            await LoadParentCategories();
            return View(model);
        }
    }

    // GET: Admin/Category/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var category = await _unitOfWork.DanhMucSanPhamRepository.GetByIdAsync(id);
            if (category == null)
            {
                ShowErrorMessage("Không tìm thấy danh mục");
                return RedirectToAction(nameof(Index));
            }

            var model = new CategoryViewModel
            {
                Id = category.Id,
                TenDanhMuc = category.TenDanhMuc,
                MoTa = category.MoTa,
                MaDanhMucCha = category.MaDanhMucCha,
                ThuTuSapXep = category.ThuTuSapXep,
                TrangThaiHoatDong = category.TrangThaiHoatDong,
                CurrentImage = category.HinhAnhDaiDien
            };

            await LoadParentCategories(id);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading category {CategoryId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi tải thông tin danh mục");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryViewModel model, IFormFile? image)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadParentCategories(id);
            return View(model);
        }

        try
        {
            var category = await _unitOfWork.DanhMucSanPhamRepository.GetByIdAsync(id);
            if (category == null)
            {
                ShowErrorMessage("Không tìm thấy danh mục");
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra không cho chọn chính nó làm danh mục cha
            if (model.MaDanhMucCha == id)
            {
                ModelState.AddModelError("MaDanhMucCha", "Không thể chọn chính nó làm danh mục cha");
                await LoadParentCategories(id);
                return View(model);
            }

            category.TenDanhMuc = model.TenDanhMuc;
            category.MoTa = model.MoTa;
            category.MaDanhMucCha = model.MaDanhMucCha;
            category.ThuTuSapXep = model.ThuTuSapXep;
            category.TrangThaiHoatDong = model.TrangThaiHoatDong;
            category.NgayCapNhatCuoi = DateTime.Now;
            category.NguoiCapNhatCuoi = GetCurrentUserId();

            // Upload image mới nếu có
            if (image != null && image.Length > 0)
            {
                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(category.HinhAnhDaiDien))
                {
                    DeleteImage(category.HinhAnhDaiDien);
                }
                category.HinhAnhDaiDien = await SaveImageAsync(image, "categories");
            }

            _unitOfWork.DanhMucSanPhamRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã cập nhật danh mục '{category.TenDanhMuc}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi cập nhật danh mục");
            await LoadParentCategories(id);
            return View(model);
        }
    }

    // POST: Admin/Category/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var category = await _unitOfWork.DanhMucSanPhamRepository.GetByIdAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục" });
            }

            // Kiểm tra có danh mục con không
            var hasChildren = await _unitOfWork.DanhMucSanPhamRepository.CountAsync(c => c.MaDanhMucCha == id);
            if (hasChildren > 0)
            {
                return Json(new { success = false, message = "Không thể xóa danh mục có danh mục con" });
            }

            // Kiểm tra có sản phẩm không
            var hasProducts = await _unitOfWork.SanPhamRepository.CountAsync(p => p.MaDanhMuc == id);
            if (hasProducts > 0)
            {
                return Json(new { success = false, message = $"Không thể xóa danh mục vì có {hasProducts} sản phẩm" });
            }

            _unitOfWork.DanhMucSanPhamRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            // Xóa ảnh nếu có
            if (!string.IsNullOrEmpty(category.HinhAnhDaiDien))
            {
                DeleteImage(category.HinhAnhDaiDien);
            }

            return Json(new { success = true, message = $"Đã xóa danh mục '{category.TenDanhMuc}' thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return Json(new { success = false, message = "Có lỗi xảy ra khi xóa danh mục" });
        }
    }

    #region Helper Methods

    private async Task LoadParentCategories(int? excludeId = null)
    {
        var categories = await _unitOfWork.DanhMucSanPhamRepository.GetAllAsync();
        var filteredCategories = categories.Where(c => c.TrangThaiHoatDong && (!excludeId.HasValue || c.Id != excludeId.Value));
        ViewBag.ParentCategories = new SelectList(filteredCategories, "Id", "TenDanhMuc");
    }

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
