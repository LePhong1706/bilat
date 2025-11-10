using BilliardShop.Domain.Entities;

namespace BilliardShop.Domain.Interfaces.Repositories;

public interface IQuyenVaiTroRepository : IGenericRepository<QuyenVaiTro>
{
    Task<IEnumerable<Quyen>> GetQuyenByVaiTroIdAsync(int vaiTroId);
    Task<IEnumerable<QuyenVaiTro>> GetByVaiTroIdAsync(int vaiTroId);
    Task<bool> HasPermissionAsync(int vaiTroId, string maQuyen);
    Task DeleteByVaiTroIdAsync(int vaiTroId);
}
