using ProjectName.AccountService.Domain.Entities;
using ProjectName.Shared.Repository;

namespace ProjectName.AccountService.Repository.Interfaces;

public interface IAccountRepository : IRepositoryBase<Account, int>
{
    Task<Account?> GetByIdAsync(int id);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<bool> CreateAsync(Account entity);
    Task<bool> UpdateAsync(Account entity);
    Task<bool> DeleteAsync(int id);
}
