using ProjectName.AccountService.Application.DTOs;

namespace ProjectName.AccountService.Application.Interfaces;

public interface IAccountService
{
    Task<AccountDto?> GetByIdAsync(int id);
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<bool> CreateAsync(CreateAccountDto dto);
    Task<bool> UpdateAsync(int id, UpdateAccountDto dto);
    Task<bool> DeleteAsync(int id);
}
