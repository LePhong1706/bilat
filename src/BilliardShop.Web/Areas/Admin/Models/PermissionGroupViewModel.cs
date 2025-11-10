namespace BilliardShop.Web.Areas.Admin.Models;

public class PermissionGroupViewModel
{
    public string GroupName { get; set; } = string.Empty;
    public List<PermissionItemViewModel> Permissions { get; set; } = new List<PermissionItemViewModel>();
}
