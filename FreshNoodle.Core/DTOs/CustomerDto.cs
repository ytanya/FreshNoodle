namespace FreshNoodle.Core.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public int CustomerTypeId { get; set; }
    public string? CustomerTypeName { get; set; }
    
    public int PriceTypeId { get; set; }
    public string? PriceTypeName { get; set; }
    
    public int PaymentTypeId { get; set; }
    public string? PaymentTypeName { get; set; }
    
    public bool SkipDay { get; set; }
    public int? PriorityOrder { get; set; }
    public bool IsActive { get; set; }
}
