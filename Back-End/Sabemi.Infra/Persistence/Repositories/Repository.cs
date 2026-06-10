using Sabemi.Domain.Interfaces;
using Sabemi.Domain.Interfaces.Repositories;
using Sabemi.Infra.Persistence.Contexts;
namespace Sabemi.Infra.Persistence.Repositories;

internal class Repository<T>(ApplicationDbContext dbContext) : IRepository<T> where T : class, IEntity
{
    protected readonly ApplicationDbContext _context = dbContext;

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Set<T>().FindAsync([id], cancellationToken).AsTask();
    }

    public Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_context.Set<T>().AsEnumerable());
    }
}

