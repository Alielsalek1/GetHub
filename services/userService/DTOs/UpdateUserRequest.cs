namespace userService.DTOs;

public class UpdateUserRequest
{
    public string? bankAccountNumber { get; set; }
    public string? bio { get; set; }
    public string? phoneNumber { get; set; }
    public string? address { get; set; }
    public string? profileImageUrl { get; set; }
}