using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using FreshNoodle.Infrastructure.Data;

namespace FreshNoodle.API.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly FreshNoodleDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, FreshNoodleDbContext context, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _context = context;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameWithRolesAsync(loginDto.Username);

        if (user == null || !user.IsActive || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Authentication failure for user: {Username}", loginDto.Username);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }


        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            }
        };
    }

    public async Task<AuthResponseDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Check if username already exists
        var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
        if (existingUser != null)
        {
            return new AuthResponseDto { Success = false, Message = "Username already exists" };
        }

        // Check if email already exists
        var existingEmail = await _userRepository.GetByEmailAsync(createUserDto.Email);
        if (existingEmail != null)
        {
            return new AuthResponseDto { Success = false, Message = "Email already exists" };
        }

        var passwordError = ValidatePassword(createUserDto.Password);
        if (passwordError != null)
        {
            return new AuthResponseDto { Success = false, Message = passwordError };
        }

        var user = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            PasswordHash = HashPassword(createUserDto.Password),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };


        await _userRepository.AddAsync(user);
        _logger.LogInformation("New user created: {Username} by Admin action", user.Username);


        // Assign roles
        if (createUserDto.RoleIds != null && createUserDto.RoleIds.Any())
        {
            foreach (var roleId in createUserDto.RoleIds)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }
            await _context.SaveChangesAsync();
        }

        return new AuthResponseDto { Success = true, Message = "User created successfully" };
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userRepository.GetByIdAsync(resetPasswordDto.UserId);
        if (user == null) return false;

        var passwordError = ValidatePassword(resetPasswordDto.NewPassword);
        if (passwordError != null) return false;

        user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Password reset for user ID: {UserId} by Admin action", user.Id);


        // Mark any reset requests as completed
        var requests = await _context.PasswordResetRequests
            .Where(r => r.UserId == user.Id && !r.IsCompleted)
            .ToListAsync();

        foreach (var req in requests)
        {
            req.IsCompleted = true;
            req.CompletedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return true; // Security: don't reveal if user exists

        var verificationCode = new Random().Next(100000, 999999).ToString();
        var request = new PasswordResetRequest
        {
            UserId = user.Id,
            RequestedAt = DateTime.UtcNow,
            VerificationCode = verificationCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        _context.PasswordResetRequests.Add(request);
        await _context.SaveChangesAsync();
        
        // Log the code for testing purposes as per implementation plan
        _logger.LogInformation("Password reset requested for {Email}. Verification Code: {Code}", email, verificationCode);

        return true;
    }

    public async Task<bool> VerifyResetCodeAsync(string email, string code)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        var request = await _context.PasswordResetRequests
            .Where(r => r.UserId == user.Id && !r.IsCompleted && r.VerificationCode == code && r.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(r => r.RequestedAt)
            .FirstOrDefaultAsync();

        return request != null;
    }

    public async Task<bool> CompletePasswordResetAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
        if (user == null) return false;

        var request = await _context.PasswordResetRequests
            .Where(r => r.UserId == user.Id && !r.IsCompleted && r.VerificationCode == resetPasswordDto.VerificationCode && r.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(r => r.RequestedAt)
            .FirstOrDefaultAsync();

        if (request == null) return false;

        var passwordError = ValidatePassword(resetPasswordDto.NewPassword);
        if (passwordError != null) return false;

        user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
        await _userRepository.UpdateAsync(user);

        request.IsCompleted = true;
        request.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Password reset completed for user: {Email}", resetPasswordDto.Email);

        return true;
    }

    public string? ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return "Password must be at least 8 characters long.";

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return "Password must contain at least one uppercase letter.";

        if (!Regex.IsMatch(password, @"[a-z]"))
            return "Password must contain at least one lowercase letter.";

        if (!Regex.IsMatch(password, @"[0-9]"))
            return "Password must contain at least one digit.";

        if (!Regex.IsMatch(password, @"[\W_]"))
            return "Password must contain at least one special character.";

        return null;
    }

    public async Task<bool> LogoutAsync()
    {
        return await Task.FromResult(true);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "FreshNoodleAPI";
        var audience = jwtSettings["Audience"] ?? "FreshNoodleClient";

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

