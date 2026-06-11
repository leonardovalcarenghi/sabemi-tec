using MediatR;
namespace Sabemi.Application.Features.Contracts.FindContractById;

public record FindContractByIdQuery(Guid Id) : IRequest<ContractModel>
{
}
