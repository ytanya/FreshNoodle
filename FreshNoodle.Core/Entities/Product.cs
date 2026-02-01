using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.Core.Entities;

public class Product : ISoftDeletable, IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

