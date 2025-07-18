using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;
using SDTicaret.Infrastructure.Data;

namespace SDTicaret.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SDTicaretDbContext _context;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(SDTicaretDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class, IEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 
