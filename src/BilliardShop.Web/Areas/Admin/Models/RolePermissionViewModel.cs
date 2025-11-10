namespace BilliardShop.Web.Areas.Admin.Models;

public class RolePermissionsViewModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<PermissionGroupViewModel> PermissionGroups { get; set; } = new List<PermissionGroupViewModel>();
    public List<int> AssignedPermissionIds { get; set; } = new List<int>();
}
