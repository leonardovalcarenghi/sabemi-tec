using AutoMapper;
using MediatR;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Exceptions;
using Sabemi.Domain.Interfaces.Repositories;
namespace Sabemi.Application.Features.Contracts.FindContractById;

internal class FindContractByIdQueryHandler(IContractRepository repository, IMapper mapper) : IRequestHandler<FindContractByIdQuery, ContractModel>
{
    public async Task<ContractModel> Handle(FindContractByIdQuery request, CancellationToken cancellationToken)
    {
        Contract? contract = await repository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"O contrato com ID {request.Id} não foi encontrado.");

        return mapper.Map<ContractModel>(contract);
    }
}
