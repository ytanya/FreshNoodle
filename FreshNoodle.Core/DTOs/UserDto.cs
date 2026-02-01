namespace FreshNoodle.Core.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}

