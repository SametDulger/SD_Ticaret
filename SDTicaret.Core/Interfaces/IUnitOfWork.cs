using SDTicaret.Core.Entities;

namespace SDTicaret.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class, IEntity;
    Task<int> SaveChangesAsync();
} 