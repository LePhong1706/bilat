using BilliardShop.Domain.Interfaces;
using System.Security.Claims;

namespace BilliardShop.Web.Helpers;

/// <summary>
/// Helper class để kiểm tra quyền của người dùng
/// </summary>
public class PermissionHelper
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Dictionary<string, bool>? _userPermissionsCache;

    public PermissionHelper(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Kiểm tra xem người dùng có quyền với mã quyền cụ thể hay không
    /// </summary>
    public async Task<bool> HasPermissionAsync(string permissionCode)
    {
        if (string.IsNullOrEmpty(permissionCode))
            return false;

        // Lấy roleId từ claims
        var roleIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("RoleId")?.Value;
        if (string.IsNullOrEmpty(roleIdClaim) || !int.TryParse(roleIdClaim, out int roleId))
            return false;

        // Kiểm tra cache
        if (_userPermissionsCache == null)
        {
            await LoadUserPermissionsAsync(roleId);
        }

        return _userPermissionsCache?.ContainsKey(permissionCode) == true && _userPermissionsCache[permissionCode];
    }

    /// <summary>
    /// Tải tất cả quyền của người dùng vào cache
    /// </summary>
    private async Task LoadUserPermissionsAsync(int roleId)
    {
        _userPermissionsCache = new Dictionary<string, bool>();

        // Lấy tất cả quyền của vai trò này
        var rolePermissions = await _unitOfWork.QuyenVaiTroRepository.FindAsync(
            qvt => qvt.MaVaiTro == roleId,
            qvt => qvt.Quyen
        );

        foreach (var rolePermission in rolePermissions)
        {
            if (rolePermission.Quyen != null && rolePermission.Quyen.TrangThaiHoatDong)
            {
                _userPermissionsCache[rolePermission.Quyen.MaQuyen] = true;
            }
        }
    }

    /// <summary>
    /// Kiểm tra xem người dùng có bất kỳ quyền nào trong danh sách hay không
    /// </summary>
    public async Task<bool> HasAnyPermissionAsync(params string[] permissionCodes)
    {
        foreach (var code in permissionCodes)
        {
            if (await HasPermissionAsync(code))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Kiểm tra xem người dùng có tất cả các quyền trong danh sách hay không
    /// </summary>
    public async Task<bool> HasAllPermissionsAsync(params string[] permissionCodes)
    {
        foreach (var code in permissionCodes)
        {
            if (!await HasPermissionAsync(code))
                return false;
        }
        return true;
    }
}
