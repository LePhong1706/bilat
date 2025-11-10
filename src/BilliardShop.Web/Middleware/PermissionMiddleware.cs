using BilliardShop.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Claims;

namespace BilliardShop.Web.Middleware;

/// <summary>
/// Middleware để kiểm tra phân quyền động cho mỗi request
/// </summary>
public class PermissionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PermissionMiddleware> _logger;

    public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        // Bỏ qua nếu không phải admin area
        if (!context.Request.Path.StartsWithSegments("/Admin"))
        {
            await _next(context);
            return;
        }

        // Bỏ qua Login và các trang public
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/login") || path.Contains("/accessdenied") || path.Contains("/logout"))
        {
            await _next(context);
            return;
        }

        // Bỏ qua các file tĩnh
        if (path.Contains("/assets/") || path.Contains("/js/") || path.Contains("/css/"))
        {
            await _next(context);
            return;
        }

        // Kiểm tra user đã đăng nhập chưa
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }

        // Lấy vai trò của user
        var roleIdClaim = context.User.FindFirst("RoleId")?.Value;
        if (string.IsNullOrEmpty(roleIdClaim) || !int.TryParse(roleIdClaim, out int roleId))
        {
            context.Response.Redirect("/Account/AccessDenied");
            return;
        }

        // Lấy controller và action từ endpoint
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor == null)
        {
            await _next(context);
            return;
        }

        var controller = controllerActionDescriptor.ControllerName;
        var action = controllerActionDescriptor.ActionName;

        // Cho phép Dashboard.Index luôn (trang chủ admin)
        if (controller == "Dashboard" && action == "Index")
        {
            await _next(context);
            return;
        }

        // Tạo mã quyền: Controller.Action (VD: SanPham.Xem hoặc Product.Index)
        var requiredPermission = $"{controller}.{action}";

        try
        {
            // Kiểm tra quyền
            var hasPermission = await unitOfWork.QuyenVaiTroRepository.HasPermissionAsync(roleId, requiredPermission);

            if (!hasPermission)
            {
                _logger.LogWarning("User {UserId} with role {RoleId} attempted to access {Controller}.{Action} but was denied",
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    roleId,
                    controller,
                    action);

                context.Response.Redirect("/Account/AccessDenied");
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission for {Controller}.{Action}", controller, action);
            // Trong trường hợp lỗi, cho phép truy cập để không block hệ thống
            await _next(context);
            return;
        }

        await _next(context);
    }
}
