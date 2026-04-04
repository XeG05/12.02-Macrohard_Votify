using System.Linq.Expressions;

namespace Votify.Persistencia.Interfaces;

public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(long id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

}