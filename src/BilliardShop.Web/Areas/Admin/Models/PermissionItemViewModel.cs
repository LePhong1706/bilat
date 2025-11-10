namespace BilliardShop.Web.Areas.Admin.Models;

public class PermissionItemViewModel
{
    public int Id { get; set; }
    public string MaQuyen { get; set; } = string.Empty;
    public string TenQuyen { get; set; } = string.Empty;
    public string HanhDong { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public bool IsSelected { get; set; }
}
