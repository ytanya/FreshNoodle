using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.Core.Entities;

public class ProductionDay : IEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsClosed { get; set; }
}
