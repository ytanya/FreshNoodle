using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using FreshNoodle.Infrastructure.Data;

namespace FreshNoodle.API.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly FreshNoodleDbContext _context;

    public AdminController(
        IAuthService authService, 
        IUserRepository userRepository, 
        IRepository<Role> roleRepository,
        FreshNoodleDbContext context)
    {
        _authService = authService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _context = context;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        // Admins should see all users including inactive ones
        var users = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();

        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            CreatedDate = u.CreatedDate,
            IsActive = u.IsActive,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        });

        return Ok(userDtos);
    }


    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate roles are active
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            var inactiveRoles = await _context.Roles
                .Where(r => dto.RoleIds.Contains(r.Id) && !r.IsActive)
                .Select(r => r.Name)
                .ToListAsync();

            if (inactiveRoles.Any())
                return BadRequest(new { message = $"Cannot assign inactive roles: {string.Join(", ", inactiveRoles)}" });
        }

        var result = await _authService.CreateUserAsync(dto);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        if (id != dto.Id) return BadRequest(new { message = "ID mismatch" });

        var user = await _userRepository.GetByIdWithInactiveAsync(id);

        if (user == null) return NotFound();

        // Check email uniqueness if changed
        if (user.Email != dto.Email)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null) return BadRequest(new { message = "Email already in use" });
            user.Email = dto.Email;
        }

        user.Username = dto.Username;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;

        await _userRepository.UpdateAsync(user);
        return Ok(new { message = "User updated successfully" });
    }

    [HttpPut("users/{id}/roles")]

    public async Task<IActionResult> UpdateUserRoles(int id, [FromBody] List<int> roleIds)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(id);
        if (user == null) return NotFound();

        // Validate roles are active - Use IgnoreQueryFilters to see them
        var inactiveRoles = await _context.Roles
            .IgnoreQueryFilters()
            .Where(r => roleIds.Contains(r.Id) && !r.IsActive)
            .Select(r => r.Name)
            .ToListAsync();


        if (inactiveRoles.Any())
            return BadRequest(new { message = $"Cannot assign inactive roles: {string.Join(", ", inactiveRoles)}" });

        // Update UserRoles
        var existingUserRoles = _context.UserRoles.Where(ur => ur.UserId == id);
        _context.UserRoles.RemoveRange(existingUserRoles);

        foreach (var roleId in roleIds)
        {
            _context.UserRoles.Add(new UserRole { UserId = id, RoleId = roleId });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "User roles updated successfully" });
    }

    [HttpPost("users/{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
    {
        if (id != dto.UserId)
            return BadRequest(new { message = "User ID mismatch" });

        var success = await _authService.ResetPasswordAsync(dto);
        if (!success)
            return BadRequest(new { message = "Failed to reset password. Ensure it meets policy requirements." });

        return Ok(new { message = "Password reset successful" });
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleRepository.GetAllWithInactiveAsync();
        return Ok(roles.Select(r => new RoleDto 
        { 
            Id = r.Id, 
            Name = r.Name, 
            Description = r.Description,
            IsActive = r.IsActive
        }));
    }


    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            return BadRequest(new { message = "Role name is required" });

        var existing = await _context.Roles.AnyAsync(r => r.Name == dto.Name);
        if (existing)
            return BadRequest(new { message = "Role name already exists" });

        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true
        };

        await _roleRepository.AddAsync(role);
        return Ok(new RoleDto { Id = role.Id, Name = role.Name, Description = role.Description, IsActive = role.IsActive });
    }

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto dto)
    {
        var role = await _roleRepository.GetByIdWithInactiveAsync(id);

        if (role == null) return NotFound();

        if (role.Name != dto.Name)
        {
            var existing = await _context.Roles.AnyAsync(r => r.Name == dto.Name && r.Id != id);
            if (existing)
                return BadRequest(new { message = "Role name already exists" });
            role.Name = dto.Name;
        }

        role.Description = dto.Description;
        role.IsActive = dto.IsActive;

        await _roleRepository.UpdateAsync(role);
        return Ok(new RoleDto { Id = role.Id, Name = role.Name, Description = role.Description, IsActive = role.IsActive });
    }


    [HttpGet("password-reset-requests")]
    public async Task<IActionResult> GetPasswordResetRequests()
    {
        var requests = await _context.PasswordResetRequests
            .Include(r => r.User)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();

        return Ok(requests.Select(r => new 
        {
            r.Id,
            r.UserId,
            Username = r.User.Username,
            r.RequestedAt,
            r.IsCompleted,
            r.CompletedAt
        }));
    }
}
