using AutoMapper;
using MediatR;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
namespace Sabemi.Application.Features.Contracts.GetAllContracts;

internal class GetAllContractsQueryHandler(IContractRepository repository, IMapper mapper) : IRequestHandler<GetAllContractsQuery, IEnumerable<ContractModel>>
{
    public async Task<IEnumerable<ContractModel>> Handle(GetAllContractsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Contract> contracts = await repository.FindAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<ContractModel>>(contracts);
    }
}
