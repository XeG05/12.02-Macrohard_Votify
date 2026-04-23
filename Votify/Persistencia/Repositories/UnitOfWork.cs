using Votify.Persistencia.Interfaces;

namespace Votify.Persistencia.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly VotifyContext _context;
    private readonly Dictionary<Type, object> _repos = new();

    public UnitOfWork(VotifyContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repos.ContainsKey(type))
            _repos[type] = new Repository<T>(_context);
        return (IRepository<T>)_repos[type];
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}