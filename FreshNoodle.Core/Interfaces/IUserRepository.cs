using FreshNoodle.Core.Entities;

namespace FreshNoodle.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameWithRolesAsync(string username);
    Task<User?> GetByIdWithRolesAsync(int id);
    Task<List<User>> GetAllWithRolesAsync();
}

