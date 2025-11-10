using BilliardShop.Domain.Entities;

namespace BilliardShop.Domain.Interfaces.Repositories;

public interface IQuyenRepository : IGenericRepository<Quyen>
{
    Task<IEnumerable<Quyen>> GetQuyenByNhomAsync(string nhomQuyen);
    Task<IEnumerable<Quyen>> GetQuyenHoatDongAsync();
    Task<Quyen?> GetByMaQuyenAsync(string maQuyen);
}
