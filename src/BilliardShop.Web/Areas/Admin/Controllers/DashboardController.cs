using BilliardShop.Domain.Interfaces;
using BilliardShop.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : BaseAdminController
{
    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        : base(unitOfWork, logger)
    {
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var model = new DashboardViewModel();

            // Tổng số sản phẩm
            model.TotalProducts = await _unitOfWork.SanPhamRepository.CountAsync(p => p.TrangThaiHoatDong);

            // Tổng số đơn hàng
            model.TotalOrders = await _unitOfWork.DonHangRepository.CountAsync();

            // Tổng số khách hàng (vai trò = KhachHang)
            model.TotalCustomers = await _unitOfWork.NguoiDungRepository.CountAsync(u => u.MaVaiTro == 4 && u.TrangThaiHoatDong);

            // Tổng doanh thu (đơn hàng đã giao)
            var completedOrders = await _unitOfWork.DonHangRepository
                .FindAsync(o => o.MaTrangThai == 5); // 5 = DaGiao
            model.TotalRevenue = completedOrders.Sum(o => o.TongThanhToan);

            // Sản phẩm sắp hết hàng (tồn kho < số lượng tối thiểu)
            model.LowStockProducts = await _unitOfWork.SanPhamRepository
                .FindAsync(p => p.TrangThaiHoatDong && p.SoLuongTonKho < p.SoLuongToiThieu);

            // Đơn hàng gần đây (10 đơn)
            var (orderList, _) = await _unitOfWork.DonHangRepository
                .GetPagedAsync(1, 10, null, query => query.OrderByDescending(o => o.NgayDatHang));
            model.RecentOrders = orderList;

            // Đánh giá cần duyệt
            model.PendingReviews = await _unitOfWork.DanhGiaSanPhamRepository
                .CountAsync(r => !r.DaDuyet);

            // Doanh thu 7 ngày gần đây
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .ToList();

            model.RevenueChart = new ChartData
            {
                Labels = last7Days.Select(d => d.ToString("dd/MM")).ToList(),
                Data = new List<decimal>()
            };

            foreach (var date in last7Days)
            {
                var dayOrders = await _unitOfWork.DonHangRepository
                    .FindAsync(o => o.NgayDatHang.Date == date && o.MaTrangThai == 5);
                model.RevenueChart.Data.Add(dayOrders.Sum(o => o.TongThanhToan));
            }

            // Top 5 sản phẩm bán chạy
            var topProducts = await _unitOfWork.ChiTietDonHangRepository
                .Query()
                .GroupBy(c => c.MaSanPham)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(c => c.SoLuong)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToListAsync();

            model.TopProducts = new List<TopProductViewModel>();
            foreach (var item in topProducts)
            {
                var product = await _unitOfWork.SanPhamRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    model.TopProducts.Add(new TopProductViewModel
                    {
                        ProductName = product.TenSanPham,
                        TotalSold = item.TotalSold,
                        Revenue = 0 // Tính sau nếu cần
                    });
                }
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            ShowErrorMessage("Có lỗi xảy ra khi tải dashboard");
            return View(new DashboardViewModel());
        }
    }
}
