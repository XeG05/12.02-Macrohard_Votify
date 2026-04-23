using System.Linq.Expressions;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class Service<T> : IService<T> where T : class
    {
        protected readonly IRepository<T> _repository;
        protected readonly IUnitOfWork _uow;

        public Service(IRepository<T> repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                await _uow.SaveChangesAsync();
            }

        }
    }
}
