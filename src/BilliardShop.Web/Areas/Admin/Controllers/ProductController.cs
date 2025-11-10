using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces;
using BilliardShop.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : BaseAdminController
{
    private readonly IWebHostEnvironment _environment;

    public ProductController(
        IUnitOfWork unitOfWork,
        ILogger<ProductController> logger,
        IWebHostEnvironment environment)
        : base(unitOfWork, logger)
    {
        _environment = environment;
    }

    // GET: Admin/Product
    public async Task<IActionResult> Index(string? search, int? categoryId, bool showDeleted = false, int page = 1)
    {
        try
        {
            int pageSize = 20;
            var query = _unitOfWork.SanPhamRepository.Query();

            // Mặc định chỉ hiển thị sản phẩm đang hoạt động
            if (!showDeleted)
            {
                query = query.Where(p => p.TrangThaiHoatDong);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.TenSanPham.Contains(search) || p.MaCodeSanPham.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.MaDanhMuc == categoryId.Value);
            }

            var (productList, totalCount) = await _unitOfWork.SanPhamRepository.GetPagedAsync(
                page,
                pageSize,
                p => true,
                query => query.OrderByDescending(p => p.NgayTao),
                p => p.DanhMuc!,
                p => p.ThuongHieu!,
                p => p.HinhAnhs
            );
            var products = productList;

            // Load categories cho filter
            var categories = await _unitOfWork.DanhMucSanPhamRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "TenDanhMuc", categoryId);
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentPage = page;

            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products");
            ShowErrorMessage("Có lỗi xảy ra khi tải danh sách sản phẩm");
            return View(new List<SanPham>());
        }
    }

    // GET: Admin/Product/Create
    public async Task<IActionResult> Create()
    {
        await LoadViewData();
        return View(new ProductViewModel());
    }

    // POST: Admin/Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model, IFormFileCollection images)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(model);
        }

        try
        {
            var product = new SanPham
            {
                MaCodeSanPham = model.MaCodeSanPham,
                TenSanPham = model.TenSanPham,
                DuongDanSanPham = GenerateSlug(model.TenSanPham),
                MoTaNgan = model.MoTaNgan,
                MoTaChiTiet = model.MoTaChiTiet,
                MaDanhMuc = model.MaDanhMuc,
                MaThuongHieu = model.MaThuongHieu,
                GiaGoc = model.GiaGoc,
                GiaKhuyenMai = model.GiaKhuyenMai,
                SoLuongTonKho = model.SoLuongTonKho,
                SoLuongToiThieu = model.SoLuongToiThieu,
                TrongLuong = model.TrongLuong,
                KichThuoc = model.KichThuoc,
                ChatLieu = model.ChatLieu,
                MauSac = model.MauSac,
                TrangThaiHoatDong = model.TrangThaiHoatDong,
                LaSanPhamNoiBat = model.LaSanPhamNoiBat,
                TieuDeSEO = model.TieuDeSEO,
                MoTaSEO = model.MoTaSEO,
                TuKhoaSEO = model.TuKhoaSEO,
                NguoiTaoMaSanPham = GetCurrentUserId(),
                NgayTao = DateTime.Now
            };

            await _unitOfWork.SanPhamRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Upload và lưu hình ảnh
            if (images != null && images.Count > 0)
            {
                await SaveProductImagesAsync(product.Id, images);
            }

            ShowSuccessMessage($"Đã thêm sản phẩm '{product.TenSanPham}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            ShowErrorMessage("Có lỗi xảy ra khi thêm sản phẩm");
            await LoadViewData();
            return View(model);
        }
    }

    // GET: Admin/Product/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var product = await _unitOfWork.SanPhamRepository.GetByIdAsync(id);
            if (product == null)
            {
                ShowErrorMessage("Không tìm thấy sản phẩm");
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                MaCodeSanPham = product.MaCodeSanPham,
                TenSanPham = product.TenSanPham,
                MoTaNgan = product.MoTaNgan,
                MoTaChiTiet = product.MoTaChiTiet,
                MaDanhMuc = product.MaDanhMuc,
                MaThuongHieu = product.MaThuongHieu,
                GiaGoc = product.GiaGoc,
                GiaKhuyenMai = product.GiaKhuyenMai,
                SoLuongTonKho = product.SoLuongTonKho,
                SoLuongToiThieu = product.SoLuongToiThieu,
                TrongLuong = product.TrongLuong,
                KichThuoc = product.KichThuoc,
                ChatLieu = product.ChatLieu,
                MauSac = product.MauSac,
                TrangThaiHoatDong = product.TrangThaiHoatDong,
                LaSanPhamNoiBat = product.LaSanPhamNoiBat,
                TieuDeSEO = product.TieuDeSEO,
                MoTaSEO = product.MoTaSEO,
                TuKhoaSEO = product.TuKhoaSEO
            };

            // Load hình ảnh hiện có
            var existingImages = await _unitOfWork.HinhAnhSanPhamRepository
                .FindAsync(h => h.MaSanPham == id);
            model.ExistingImages = existingImages.ToList();

            await LoadViewData();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading product {ProductId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi tải thông tin sản phẩm");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel model, IFormFileCollection images)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(model);
        }

        try
        {
            var product = await _unitOfWork.SanPhamRepository.GetByIdAsync(id);
            if (product == null)
            {
                ShowErrorMessage("Không tìm thấy sản phẩm");
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật thông tin
            product.TenSanPham = model.TenSanPham;
            product.MoTaNgan = model.MoTaNgan;
            product.MoTaChiTiet = model.MoTaChiTiet;
            product.MaDanhMuc = model.MaDanhMuc;
            product.MaThuongHieu = model.MaThuongHieu;
            product.GiaGoc = model.GiaGoc;
            product.GiaKhuyenMai = model.GiaKhuyenMai;
            product.SoLuongTonKho = model.SoLuongTonKho;
            product.SoLuongToiThieu = model.SoLuongToiThieu;
            product.TrongLuong = model.TrongLuong;
            product.KichThuoc = model.KichThuoc;
            product.ChatLieu = model.ChatLieu;
            product.MauSac = model.MauSac;
            product.TrangThaiHoatDong = model.TrangThaiHoatDong;
            product.LaSanPhamNoiBat = model.LaSanPhamNoiBat;
            product.TieuDeSEO = model.TieuDeSEO;
            product.MoTaSEO = model.MoTaSEO;
            product.TuKhoaSEO = model.TuKhoaSEO;
            product.NgayCapNhatCuoi = DateTime.Now;

            _unitOfWork.SanPhamRepository.Update(product);

            // Upload hình mới nếu có
            if (images != null && images.Count > 0)
            {
                await SaveProductImagesAsync(product.Id, images);
            }

            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã cập nhật sản phẩm '{product.TenSanPham}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi cập nhật sản phẩm");
            await LoadViewData();
            return View(model);
        }
    }

    // POST: Admin/Product/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var product = await _unitOfWork.SanPhamRepository.GetByIdAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            var productName = product.TenSanPham;

            // Xóa hình ảnh liên quan trước
            var productImages = await _unitOfWork.HinhAnhSanPhamRepository.FindAsync(h => h.MaSanPham == id);
            foreach (var image in productImages)
            {
                _unitOfWork.HinhAnhSanPhamRepository.Remove(image);

                // Xóa file vật lý nếu có
                if (!string.IsNullOrEmpty(image.DuongDanHinhAnh))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, image.DuongDanHinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            // Hard delete - xóa vĩnh viễn
            _unitOfWork.SanPhamRepository.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã xóa sản phẩm '{productName}' vĩnh viễn thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm" });
        }
    }

    #region Helper Methods

    private async Task LoadViewData()
    {
        var categories = await _unitOfWork.DanhMucSanPhamRepository.GetAllAsync();
        var brands = await _unitOfWork.ThuongHieuRepository.GetAllAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "TenDanhMuc");
        ViewBag.Brands = new SelectList(brands, "Id", "TenThuongHieu");
    }

    private static string GenerateSlug(string text)
    {
        // Simple slug generation - có thể cải tiến
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

    private async Task SaveProductImagesAsync(int productId, IFormFileCollection images)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "products", productId.ToString());
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        int order = 1;
        foreach (var image in images)
        {
            if (image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var productImage = new HinhAnhSanPham
                {
                    MaSanPham = productId,
                    DuongDanHinhAnh = $"/uploads/products/{productId}/{fileName}",
                    LaHinhAnhChinh = order == 1,
                    ThuTuSapXep = order++,
                    NgayTao = DateTime.Now
                };

                await _unitOfWork.HinhAnhSanPhamRepository.AddAsync(productImage);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    #endregion
}
