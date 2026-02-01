namespace FreshNoodle.Core.DTOs;

public class ResetPasswordDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
