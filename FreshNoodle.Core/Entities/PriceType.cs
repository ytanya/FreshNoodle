using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.Core.Entities;

public class PriceType : ISoftDeletable, IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DefaultPricePerKg { get; set; }
    public bool IsActive { get; set; } = true;
}
