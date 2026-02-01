namespace FreshNoodle.Core.DTOs;

public class FinancialStatsDto
{
    public decimal ExpectedRevenue { get; set; }
    public decimal ActualCollected { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal OverdueRevenue { get; set; }
    public int UnpaidCustomers { get; set; }
    
    public List<RevenuePointDto> RevenueHistory { get; set; } = new();
    public List<RevenuePointDto> TodayRevenueHistory { get; set; } = new();
}

public class RevenuePointDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
