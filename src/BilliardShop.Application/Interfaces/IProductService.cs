using BilliardShop.Domain.Entities;

namespace BilliardShop.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<SanPham>> GetFeaturedProductsAsync(int count = 10);
    Task<SanPham?> GetProductBySlugAsync(string productSlug);
    Task<IEnumerable<DanhMucSanPham>> GetAllCategoriesAsync();
    Task<DanhMucSanPham?> GetCategoryBySlugAsync(string categorySlug);

    // Unified filtering method
    Task<IEnumerable<SanPham>> GetProductsAsync(
        string? searchTerm = null,
        string? categorySlug = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        int pageNumber = 1,
        int pageSize = 12);

    Task<int> GetProductsCountAsync(
        string? searchTerm = null,
        string? categorySlug = null,
        decimal? minPrice = null,
        decimal? maxPrice = null);
}
