using BilliardShop.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public abstract class BaseAdminController : Controller
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ILogger _logger;

    protected BaseAdminController(IUnitOfWork unitOfWork, ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Lấy ID người dùng hiện tại
    /// </summary>
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }

    /// <summary>
    /// Lấy tên người dùng hiện tại
    /// </summary>
    protected string GetCurrentUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
    }

    /// <summary>
    /// Lấy ID vai trò người dùng hiện tại
    /// </summary>
    protected int GetCurrentUserRoleId()
    {
        var roleIdClaim = User.FindFirst("RoleId")?.Value;
        return int.TryParse(roleIdClaim, out int roleId) ? roleId : 0;
    }

    /// <summary>
    /// Hiển thị thông báo thành công
    /// </summary>
    protected void ShowSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    /// <summary>
    /// Hiển thị thông báo lỗi
    /// </summary>
    protected void ShowErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    /// <summary>
    /// Hiển thị thông báo cảnh báo
    /// </summary>
    protected void ShowWarningMessage(string message)
    {
        TempData["WarningMessage"] = message;
    }

    /// <summary>
    /// Hiển thị thông báo thông tin
    /// </summary>
    protected void ShowInfoMessage(string message)
    {
        TempData["InfoMessage"] = message;
    }

    /// <summary>
    /// Kiểm tra xem user có quyền hay không
    /// </summary>
    protected async Task<bool> HasPermissionAsync(string permissionCode)
    {
        var roleId = GetCurrentUserRoleId();
        if (roleId == 0) return false;

        return await _unitOfWork.QuyenVaiTroRepository.HasPermissionAsync(roleId, permissionCode);
    }
}
