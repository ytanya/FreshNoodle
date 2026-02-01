using FreshNoodle.Core.Interfaces;

namespace FreshNoodle.Core.Entities;

public class Customer : ISoftDeletable, IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public int CustomerTypeId { get; set; }
    public CustomerType CustomerType { get; set; } = null!;
    
    public int PriceTypeId { get; set; }
    public PriceType PriceType { get; set; } = null!;
    
    public int PaymentTypeId { get; set; }
    public PaymentType PaymentType { get; set; } = null!;
    
    public bool SkipDay { get; set; }
    public int? PriorityOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
