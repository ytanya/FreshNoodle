namespace FreshNoodle.Core.Entities;

public class PasswordResetRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string? VerificationCode { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public string? CompletedByUsername { get; set; }
}
