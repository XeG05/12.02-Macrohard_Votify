using System.Linq.Expressions;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<T?> GetByIdAsync(long id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(long id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
