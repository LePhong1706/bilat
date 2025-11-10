using BilliardShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BilliardShop.Infrastructure.Data.Configurations;

public class QuyenConfiguration : IEntityTypeConfiguration<Quyen>
{
    public void Configure(EntityTypeBuilder<Quyen> builder)
    {
        builder.ToTable("Quyen");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.MaQuyen)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(q => q.MaQuyen)
            .IsUnique();

        builder.Property(q => q.TenQuyen)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.MoTa)
            .HasMaxLength(500);

        builder.Property(q => q.NhomQuyen)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(q => q.HanhDong)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(q => q.ThuTuSapXep)
            .HasDefaultValue(0);

        builder.Property(q => q.TrangThaiHoatDong)
            .HasDefaultValue(true);

        builder.Property(q => q.NgayTao)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(q => q.NgayCapNhatCuoi)
            .HasDefaultValueSql("GETDATE()");

        // Relationships
        builder.HasMany(q => q.QuyenVaiTros)
            .WithOne(qv => qv.Quyen)
            .HasForeignKey(qv => qv.MaQuyen)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
