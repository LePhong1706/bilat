using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilliardShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "ThuongHieu");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "ThuongHieu");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "ThuongHieu");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "SuDungMaGiamGia");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "SuDungMaGiamGia");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "SuDungMaGiamGia");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "SuDungMaGiamGia");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "NhaCungCap");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "NhaCungCap");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "NhaCungCap");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "MaGiamGia");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "MaGiamGia");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "HinhAnhSanPham");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "HinhAnhSanPham");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "HinhAnhSanPham");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "GioHang");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "GioHang");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "DonHang");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "DonHang");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "DonHang");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "DiaChiNguoiDung");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "DiaChiNguoiDung");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "DiaChiNguoiDung");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "DanhSachYeuThich");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "DanhSachYeuThich");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "DanhSachYeuThich");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "DanhMucSanPham");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "DanhMucSanPham");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "DanhMucSanPham");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "DanhGiaSanPham");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "DanhGiaSanPham");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "DanhGiaSanPham");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "CaiDatHeThong");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "CaiDatHeThong");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "BinhLuanBaiViet");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "BinhLuanBaiViet");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "BinhLuanBaiViet");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatCuoi",
                table: "BienDongKhoHang");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "BienDongKhoHang");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "BienDongKhoHang");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatCuoi",
                table: "BaiViet");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "BaiViet");

            migrationBuilder.CreateTable(
                name: "Quyen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaQuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenQuyen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NhomQuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThuTuSapXep = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhatCuoi = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    NguoiTao = table.Column<int>(type: "int", nullable: true),
                    NguoiCapNhatCuoi = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quyen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuyenVaiTro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    MaQuyen = table.Column<int>(type: "int", nullable: false),
                    NgayGan = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NguoiGan = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyenVaiTro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuyenVaiTro_Quyen_MaQuyen",
                        column: x => x.MaQuyen,
                        principalTable: "Quyen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuyenVaiTro_VaiTroNguoiDung_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTroNguoiDung",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quyen_MaQuyen",
                table: "Quyen",
                column: "MaQuyen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuyenVaiTro_MaQuyen",
                table: "QuyenVaiTro",
                column: "MaQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_QuyenVaiTro_MaVaiTro_MaQuyen",
                table: "QuyenVaiTro",
                columns: new[] { "MaVaiTro", "MaQuyen" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuyenVaiTro");

            migrationBuilder.DropTable(
                name: "Quyen");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "VaiTroNguoiDung",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "VaiTroNguoiDung",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "VaiTroNguoiDung",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "ThuongHieu",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "ThuongHieu",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "ThuongHieu",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "SuDungMaGiamGia",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "SuDungMaGiamGia",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "SuDungMaGiamGia",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "SuDungMaGiamGia",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "SanPham",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "NhaCungCap",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "NhaCungCap",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "NhaCungCap",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "NguoiDung",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "NguoiDung",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "MaGiamGia",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "MaGiamGia",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "HinhAnhSanPham",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "HinhAnhSanPham",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "HinhAnhSanPham",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "GioHang",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "GioHang",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "DonHang",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "DonHang",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "DonHang",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "DiaChiNguoiDung",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "DiaChiNguoiDung",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "DiaChiNguoiDung",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "DanhSachYeuThich",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "DanhSachYeuThich",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "DanhSachYeuThich",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "DanhMucSanPham",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "DanhMucSanPham",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "DanhMucSanPham",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "DanhGiaSanPham",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "DanhGiaSanPham",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "DanhGiaSanPham",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "CaiDatHeThong",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "CaiDatHeThong",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "BinhLuanBaiViet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "BinhLuanBaiViet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "BinhLuanBaiViet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatCuoi",
                table: "BienDongKhoHang",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "BienDongKhoHang",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "BienDongKhoHang",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatCuoi",
                table: "BaiViet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiTao",
                table: "BaiViet",
                type: "int",
                nullable: true);
        }
    }
}
