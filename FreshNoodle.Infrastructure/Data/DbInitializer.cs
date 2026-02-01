using FreshNoodle.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreshNoodle.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedDataAsync(FreshNoodleDbContext context)
    {
        // Ensure database is created
        _ = await context.Database.EnsureCreatedAsync();

        // Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            List<Role> roles = new()
            {
                new Role { Name = "Admin", Description = "Full system access" },
                new Role { Name = "Accounting", Description = "Financial and Billing access" },
                new Role { Name = "Delivery", Description = "Logistics and Delivery access" }
            };
            await context.Roles.AddRangeAsync(roles);
            _ = await context.SaveChangesAsync();
        }

        // Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            Role adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            User adminUser = new()
            {
                Username = "admin",
                Email = "admin@freshnoodle.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _ = await context.Users.AddAsync(adminUser);
            _ = await context.SaveChangesAsync();

            _ = await context.UserRoles.AddAsync(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
            _ = await context.SaveChangesAsync();
        }

        // Seed Customer Types
        if (!await context.CustomerTypes.AnyAsync())
        {
            List<CustomerType> types = new()
            {
                new CustomerType { Name = "Retail", Description = "Individual consumers", IsActive = true },
                new CustomerType { Name = "Wholesale", Description = "Restaurants and resellers", IsActive = true },
                new CustomerType { Name = "Corporate", Description = "Office and institutional accounts", IsActive = true }
            };
            await context.CustomerTypes.AddRangeAsync(types);
        }

        // Seed Price Types
        if (!await context.PriceTypes.AnyAsync())
        {
            List<PriceType> prices = new()
            {
                new PriceType { Name = "Standard", DefaultPricePerKg = 5.0m, IsActive = true },
                new PriceType { Name = "Bulk Discount", DefaultPricePerKg = 4.2m, IsActive = true },
                new PriceType { Name = "Premium", DefaultPricePerKg = 6.5m, IsActive = true }
            };
            await context.PriceTypes.AddRangeAsync(prices);
        }

        // Seed Payment Types
        if (!await context.PaymentTypes.AnyAsync())
        {
            List<PaymentType> payments = new()
            {
                new PaymentType { Name = "Cash", Description = "Payment upon delivery", IsActive = true },
                new PaymentType { Name = "Bank Transfer", Description = "Monthly invoicing", IsActive = true },
                new PaymentType { Name = "Mobile Pay", Description = "Instant digital payment", IsActive = true }
            };
            await context.PaymentTypes.AddRangeAsync(payments);
        }

        _ = await context.SaveChangesAsync();
    }
}
