namespace ProjectName.AccountService.Application.DTOs;

public class AccountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAccountDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateAccountDto
{
    public string Name { get; set; } = string.Empty;
}
