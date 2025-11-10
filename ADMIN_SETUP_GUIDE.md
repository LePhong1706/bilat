# 🎯 HƯỚNG DẪN HOÀN THIỆN HỆ THỐNG ADMIN

## ✅ ĐÃ HOÀN THÀNH

### 1. Hệ thống phân quyền động
- ✅ Models: `Quyen`, `QuyenVaiTro`
- ✅ Repositories: `QuyenRepository`, `QuyenVaiTroRepository`
- ✅ Đã cập nhật UnitOfWork và DbContext
- ✅ Migration đã tạo: `AddPermissionSystem`

### 2. Middleware & Authentication
- ✅ `PermissionMiddleware` - Kiểm tra quyền tự động
- ✅ `PermissionAttribute` - Attribute cho controller/action
- ✅ Đã cấu hình trong Program.cs

### 3. Area Admin
- ✅ BaseAdminController với các helper methods
- ✅ AuthController (Login, Logout, AccessDenied)
- ✅ DashboardController với thống kê
- ✅ ProductController (CRUD sản phẩm)
- ✅ RoleController (CRUD vai trò & phân quyền)

### 4. Template & Assets
- ✅ Template Vyzor đã copy vào wwwroot/assets
- ✅ Views: Login, AccessDenied

### 5. Seed Data
- ✅ `PermissionSeeder` - 65+ quyền mặc định

---

## 🔧 CẦN HOÀN THIỆN

### BƯỚC 1: Apply Migration

```bash
cd src/BilliardShop.Infrastructure
dotnet ef database update --startup-project ../BilliardShop.Web
```

### BƯỚC 2: Seed Permissions

Trong `Program.cs`, thêm code seed sau (sau `var app = builder.Build();`):

```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BilliardShopDbContext>();
        await PermissionSeeder.SeedPermissionsAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding permissions.");
    }
}
```

### BƯỚC 3: Tạo Admin Layout

Tạo file: `Areas/Admin/Views/Shared/_Layout.cshtml`

```cshtml
@inject IUnitOfWork _unitOfWork
<!DOCTYPE html>
<html lang="vi" dir="ltr" data-nav-layout="vertical" data-theme-mode="light">
<head>
    <meta charset="UTF-8">
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>@ViewData["Title"] - Admin</title>

    <link rel="icon" href="~/assets/images/brand-logos/favicon.ico" type="image/x-icon">
    <link href="~/assets/libs/bootstrap/css/bootstrap.min.css" rel="stylesheet">
    <link href="~/assets/css/styles.css" rel="stylesheet">
    <link href="~/assets/css/icons.css" rel="stylesheet">
    @await RenderSectionAsync("Styles", required: false)
</head>
<body>
    <div class="page">
        <!-- Header -->
        <header class="app-header">
            <div class="main-header-container container-fluid">
                <div class="header-content-left">
                    <div class="header-element">
                        <a href="@Url.Action("Index", "Dashboard", new { area = "Admin" })">
                            <span class="desktop-logo">
                                <h3>BilliardShop Admin</h3>
                            </span>
                        </a>
                    </div>
                </div>

                <div class="header-content-right">
                    <div class="header-element">
                        <span>@User.FindFirst("FullName")?.Value</span>
                    </div>
                    <div class="header-element">
                        <form asp-area="Admin" asp-controller="Auth" asp-action="Logout" method="post">
                            <button type="submit" class="btn btn-sm btn-danger">
                                <i class="ri-logout-box-line"></i> Đăng xuất
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </header>

        <!-- Sidebar -->
        <aside class="app-sidebar sticky" id="sidebar">
            <div class="main-sidebar">
                <nav class="main-menu-container nav nav-pills flex-column sub-open">
                    <ul class="main-menu">
                        <li class="slide">
                            <a asp-area="Admin" asp-controller="Dashboard" asp-action="Index" class="side-menu__item">
                                <i class="ri-dashboard-line side-menu__icon"></i>
                                <span class="side-menu__label">Dashboard</span>
                            </a>
                        </li>
                        <li class="slide">
                            <a asp-area="Admin" asp-controller="Product" asp-action="Index" class="side-menu__item">
                                <i class="ri-product-hunt-line side-menu__icon"></i>
                                <span class="side-menu__label">Sản phẩm</span>
                            </a>
                        </li>
                        <li class="slide">
                            <a asp-area="Admin" asp-controller="Order" asp-action="Index" class="side-menu__item">
                                <i class="ri-shopping-cart-line side-menu__icon"></i>
                                <span class="side-menu__label">Đơn hàng</span>
                            </a>
                        </li>
                        <li class="slide">
                            <a asp-area="Admin" asp-controller="User" asp-action="Index" class="side-menu__item">
                                <i class="ri-user-line side-menu__icon"></i>
                                <span class="side-menu__label">Người dùng</span>
                            </a>
                        </li>
                        <li class="slide">
                            <a asp-area="Admin" asp-controller="Role" asp-action="Index" class="side-menu__item">
                                <i class="ri-shield-user-line side-menu__icon"></i>
                                <span class="side-menu__label">Vai trò & Phân quyền</span>
                            </a>
                        </li>
                    </ul>
                </nav>
            </div>
        </aside>

        <!-- Main Content -->
        <div class="main-content app-content">
            <div class="container-fluid">
                @if (TempData["SuccessMessage"] != null)
                {
                    <div class="alert alert-success alert-dismissible fade show" role="alert">
                        @TempData["SuccessMessage"]
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                }
                @if (TempData["ErrorMessage"] != null)
                {
                    <div class="alert alert-danger alert-dismissible fade show" role="alert">
                        @TempData["ErrorMessage"]
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                }

                @RenderBody()
            </div>
        </div>
    </div>

    <script src="~/assets/libs/bootstrap/js/bootstrap.bundle.min.js"></script>
    <script src="~/assets/js/main.js"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

### BƯỚC 4: Tạo Dashboard View

Tạo file: `Areas/Admin/Views/Dashboard/Index.cshtml`

```cshtml
@model DashboardViewModel
@{
    ViewData["Title"] = "Dashboard";
}

<div class="page-header-breadcrumb mb-3">
    <h1 class="page-title">Dashboard</h1>
</div>

<div class="row">
    <div class="col-xl-3 col-lg-6 col-md-6">
        <div class="card custom-card">
            <div class="card-body">
                <div class="d-flex align-items-top">
                    <div class="flex-fill">
                        <span class="d-block fs-13">Tổng sản phẩm</span>
                        <h3 class="mb-0">@Model.TotalProducts</h3>
                    </div>
                    <div class="ms-2">
                        <span class="avatar avatar-md bg-primary-transparent">
                            <i class="ri-product-hunt-line fs-20"></i>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-xl-3 col-lg-6 col-md-6">
        <div class="card custom-card">
            <div class="card-body">
                <div class="d-flex align-items-top">
                    <div class="flex-fill">
                        <span class="d-block fs-13">Tổng đơn hàng</span>
                        <h3 class="mb-0">@Model.TotalOrders</h3>
                    </div>
                    <div class="ms-2">
                        <span class="avatar avatar-md bg-success-transparent">
                            <i class="ri-shopping-cart-line fs-20"></i>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-xl-3 col-lg-6 col-md-6">
        <div class="card custom-card">
            <div class="card-body">
                <div class="d-flex align-items-top">
                    <div class="flex-fill">
                        <span class="d-block fs-13">Khách hàng</span>
                        <h3 class="mb-0">@Model.TotalCustomers</h3>
                    </div>
                    <div class="ms-2">
                        <span class="avatar avatar-md bg-warning-transparent">
                            <i class="ri-user-line fs-20"></i>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-xl-3 col-lg-6 col-md-6">
        <div class="card custom-card">
            <div class="card-body">
                <div class="d-flex align-items-top">
                    <div class="flex-fill">
                        <span class="d-block fs-13">Doanh thu</span>
                        <h3 class="mb-0">@Model.TotalRevenue.ToString("N0") đ</h3>
                    </div>
                    <div class="ms-2">
                        <span class="avatar avatar-md bg-danger-transparent">
                            <i class="ri-money-dollar-circle-line fs-20"></i>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="row">
    <div class="col-xl-12">
        <div class="card custom-card">
            <div class="card-header">
                <div class="card-title">Đơn hàng gần đây</div>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-bordered">
                        <thead>
                            <tr>
                                <th>Mã đơn</th>
                                <th>Khách hàng</th>
                                <th>Ngày đặt</th>
                                <th>Tổng tiền</th>
                                <th>Trạng thái</th>
                            </tr>
                        </thead>
                        <tbody>
                            @foreach (var order in Model.RecentOrders)
                            {
                                <tr>
                                    <td>@order.SoDonHang</td>
                                    <td>@order.TenKhachHang</td>
                                    <td>@order.NgayDatHang.ToString("dd/MM/yyyy")</td>
                                    <td>@order.TongThanhToan.ToString("N0") đ</td>
                                    <td><span class="badge bg-primary">Đang xử lý</span></td>
                                </tr>
                            }
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
```

---

## 📝 CÁC CONTROLLERS CẦN TẠO THÊM

### 1. UserController (Quản lý người dùng)
```csharp
// Areas/Admin/Controllers/UserController.cs
- Index() - Danh sách user
- Create() - Thêm user
- Edit(id) - Sửa user
- Delete(id) - Xóa user
- ResetPassword(id) - Reset password
```

### 2. CategoryController (Quản lý danh mục)
```csharp
// Areas/Admin/Controllers/CategoryController.cs
- Index() - Danh sách danh mục (tree view)
- Create() - Thêm danh mục
- Edit(id) - Sửa danh mục
- Delete(id) - Xóa danh mục
```

### 3. BrandController (Quản lý thương hiệu)
```csharp
// Areas/Admin/Controllers/BrandController.cs
- Index() - Danh sách thương hiệu
- Create() - Thêm thương hiệu
- Edit(id) - Sửa thương hiệu
- Delete(id) - Xóa thương hiệu
```

### 4. OrderController (Quản lý đơn hàng)
```csharp
// Areas/Admin/Controllers/OrderController.cs
- Index() - Danh sách đơn hàng
- Details(id) - Chi tiết đơn hàng
- UpdateStatus(id, status) - Cập nhật trạng thái
- Cancel(id) - Hủy đơn
- Print(id) - In đơn hàng
```

### 5. InventoryController (Quản lý kho)
```csharp
// Areas/Admin/Controllers/InventoryController.cs
- Index() - Danh sách tồn kho
- Import() - Nhập kho
- Export() - Xuất kho
- Adjust() - Điều chỉnh kho
- History() - Lịch sử biến động
```

### 6-10. Các controllers còn lại
- SupplierController (Nhà cung cấp)
- DiscountController (Mã giảm giá)
- ReviewController (Đánh giá)
- ArticleController (Bài viết)
- SettingsController (Cài đặt)

---

## 🎨 HƯỚNG DẪN TẠO VIEWS

### Cấu trúc Views cần tạo:

```
Areas/Admin/Views/
├── Shared/
│   ├── _Layout.cshtml ✅ (tạo theo hướng dẫn trên)
│   ├── _Sidebar.cshtml (partial cho sidebar)
│   └── _Header.cshtml (partial cho header)
├── Dashboard/
│   └── Index.cshtml ✅ (tạo theo hướng dẫn trên)
├── Product/
│   ├── Index.cshtml (danh sách)
│   ├── Create.cshtml (thêm mới)
│   ├── Edit.cshtml (sửa)
│   └── _ProductForm.cshtml (partial form)
├── Role/
│   ├── Index.cshtml (danh sách vai trò)
│   ├── Create.cshtml
│   ├── Edit.cshtml
│   └── Permissions.cshtml (giao diện phân quyền dạng matrix)
├── User/
│   ├── Index.cshtml
│   ├── Create.cshtml
│   └── Edit.cshtml
└── ... (các views khác tương tự)
```

### Template cho Product/Index.cshtml:

```cshtml
@model IEnumerable<SanPham>
@{
    ViewData["Title"] = "Quản lý Sản phẩm";
}

<div class="page-header-breadcrumb mb-3">
    <div class="d-flex align-items-center justify-content-between">
        <h1 class="page-title">Quản lý Sản phẩm</h1>
        <a asp-action="Create" class="btn btn-primary">
            <i class="ri-add-line"></i> Thêm sản phẩm
        </a>
    </div>
</div>

<div class="row">
    <div class="col-xl-12">
        <div class="card custom-card">
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-bordered" id="productTable">
                        <thead>
                            <tr>
                                <th>Mã SP</th>
                                <th>Tên sản phẩm</th>
                                <th>Danh mục</th>
                                <th>Giá</th>
                                <th>Tồn kho</th>
                                <th>Trạng thái</th>
                                <th>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            @foreach (var product in Model)
                            {
                                <tr>
                                    <td>@product.MaCodeSanPham</td>
                                    <td>@product.TenSanPham</td>
                                    <td>@product.DanhMuc?.TenDanhMuc</td>
                                    <td>@product.GiaGoc.ToString("N0")đ</td>
                                    <td>@product.SoLuongTonKho</td>
                                    <td>
                                        @if (product.TrangThaiHoatDong)
                                        {
                                            <span class="badge bg-success">Hoạt động</span>
                                        }
                                        else
                                        {
                                            <span class="badge bg-danger">Ngưng</span>
                                        }
                                    </td>
                                    <td>
                                        <a asp-action="Edit" asp-route-id="@product.Id" class="btn btn-sm btn-warning">
                                            <i class="ri-edit-line"></i>
                                        </a>
                                        <button class="btn btn-sm btn-danger" onclick="deleteProduct(@product.Id)">
                                            <i class="ri-delete-bin-line"></i>
                                        </button>
                                    </td>
                                </tr>
                            }
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script>
        function deleteProduct(id) {
            if (confirm('Bạn có chắc muốn xóa sản phẩm này?')) {
                $.ajax({
                    url: '@Url.Action("Delete", "Product", new { area = "Admin" })',
                    type: 'POST',
                    data: { id: id },
                    success: function(result) {
                        if (result.success) {
                            alert(result.message);
                            location.reload();
                        } else {
                            alert(result.message);
                        }
                    }
                });
            }
        }
    </script>
}
```

---

## 🚀 TESTING

### 1. Build và Run

```bash
dotnet build
dotnet run --project src/BilliardShop.Web
```

### 2. Truy cập Admin

```
URL: https://localhost:5001/Admin
```

### 3. Tạo user admin đầu tiên (qua SQL)

```sql
-- Tạo user admin (password: admin123)
INSERT INTO NguoiDung (TenDangNhap, Email, MatKhauMaHoa, MuoiMatKhau, Ho, Ten, MaVaiTro, TrangThaiHoatDong)
VALUES ('admin', 'admin@billiardshop.vn',
        'hash_password_here',
        'salt_here',
        'Admin', 'System', 1, 1)
```

### 4. Gán full quyền cho admin role

```sql
INSERT INTO QuyenVaiTro (MaVaiTro, MaQuyen, NgayGan)
SELECT 1, Id, GETDATE() FROM Quyen WHERE TrangThaiHoatDong = 1
```

---

## 📚 TÀI LIỆU THAM KHẢO

- Template Vyzor: `/Template/final/Views/`
- Các component có sẵn: Datatables, ApexCharts, SweetAlert2
- Icon set: Remixicon (ri-*)

---

## ✨ GỢI Ý MỞ RỘNG

1. **Upload nhiều ảnh**: Sử dụng Dropzone.js
2. **Rich text editor**: TinyMCE hoặc CKEditor
3. **Export Excel**: EPPlus hoặc NPOI
4. **Real-time notifications**: SignalR
5. **Activity Log**: Ghi log mọi thao tác admin
6. **Two-Factor Authentication**: Google Authenticator
7. **File Manager**: Quản lý file upload
8. **Backup/Restore**: Backup database tự động

---

**Chúc bạn hoàn thành dự án thành công! 🎉**
