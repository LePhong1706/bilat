using BilliardShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BilliardShop.Infrastructure.Data.Configurations;

public class QuyenVaiTroConfiguration : IEntityTypeConfiguration<QuyenVaiTro>
{
    public void Configure(EntityTypeBuilder<QuyenVaiTro> builder)
    {
        builder.ToTable("QuyenVaiTro");

        builder.HasKey(qv => qv.Id);

        // Composite index để tránh trùng lặp
        builder.HasIndex(qv => new { qv.MaVaiTro, qv.MaQuyen })
            .IsUnique();

        builder.Property(qv => qv.NgayGan)
            .HasDefaultValueSql("GETDATE()");

        // Relationships
        builder.HasOne(qv => qv.VaiTro)
            .WithMany(v => v.QuyenVaiTros)
            .HasForeignKey(qv => qv.MaVaiTro)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(qv => qv.Quyen)
            .WithMany(q => q.QuyenVaiTros)
            .HasForeignKey(qv => qv.MaQuyen)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
