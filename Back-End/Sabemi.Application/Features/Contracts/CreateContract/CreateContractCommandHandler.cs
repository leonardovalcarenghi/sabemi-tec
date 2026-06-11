using AutoMapper;
using MediatR;
using Sabemi.Application.Abstractions;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Enums;
using Sabemi.Domain.Interfaces.Repositories;
namespace Sabemi.Application.Features.Contracts.CreateContract;

internal class CreateContractCommandHandler(
    IContractRepository repository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    INotificationService notificationService
) : IRequestHandler<CreateContractCommand, Guid>
{
    public async Task<Guid> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        Contract contract = Contract.Create(request.Name, request.TotalAmount);
        contract.Status = ContractStatus.Create(contract.Id, EContractStatus.Pending);

        if (await repository.ExistsByNameAsync(contract.Name, cancellationToken))
            throw new InvalidOperationException($"O contrato com o nome '{contract.Name}' já existe.");

        repository.Add(contract);

        if (!await unitOfWork.SaveChangesAsync(cancellationToken))
            throw new InvalidOperationException("Ocorreu um erro ao salvar o contrato. Por favor, tente novamente.");

        await notificationService.NotifyContractCreatedAsync(contract.Id, cancellationToken);

        return contract.Id;
    }
}

