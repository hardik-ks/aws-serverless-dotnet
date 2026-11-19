using Dapper;
using ProjectName.AccountService.Domain.Entities;
using ProjectName.AccountService.Repository.Interfaces;
using ProjectName.Shared.Repository;

namespace ProjectName.AccountService.Repository.Implementations;

public class AccountRepository : RepositoryBase<Account, int>, IAccountRepository
{
    public AccountRepository(IConnectionFactory connectionFactory) 
        : base(connectionFactory) 
    { 
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Accounts WHERE Id = @Id AND IsActive = 1";

        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Account>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        var sql = "SELECT * FROM Accounts WHERE IsActive = 1 ORDER BY CreatedAt DESC";

        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<Account>(sql);
    }

    public async Task<bool> CreateAsync(Account entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var id = await InsertAsync(entity);
        return id > 0;
    }

    public async Task<bool> UpdateAsync(Account entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        return await base.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sql = "UPDATE Accounts SET IsActive = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";

        using var conn = connectionFactory.CreateConnection();
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }
}
