using Microsoft.EntityFrameworkCore;
using Sabemi.Domain.Interfaces;
using Sabemi.Domain.Interfaces.Repositories;
using Sabemi.Infra.Persistence.Contexts;
using System.Linq.Expressions;
namespace Sabemi.Infra.Persistence.Repositories;

internal class Repository<T>(ApplicationDbContext dbContext) : IRepository<T> where T : class, IEntity
{
    protected readonly ApplicationDbContext _context = dbContext;

    public void Add(T entity)
        => _context.Set<T>().Add(entity);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public void Remove(T entity)
        => _context.Set<T>().Remove(entity);

    public Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Set<T>().FindAsync([id], cancellationToken).AsTask();

    public async Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<T>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Set<T>().Where(predicate).ToListAsync(cancellationToken);
}

