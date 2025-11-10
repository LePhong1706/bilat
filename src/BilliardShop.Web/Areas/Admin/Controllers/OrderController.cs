using BilliardShop.Domain.Interfaces;
using BilliardShop.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BilliardShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class OrderController : BaseAdminController
{
    public OrderController(IUnitOfWork unitOfWork, ILogger<OrderController> logger)
        : base(unitOfWork, logger)
    {
    }

    // GET: Admin/Order
    public async Task<IActionResult> Index(string? search, int? statusId, DateTime? fromDate, DateTime? toDate, int page = 1)
    {
        try
        {
            int pageSize = 20;
            var query = _unitOfWork.DonHangRepository.Query()
                .Include(o => o.TrangThai)
                .Include(o => o.NguoiDung)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => o.SoDonHang.Contains(search) ||
                                        o.TenKhachHang.Contains(search) ||
                                        o.EmailKhachHang.Contains(search));
            }

            if (statusId.HasValue)
            {
                query = query.Where(o => o.MaTrangThai == statusId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.NgayDatHang >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.NgayDatHang <= toDate.Value.AddDays(1));
            }

            var orders = await query
                .OrderByDescending(o => o.NgayDatHang)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var statuses = await _unitOfWork.TrangThaiDonHangRepository.GetAllAsync();
            ViewBag.Statuses = new SelectList(statuses, "Id", "TenTrangThai", statusId);
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatusId = statusId;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.CurrentPage = page;

            return View(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading orders");
            ShowErrorMessage("Có lỗi xảy ra khi tải danh sách đơn hàng");
            return View(new List<Domain.Entities.DonHang>());
        }
    }

    // GET: Admin/Order/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var order = await _unitOfWork.DonHangRepository.Query()
                .Include(o => o.TrangThai)
                .Include(o => o.NguoiDung)
                .Include(o => o.PhuongThucThanhToan)
                .Include(o => o.PhuongThucVanChuyen)
                .Include(o => o.ChiTietDonHangs)
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                ShowErrorMessage("Không tìm thấy đơn hàng");
                return RedirectToAction(nameof(Index));
            }

            // Load tất cả trạng thái
            var statuses = await _unitOfWork.TrangThaiDonHangRepository.GetAllAsync();
            ViewBag.Statuses = new SelectList(statuses.OrderBy(s => s.ThuTuSapXep), "Id", "TenTrangThai", order.MaTrangThai);

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order details {OrderId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi tải chi tiết đơn hàng");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Order/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int orderId, int statusId, string? note)
    {
        try
        {
            var order = await _unitOfWork.DonHangRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            var oldStatusId = order.MaTrangThai;
            order.MaTrangThai = statusId;

            // Cập nhật trạng thái thanh toán
            if (statusId == 5) // Đã giao
            {
                order.NgayGiaoHang = DateTime.Now;
                if (order.TrangThaiThanhToan == "ChoThanhToan")
                {
                    order.TrangThaiThanhToan = "DaThanhToan";
                }
            }
            else if (statusId == 6) // Đã hủy
            {
                // Hoàn lại số lượng tồn kho
                var orderItems = await _unitOfWork.ChiTietDonHangRepository
                    .FindAsync(c => c.MaDonHang == orderId);

                foreach (var item in orderItems)
                {
                    var product = await _unitOfWork.SanPhamRepository.GetByIdAsync(item.MaSanPham);
                    if (product != null)
                    {
                        product.SoLuongTonKho += item.SoLuong;
                        _unitOfWork.SanPhamRepository.Update(product);

                        // Tạo biến động kho
                        var bienDong = new Domain.Entities.BienDongKhoHang
                        {
                            MaSanPham = product.Id,
                            LoaiBienDong = "NHAP",
                            SoLuong = item.SoLuong,
                            TonKhoTruoc = product.SoLuongTonKho - item.SoLuong,
                            TonKhoSau = product.SoLuongTonKho,
                            ThamChieu = $"Hủy đơn #{order.SoDonHang}",
                            GhiChu = note,
                            NgayTao = DateTime.Now,
                            NguoiThucHien = GetCurrentUserId()
                        };
                        await _unitOfWork.BienDongKhoHangRepository.AddAsync(bienDong);
                    }
                }

                order.TrangThaiThanhToan = "HoanTien";
            }

            if (!string.IsNullOrEmpty(note))
            {
                order.GhiChuQuanTri = note;
            }

            _unitOfWork.DonHangRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Đã cập nhật trạng thái đơn hàng thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status {OrderId}", orderId);
            return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
        }
    }

    // GET: Admin/Order/Print/5
    public async Task<IActionResult> Print(int id)
    {
        try
        {
            var order = await _unitOfWork.DonHangRepository.Query()
                .Include(o => o.TrangThai)
                .Include(o => o.NguoiDung)
                .Include(o => o.PhuongThucThanhToan)
                .Include(o => o.PhuongThucVanChuyen)
                .Include(o => o.ChiTietDonHangs)
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                ShowErrorMessage("Không tìm thấy đơn hàng");
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error printing order {OrderId}", id);
            ShowErrorMessage("Có lỗi xảy ra khi in đơn hàng");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Admin/Order/Statistics
    public async Task<IActionResult> Statistics(DateTime? fromDate, DateTime? toDate)
    {
        try
        {
            var from = fromDate ?? DateTime.Now.AddMonths(-1);
            var to = toDate ?? DateTime.Now;

            var orders = await _unitOfWork.DonHangRepository.Query()
                .Include(o => o.TrangThai)
                .Where(o => o.NgayDatHang >= from && o.NgayDatHang <= to)
                .ToListAsync();

            var model = new OrderStatisticsViewModel
            {
                FromDate = from,
                ToDate = to,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Where(o => o.MaTrangThai == 5).Sum(o => o.TongThanhToan),
                PendingOrders = orders.Count(o => o.MaTrangThai == 1),
                ProcessingOrders = orders.Count(o => o.MaTrangThai == 2 || o.MaTrangThai == 3),
                CompletedOrders = orders.Count(o => o.MaTrangThai == 5),
                CancelledOrders = orders.Count(o => o.MaTrangThai == 6),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TongThanhToan) : 0
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order statistics");
            ShowErrorMessage("Có lỗi xảy ra khi tải thống kê đơn hàng");
            return View(new OrderStatisticsViewModel());
        }
    }
}
