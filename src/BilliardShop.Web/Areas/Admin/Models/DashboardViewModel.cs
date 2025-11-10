using BilliardShop.Domain.Entities;

namespace BilliardShop.Web.Areas.Admin.Models;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingReviews { get; set; }

    public IEnumerable<SanPham> LowStockProducts { get; set; } = new List<SanPham>();
    public IEnumerable<DonHang> RecentOrders { get; set; } = new List<DonHang>();
    public List<TopProductViewModel> TopProducts { get; set; } = new List<TopProductViewModel>();
    public ChartData RevenueChart { get; set; } = new ChartData();
}

public class ChartData
{
    public List<string> Labels { get; set; } = new List<string>();
    public List<decimal> Data { get; set; } = new List<decimal>();
}
