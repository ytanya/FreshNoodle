using FreshNoodle.Core.DTOs;

namespace FreshNoodle.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> LogoutAsync();
    Task<AuthResponseDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<bool> VerifyResetCodeAsync(string email, string code);
    Task<bool> CompletePasswordResetAsync(ResetPasswordDto resetPasswordDto);
    string? ValidatePassword(string password);
}

