using Microsoft.EntityFrameworkCore;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
using Sabemi.Infra.Persistence.Contexts;
using System.Linq.Expressions;
namespace Sabemi.Infra.Persistence.Repositories;

internal class ContractRepository(ApplicationDbContext dbContext) : Repository<Contract>(dbContext), IContractRepository
{
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
        => _context.Contracts.AnyAsync(c => c.Name == name, cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, Guid excludeId, CancellationToken cancellationToken)
        => _context.Contracts.AnyAsync(c => c.Name == name && c.Id != excludeId, cancellationToken);
}
