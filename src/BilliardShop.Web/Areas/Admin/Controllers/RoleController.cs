using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces;
using BilliardShop.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RoleController : BaseAdminController
{
    public RoleController(IUnitOfWork unitOfWork, ILogger<RoleController> logger)
        : base(unitOfWork, logger)
    {
    }

    // GET: Admin/Role
    public async Task<IActionResult> Index()
    {
        try
        {
            var roles = await _unitOfWork.VaiTroNguoiDungRepository.GetAllAsync();
            return View(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading roles");
            ShowErrorMessage("Có lỗi xảy ra khi tải danh sách vai trò");
            return View(new List<VaiTroNguoiDung>());
        }
    }

    // GET: Admin/Role/Create
    public IActionResult Create()
    {
        return View(new RoleViewModel());
    }

    // POST: Admin/Role/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var role = new VaiTroNguoiDung
            {
                TenVaiTro = model.TenVaiTro,
                MoTa = model.MoTa,
                TrangThaiHoatDong = model.TrangThaiHoatDong,
                NgayTao = DateTime.Now,
                NguoiTao = GetCurrentUserId()
            };

            await _unitOfWork.VaiTroNguoiDungRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã thêm vai trò '{role.TenVaiTro}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            ShowErrorMessage("Có lỗi xảy ra khi thêm vai trò");
            return View(model);
        }
    }

    // GET: Admin/Role/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var role = await _unitOfWork.VaiTroNguoiDungRepository.GetByIdAsync(id);
            if (role == null)
            {
                ShowErrorMessage("Không tìm thấy vai trò");
                return RedirectToAction(nameof(Index));
            }

            var model = new RoleViewModel
            {
                Id = role.Id,
                TenVaiTro = role.TenVaiTro,
                MoTa = role.MoTa,
                TrangThaiHoatDong = role.TrangThaiHoatDong
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading role {RoleId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi tải thông tin vai trò");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Role/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RoleViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var role = await _unitOfWork.VaiTroNguoiDungRepository.GetByIdAsync(id);
            if (role == null)
            {
                ShowErrorMessage("Không tìm thấy vai trò");
                return RedirectToAction(nameof(Index));
            }

            role.TenVaiTro = model.TenVaiTro;
            role.MoTa = model.MoTa;
            role.TrangThaiHoatDong = model.TrangThaiHoatDong;
            role.NgayCapNhatCuoi = DateTime.Now;
            role.NguoiCapNhatCuoi = GetCurrentUserId();

            _unitOfWork.VaiTroNguoiDungRepository.Update(role);
            await _unitOfWork.SaveChangesAsync();

            ShowSuccessMessage($"Đã cập nhật vai trò '{role.TenVaiTro}' thành công");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi cập nhật vai trò");
            return View(model);
        }
    }

    // GET: Admin/Role/Permissions/5
    public async Task<IActionResult> Permissions(int id)
    {
        try
        {
            var role = await _unitOfWork.VaiTroNguoiDungRepository.GetByIdAsync(id);
            if (role == null)
            {
                ShowErrorMessage("Không tìm thấy vai trò");
                return RedirectToAction(nameof(Index));
            }

            // Lấy tất cả quyền trong hệ thống
            var allPermissions = await _unitOfWork.QuyenRepository.GetQuyenHoatDongAsync();

            // Lấy quyền hiện tại của vai trò
            var rolePermissions = await _unitOfWork.QuyenVaiTroRepository.GetQuyenByVaiTroIdAsync(id);
            var rolePermissionIds = rolePermissions.Select(p => p.Id).ToHashSet();

            // Nhóm quyền theo module
            var permissionGroups = allPermissions
                .GroupBy(p => p.NhomQuyen)
                .OrderBy(g => g.Key)
                .Select(g => new PermissionGroupViewModel
                {
                    GroupName = g.Key,
                    Permissions = g.Select(p => new PermissionItemViewModel
                    {
                        Id = p.Id,
                        MaQuyen = p.MaQuyen,
                        TenQuyen = p.TenQuyen,
                        HanhDong = p.MaQuyen,
                        MoTa = p.MoTa,
                        IsSelected = rolePermissionIds.Contains(p.Id)
                    }).ToList()
                })
                .ToList();

            var model = new RolePermissionsViewModel
            {
                RoleId = role.Id,
                RoleName = role.TenVaiTro,
                PermissionGroups = permissionGroups,
                AssignedPermissionIds = rolePermissionIds.ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading permissions for role {RoleId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi tải danh sách quyền");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Role/Permissions/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Permissions(int id, List<int> selectedPermissions)
    {
        try
        {
            var role = await _unitOfWork.VaiTroNguoiDungRepository.GetByIdAsync(id);
            if (role == null)
            {
                return Json(new { success = false, message = "Không tìm thấy vai trò" });
            }

            // Xóa tất cả quyền hiện tại
            await _unitOfWork.QuyenVaiTroRepository.DeleteByVaiTroIdAsync(id);

            // Thêm quyền mới
            if (selectedPermissions != null && selectedPermissions.Any())
            {
                foreach (var permissionId in selectedPermissions)
                {
                    var rolePermission = new QuyenVaiTro
                    {
                        MaVaiTro = id,
                        MaQuyen = permissionId,
                        NgayGan = DateTime.Now
                    };

                    await _unitOfWork.QuyenVaiTroRepository.AddAsync(rolePermission);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã cập nhật quyền cho vai trò '{role.TenVaiTro}' thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permissions for role {RoleId}", id);
            return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật quyền" });
        }
    }

    // POST: Admin/Role/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var role = await _unitOfWork.VaiTroNguoiDungRepository.GetByIdAsync(id);
            if (role == null)
            {
                return Json(new { success = false, message = "Không tìm thấy vai trò" });
            }

            // Kiểm tra xem có user nào đang dùng vai trò này không
            var usersWithRole = await _unitOfWork.NguoiDungRepository.CountAsync(u => u.MaVaiTro == id);
            if (usersWithRole > 0)
            {
                return Json(new { success = false, message = $"Không thể xóa vai trò vì có {usersWithRole} người dùng đang sử dụng" });
            }

            _unitOfWork.VaiTroNguoiDungRepository.Remove(role);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã xóa vai trò '{role.TenVaiTro}' thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return Json(new { success = false, message = "Có lỗi xảy ra khi xóa vai trò" });
        }
    }
}
