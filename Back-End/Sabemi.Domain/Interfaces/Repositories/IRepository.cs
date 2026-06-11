using System.Linq.Expressions;

namespace Sabemi.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)  ;
}

