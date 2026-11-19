using ProjectName.AdminService.Application.DTOs;

namespace ProjectName.AdminService.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDto?> GetByIdAsync(int id);
    Task<IEnumerable<AdminDto>> GetAllAsync();
    Task<bool> CreateAsync(CreateAdminDto dto);
    Task<bool> UpdateAsync(int id, UpdateAdminDto dto);
    Task<bool> DeleteAsync(int id);
}
