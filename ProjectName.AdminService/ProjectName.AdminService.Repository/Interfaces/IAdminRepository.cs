using ProjectName.AdminService.Domain.Entities;
using ProjectName.Shared.Repository;

namespace ProjectName.AdminService.Repository.Interfaces;

public interface IAdminRepository : IRepositoryBase<Admin, int>
{
    Task<Admin?> GetByIdAsync(int id);
    Task<IEnumerable<Admin>> GetAllAsync();
    Task<bool> CreateAsync(Admin entity);
    Task<bool> UpdateAsync(Admin entity);
    Task<bool> DeleteAsync(int id);
}
