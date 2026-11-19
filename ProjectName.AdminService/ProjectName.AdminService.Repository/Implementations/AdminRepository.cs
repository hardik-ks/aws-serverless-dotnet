using Dapper;
using ProjectName.AdminService.Domain.Entities;
using ProjectName.AdminService.Repository.Interfaces;
using ProjectName.Shared.Repository;

namespace ProjectName.AdminService.Repository.Implementations;

public class AdminRepository : RepositoryBase<Admin, int>, IAdminRepository
{
    public AdminRepository(IConnectionFactory connectionFactory) 
        : base(connectionFactory) 
    { 
    }

    public async Task<Admin?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Admins WHERE Id = @Id AND IsActive = 1";

        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Admin>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Admin>> GetAllAsync()
    {
        var sql = "SELECT * FROM Admins WHERE IsActive = 1 ORDER BY CreatedAt DESC";

        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<Admin>(sql);
    }

    public async Task<bool> CreateAsync(Admin entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var id = await InsertAsync(entity);
        return id > 0;
    }

    public async Task<bool> UpdateAsync(Admin entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        return await base.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sql = "UPDATE Admins SET IsActive = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";

        using var conn = connectionFactory.CreateConnection();
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }
}
