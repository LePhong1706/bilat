namespace BilliardShop.Web.Areas.Admin.Models;

public class TopProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
}
