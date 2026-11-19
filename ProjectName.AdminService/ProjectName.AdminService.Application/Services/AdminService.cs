using ProjectName.AdminService.Application.DTOs;
using ProjectName.AdminService.Application.Interfaces;
using ProjectName.AdminService.Domain.Entities;
using ProjectName.AdminService.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace ProjectName.AdminService.Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _repository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IAdminRepository repository,
        ILogger<AdminService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AdminDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        return MapToDto(entity);
    }

    public async Task<IEnumerable<AdminDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<bool> CreateAsync(CreateAdminDto dto)
    {
        var entity = new Admin
        {
            Name = dto.Name
        };

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAdminDto dto)
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

    private static AdminDto MapToDto(Admin entity)
    {
        return new AdminDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsActive = entity.IsActive
        };
    }
}
