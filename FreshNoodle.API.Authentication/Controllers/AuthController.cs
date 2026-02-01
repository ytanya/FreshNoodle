using Microsoft.AspNetCore.Mvc;
using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.API.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(loginDto);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        if (string.IsNullOrEmpty(forgotPasswordDto.Email))
            return BadRequest(new { message = "Email is required" });

        await _authService.RequestPasswordResetAsync(forgotPasswordDto.Email);
        return Ok(new { message = "If an account exists, a verification code has been sent." });
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto verifyCodeDto)
    {
        if (string.IsNullOrEmpty(verifyCodeDto.Email) || string.IsNullOrEmpty(verifyCodeDto.Code))
            return BadRequest(new { message = "Email and Code are required" });

        var isValid = await _authService.VerifyResetCodeAsync(verifyCodeDto.Email, verifyCodeDto.Code);
        if (!isValid)
            return BadRequest(new { message = "Invalid or expired verification code" });

        return Ok(new { message = "Code verified successfully" });
    }

    [HttpPost("reset-password-with-code")]
    public async Task<IActionResult> ResetPasswordWithCode([FromBody] ResetPasswordDto resetPasswordDto)
    {
        if (string.IsNullOrEmpty(resetPasswordDto.Email) || string.IsNullOrEmpty(resetPasswordDto.VerificationCode) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
            return BadRequest(new { message = "Email, Code, and New Password are required" });

        var success = await _authService.CompletePasswordResetAsync(resetPasswordDto);
        if (!success)
            return BadRequest(new { message = "Failed to reset password. Code may be invalid, expired, or password does not meet requirements." });

        return Ok(new { message = "Password reset successfully" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { message = "Logout successful" });
    }
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class VerifyCodeDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

