using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
using Sabemi.Infra.Persistence.Contexts;
namespace Sabemi.Infra.Persistence.Repositories;

internal class ContractRepository(ApplicationDbContext dbContext) : Repository<Contract>(dbContext), IContractRepository
{
}
