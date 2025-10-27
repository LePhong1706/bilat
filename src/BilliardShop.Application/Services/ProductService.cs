using BilliardShop.Application.Interfaces;
using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces;

namespace BilliardShop.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SanPham>> GetFeaturedProductsAsync(int count = 10)
    {
        var products = await _unitOfWork.SanPhamRepository
            .FindAsync(
                p => p.TrangThaiHoatDong && p.LaSanPhamNoiBat,
                p => p.HinhAnhs,
                p => p.DanhMuc,
                p => p.ThuongHieu
            );

        return products.OrderByDescending(p => p.NgayTao).Take(count);
    }

    public async Task<SanPham?> GetProductBySlugAsync(string productSlug)
    {
        var products = await _unitOfWork.SanPhamRepository
            .FindAsync(
                p => p.DuongDanSanPham == productSlug && p.TrangThaiHoatDong,
                p => p.HinhAnhs,
                p => p.DanhMuc,
                p => p.ThuongHieu,
                p => p.ThuocTinhs,
                p => p.DanhGias
            );

        return products.FirstOrDefault();
    }

    public async Task<IEnumerable<DanhMucSanPham>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.DanhMucSanPhamRepository.GetAllAsync();
        return categories.Where(c => c.TrangThaiHoatDong).OrderBy(c => c.ThuTuSapXep);
    }

    public async Task<DanhMucSanPham?> GetCategoryBySlugAsync(string categorySlug)
    {
        return await _unitOfWork.DanhMucSanPhamRepository
            .FirstOrDefaultAsync(c => c.DuongDanDanhMuc == categorySlug && c.TrangThaiHoatDong);
    }

    public async Task<IEnumerable<SanPham>> GetProductsAsync(
        string? searchTerm = null,
        string? categorySlug = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        int pageNumber = 1,
        int pageSize = 12)
    {
        var result = await _unitOfWork.SanPhamRepository
            .GetPagedAsync(
                pageNumber,
                pageSize,
                filter: BuildFilterExpression(searchTerm, categorySlug, minPrice, maxPrice),
                orderBy: GetSortExpression(sortBy),
                p => p.HinhAnhs,
                p => p.DanhMuc,
                p => p.ThuongHieu
            );

        return result.Items;
    }

    public async Task<int> GetProductsCountAsync(
        string? searchTerm = null,
        string? categorySlug = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        return await _unitOfWork.SanPhamRepository
            .CountAsync(BuildFilterExpression(searchTerm, categorySlug, minPrice, maxPrice));
    }

    private System.Linq.Expressions.Expression<Func<SanPham, bool>> BuildFilterExpression(
        string? searchTerm,
        string? categorySlug,
        decimal? minPrice,
        decimal? maxPrice)
    {
        return p => p.TrangThaiHoatDong
            && (string.IsNullOrEmpty(searchTerm)
                || p.TenSanPham.Contains(searchTerm)
                || (p.MoTaNgan != null && p.MoTaNgan.Contains(searchTerm))
                || (p.MoTaChiTiet != null && p.MoTaChiTiet.Contains(searchTerm)))
            && (string.IsNullOrEmpty(categorySlug) || p.DanhMuc.DuongDanDanhMuc == categorySlug)
            && (!minPrice.HasValue || (p.GiaKhuyenMai.HasValue ? p.GiaKhuyenMai.Value : p.GiaGoc) >= minPrice.Value)
            && (!maxPrice.HasValue || (p.GiaKhuyenMai.HasValue ? p.GiaKhuyenMai.Value : p.GiaGoc) <= maxPrice.Value);
    }

    private Func<IQueryable<SanPham>, IOrderedQueryable<SanPham>> GetSortExpression(string? sortBy)
    {
        return sortBy switch
        {
            "price_asc" => q => q.OrderBy(p => p.GiaKhuyenMai ?? p.GiaGoc),
            "price_desc" => q => q.OrderByDescending(p => p.GiaKhuyenMai ?? p.GiaGoc),
            "name_asc" => q => q.OrderBy(p => p.TenSanPham),
            "name_desc" => q => q.OrderByDescending(p => p.TenSanPham),
            "newest" => q => q.OrderByDescending(p => p.NgayTao),
            "oldest" => q => q.OrderBy(p => p.NgayTao),
            _ => q => q.OrderByDescending(p => p.NgayTao) // Default: newest first
        };
    }
}
