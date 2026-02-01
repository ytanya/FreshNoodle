using FreshNoodle.Core.DTOs;
using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using FreshNoodle.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshNoodle.API.Authentication.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin,Accounting")]
public class DashboardController : ControllerBase
{
    private readonly FreshNoodleDbContext _context;

    public DashboardController(FreshNoodleDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var today = DateTime.Today;
        
        var stats = new DashboardDto
        {
            ActiveCustomerCount = await _context.Customers.CountAsync(c => c.IsActive),
            ActiveUserCount = await _context.Users.CountAsync(u => u.IsActive),
            IsTodayClosed = await _context.ProductionDays
                .AnyAsync(p => p.Date.Date == today && p.IsClosed),
            
            InactivePaymentTypeCount = await _context.PaymentTypes.CountAsync(p => !p.IsActive),
            
            // Interpret "Missing price types" as customers assigned to an inactive price type or no price types assigned (if that was possible)
            // For now, let's count customers assigned to inactive price types.
            CustomersWithNoPriceTypeCount = await _context.Customers
                .CountAsync(c => !c.PriceType.IsActive)
        };

        return Ok(stats);
    }

    [HttpGet("financials")]
    public IActionResult GetFinancialStats()
    {
        // Mocking financial data as requested in C2.1
        var stats = new FinancialStatsDto
        {
            ExpectedRevenue = 12500.50m,
            ActualCollected = 8900.25m,
            OutstandingBalance = 3600.25m,
            OverdueRevenue = 1200.75m,
            UnpaidCustomers = 14,
            RevenueHistory = new List<RevenuePointDto>
            {
                new() { Date = "2026-01-20", Amount = 1200 },
                new() { Date = "2026-01-21", Amount = 1500 },
                new() { Date = "2026-01-22", Amount = 1100 },
                new() { Date = "2026-01-23", Amount = 1800 },
                new() { Date = "2026-01-24", Amount = 2200 },
                new() { Date = "2026-01-25", Amount = 1900 },
                new() { Date = "2026-01-26", Amount = 2500 }
            },
            TodayRevenueHistory = new List<RevenuePointDto>
            {
                new() { Date = "08:00", Amount = 400 },
                new() { Date = "10:00", Amount = 1200 },
                new() { Date = "12:00", Amount = 2500 },
                new() { Date = "14:00", Amount = 1800 },
                new() { Date = "16:00", Amount = 3000 }
            }
        };

        return Ok(stats);
    }

    [HttpGet("operations")]
    public async Task<IActionResult> GetOperationsStats()
    {
        // Mocking operations data for today
        var customers = await _context.Customers
            .Where(c => c.IsActive)
            .OrderBy(c => c.PriorityOrder ?? 999)
            .Take(10) // Limit for overview
            .ToListAsync();

        var stats = new OperationsStatsDto
        {
            TotalProduced = 450,
            RetailReserved = 120,
            Deliveries = customers.Select(c => new CustomerDeliveryDto
            {
                CustomerId = c.Id,
                CustomerName = c.Name,
                Status = c.SkipDay ? "Skipped" : (c.Id % 3 == 0 ? "Delivered" : "Pending"),
                Priority = c.PriorityOrder
            }).ToList()
        };

        return Ok(stats);
    }
}


