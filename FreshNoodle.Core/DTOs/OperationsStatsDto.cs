namespace FreshNoodle.Core.DTOs;

public class OperationsStatsDto
{
    public int TotalProduced { get; set; }
    public int RetailReserved { get; set; }
    public List<CustomerDeliveryDto> Deliveries { get; set; } = new();
}

public class CustomerDeliveryDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Delivered, Skipped
    public int? Priority { get; set; }
}
