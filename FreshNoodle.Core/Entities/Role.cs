using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.Core.Entities;

public class Role : ISoftDeletable, IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

