namespace FreshNoodle.Core.Interfaces;

public interface IRepository<T> where T : class, IEntity

{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdWithInactiveAsync(int id);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllWithInactiveAsync();
    Task<T> AddAsync(T entity);

    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}
