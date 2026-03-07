using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.API.User.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedDate = user.CreatedDate
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            CreatedDate = u.CreatedDate
        });
    }

    public async Task<UserDto> CreateUserAsync(RegisterDto registerDto)
    {
        var user = new Core.Entities.User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            CreatedDate = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Email = createdUser.Email,
            CreatedDate = createdUser.CreatedDate
        };
    }

    public async Task<UserDto> UpdateUserAsync(int id, UserDto userDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.Username = userDto.Username;
        user.Email = userDto.Email;

        var updatedUser = await _userRepository.UpdateAsync(user);

        return new UserDto
        {
            Id = updatedUser.Id,
            Username = updatedUser.Username,
            Email = updatedUser.Email,
            CreatedDate = updatedUser.CreatedDate
        };
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }
}
