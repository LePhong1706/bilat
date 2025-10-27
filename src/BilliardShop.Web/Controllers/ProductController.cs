using BilliardShop.Application.Interfaces;
using BilliardShop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BilliardShop.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(
        string? categorySlug,
        string? q,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        int page = 1)
    {
        var viewModel = new ProductListViewModel
        {
            CurrentPage = page,
            PageSize = 12,
            SearchTerm = q,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sortBy
        };

        // Get category info if specified
        if (!string.IsNullOrEmpty(categorySlug))
        {
            viewModel.CurrentCategory = await _productService.GetCategoryBySlugAsync(categorySlug);
            if (viewModel.CurrentCategory == null)
            {
                return NotFound();
            }
        }

        // Get products with all filters applied
        viewModel.Products = await _productService.GetProductsAsync(
            searchTerm: q,
            categorySlug: categorySlug,
            minPrice: minPrice,
            maxPrice: maxPrice,
            sortBy: sortBy,
            pageNumber: page,
            pageSize: viewModel.PageSize
        );

        viewModel.TotalProducts = await _productService.GetProductsCountAsync(
            searchTerm: q,
            categorySlug: categorySlug,
            minPrice: minPrice,
            maxPrice: maxPrice
        );

        viewModel.AllCategories = await _productService.GetAllCategoriesAsync();

        return View(viewModel);
    }

    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        var product = await _productService.GetProductBySlugAsync(slug);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new ProductDetailViewModel
        {
            Product = product,
            RelatedProducts = await _productService.GetProductsAsync(
                categorySlug: product.DanhMuc.DuongDanDanhMuc,
                pageNumber: 1,
                pageSize: 4
            )
        };

        // Remove the current product from related products
        viewModel.RelatedProducts = viewModel.RelatedProducts
            .Where(p => p.Id != product.Id)
            .Take(3);

        return View(viewModel);
    }
}
