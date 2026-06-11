using Sabemi.Domain.Entities;
using System.Linq.Expressions;

namespace Sabemi.Domain.Interfaces.Repositories;

public interface IContractRepository : IRepository<Contract>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string name, Guid excludeId, CancellationToken cancellationToken);
}
