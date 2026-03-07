using System.Linq.Expressions;

namespace Votify.Modelos.BusinessLogic.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
