using Dapper.Contrib.Extensions;

namespace ProjectName.AdminService.Domain.Entities;

[Table("Admins")]
public class Admin
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}
