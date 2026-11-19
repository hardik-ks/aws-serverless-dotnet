using ProjectName.AccountService.Application.DTOs;
using ProjectName.AccountService.Application.Interfaces;
using ProjectName.AccountService.Domain.Entities;
using ProjectName.AccountService.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace ProjectName.AccountService.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAccountRepository repository,
        ILogger<AccountService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AccountDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        return MapToDto(entity);
    }

    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<bool> CreateAsync(CreateAccountDto dto)
    {
        var entity = new Account
        {
            Name = dto.Name
        };

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAccountDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Name = dto.Name;

        return await _repository.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static AccountDto MapToDto(Account entity)
    {
        return new AccountDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsActive = entity.IsActive
        };
    }
}
