using FreshNoodle.Core.Entities;

namespace FreshNoodle.Core.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> GetAllWithTypesAsync();
    Task<IEnumerable<Customer>> GetAllWithTypesAndInactiveAsync();
    Task<Customer?> GetByIdWithTypesAsync(int id);
    Task<Customer?> GetByIdWithTypesAndInactiveAsync(int id);

}
