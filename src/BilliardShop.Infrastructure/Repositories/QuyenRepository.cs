using BilliardShop.Domain.Entities;
using BilliardShop.Domain.Interfaces.Repositories;
using BilliardShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BilliardShop.Infrastructure.Repositories;

public class QuyenRepository : GenericRepository<Quyen>, IQuyenRepository
{
    public QuyenRepository(BilliardShopDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Quyen>> GetQuyenByNhomAsync(string nhomQuyen)
    {
        return await _dbSet
            .Where(q => q.NhomQuyen == nhomQuyen && q.TrangThaiHoatDong)
            .OrderBy(q => q.ThuTuSapXep)
            .ToListAsync();
    }

    public async Task<IEnumerable<Quyen>> GetQuyenHoatDongAsync()
    {
        return await _dbSet
            .Where(q => q.TrangThaiHoatDong)
            .OrderBy(q => q.NhomQuyen)
            .ThenBy(q => q.ThuTuSapXep)
            .ToListAsync();
    }

    public async Task<Quyen?> GetByMaQuyenAsync(string maQuyen)
    {
        return await _dbSet
            .FirstOrDefaultAsync(q => q.MaQuyen == maQuyen);
    }
}
