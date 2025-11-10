using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BilliardShop.Domain.Interfaces;
using System.Security.Claims;

namespace BilliardShop.Web.Attributes;

/// <summary>
/// Attribute để kiểm tra quyền truy cập controller/action
/// Sử dụng: [Permission("SanPham.Xem")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class PermissionAttribute : TypeFilterAttribute
{
    public PermissionAttribute(string permission) : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permission };
    }
}

public class PermissionFilter : IAsyncAuthorizationFilter
{
    private readonly string _permission;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PermissionFilter> _logger;

    public PermissionFilter(string permission, IUnitOfWork unitOfWork, ILogger<PermissionFilter> logger)
    {
        _permission = permission;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Kiểm tra đăng nhập
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectResult("/Admin/Auth/Login");
            return;
        }

        // Lấy role ID
        var roleIdClaim = context.HttpContext.User.FindFirst("RoleId")?.Value;
        if (string.IsNullOrEmpty(roleIdClaim) || !int.TryParse(roleIdClaim, out int roleId))
        {
            context.Result = new RedirectResult("/Admin/Auth/AccessDenied");
            return;
        }

        // Kiểm tra quyền
        var hasPermission = await _unitOfWork.QuyenVaiTroRepository.HasPermissionAsync(roleId, _permission);

        if (!hasPermission)
        {
            _logger.LogWarning("User {UserId} with role {RoleId} denied access to {Permission}",
                context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                roleId,
                _permission);

            context.Result = new RedirectResult("/Admin/Auth/AccessDenied");
        }
    }
}
