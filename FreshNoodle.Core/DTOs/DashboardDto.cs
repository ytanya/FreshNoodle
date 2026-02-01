namespace FreshNoodle.Core.DTOs;

public class DashboardDto
{
    public int ActiveCustomerCount { get; set; }
    public int ActiveUserCount { get; set; }
    public bool IsTodayClosed { get; set; }
    
    public int InactivePaymentTypeCount { get; set; }
    public int CustomersWithNoPriceTypeCount { get; set; } // Or customers with inactive price types
}
