using System.Linq.Expressions;
using SDTicaret.Core.Entities;

namespace SDTicaret.Core.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    void Delete(T entity);
} 