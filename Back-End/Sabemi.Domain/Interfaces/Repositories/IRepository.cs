namespace Sabemi.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken = default);
}
