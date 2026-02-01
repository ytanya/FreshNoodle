using FreshNoodle.Core.Entities;
using FreshNoodle.Core.Interfaces;
using FreshNoodle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshNoodle.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(FreshNoodleDbContext context) : base(context)
    {
    }


    public async Task<IEnumerable<Customer>> GetAllWithTypesAsync()
    {
        return await _context.Customers
            .Include(c => c.CustomerType)
            .Include(c => c.PriceType)
            .Include(c => c.PaymentType)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllWithTypesAndInactiveAsync()
    {
        return await _context.Customers
            .IgnoreQueryFilters()
            .Include(c => c.CustomerType)
            .Include(c => c.PriceType)
            .Include(c => c.PaymentType)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdWithTypesAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.CustomerType)
            .Include(c => c.PriceType)
            .Include(c => c.PaymentType)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByIdWithTypesAndInactiveAsync(int id)
    {
        return await _context.Customers
            .IgnoreQueryFilters()
            .Include(c => c.CustomerType)
            .Include(c => c.PriceType)
            .Include(c => c.PaymentType)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

}
