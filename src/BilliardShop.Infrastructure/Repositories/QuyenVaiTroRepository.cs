using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces.Repositories;
using BilliardShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BilliardShop.Infrastructure.Repositories;

public class QuyenVaiTroRepository : GenericRepository<QuyenVaiTro>, IQuyenVaiTroRepository
{
    public QuyenVaiTroRepository(BilliardShopDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Quyen>> GetQuyenByVaiTroIdAsync(int vaiTroId)
    {
        return await _dbSet
            .Where(qv => qv.MaVaiTro == vaiTroId)
            .Include(qv => qv.Quyen)
            .Select(qv => qv.Quyen)
            .Where(q => q.TrangThaiHoatDong)
            .OrderBy(q => q.NhomQuyen)
            .ThenBy(q => q.ThuTuSapXep)
            .ToListAsync();
    }

    public async Task<IEnumerable<QuyenVaiTro>> GetByVaiTroIdAsync(int vaiTroId)
    {
        return await _dbSet
            .Include(qv => qv.Quyen)
            .Where(qv => qv.MaVaiTro == vaiTroId)
            .ToListAsync();
    }

    public async Task<bool> HasPermissionAsync(int vaiTroId, string maQuyen)
    {
        return await _dbSet
            .Include(qv => qv.Quyen)
            .AnyAsync(qv => qv.MaVaiTro == vaiTroId && qv.Quyen.MaQuyen == maQuyen && qv.Quyen.TrangThaiHoatDong);
    }

    public async Task DeleteByVaiTroIdAsync(int vaiTroId)
    {
        var permissions = await _dbSet
            .Where(qv => qv.MaVaiTro == vaiTroId)
            .ToListAsync();

        _dbSet.RemoveRange(permissions);
    }
}
