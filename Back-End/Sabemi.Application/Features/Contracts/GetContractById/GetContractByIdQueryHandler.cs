using AutoMapper;
using MediatR;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
namespace Sabemi.Application.Features.Contracts.GetContractById;

internal class GetContractByIdQueryHandler(IContractRepository repository, IMapper mapper) : IRequestHandler<GetContractByIdQuery, ContractModel>
{
    public async Task<ContractModel> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        Contract? contract = await repository.FindByIdAsync(request.Id, cancellationToken);
        if (contract == null)
        {
            // Handle not found case, e.g., throw an exception or return a default value

            // aqui  eu retorno null ou uma exception personalizada?
        }

        return mapper.Map<ContractModel>(contract);
    }
}
