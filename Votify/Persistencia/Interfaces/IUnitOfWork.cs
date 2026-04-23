namespace Votify.Persistencia.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task SaveChangesAsync();
    }
}
