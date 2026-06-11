using AutoMapper;
using MediatR;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
using System.Linq.Expressions;
namespace Sabemi.Application.Features.Contracts.FindContracts;

internal class FindContractsQueryHandler(IContractRepository repository, IMapper mapper) : IRequestHandler<FindContractsQuery, IEnumerable<ContractModel>>
{
    public async Task<IEnumerable<ContractModel>> Handle(FindContractsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Contract> contracts = await repository.FindAsync(BuildFilter(request), cancellationToken);
        return mapper.Map<IEnumerable<ContractModel>>(contracts);
    }

    private static Expression<Func<Contract, bool>> BuildFilter(FindContractsQuery request)
    {
        return contract =>
        (request.Id == null || contract.Id == request.Id)
        &&
        (request.Name == null || contract.Name.Contains(request.Name));
    }
}
