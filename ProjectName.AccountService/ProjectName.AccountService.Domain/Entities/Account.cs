using Dapper.Contrib.Extensions;

namespace ProjectName.AccountService.Domain.Entities;

[Table("Accounts")]
public class Account
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}
