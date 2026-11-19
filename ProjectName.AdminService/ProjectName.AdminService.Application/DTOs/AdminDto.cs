namespace ProjectName.AdminService.Application.DTOs;

public class AdminDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAdminDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateAdminDto
{
    public string Name { get; set; } = string.Empty;
}
