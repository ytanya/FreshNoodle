namespace FreshNoodle.Core.DTOs;

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;

    public List<int> RoleIds { get; set; } = new();
}
