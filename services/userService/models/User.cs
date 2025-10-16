namespace userService.Models;
public class User
{
    public required Guid Id { get; set; }
    public required string PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? BankAccountNumber { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
}