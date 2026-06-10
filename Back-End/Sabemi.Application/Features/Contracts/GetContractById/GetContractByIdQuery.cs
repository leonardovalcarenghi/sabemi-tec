using MediatR;
namespace Sabemi.Application.Features.Contracts.GetContractById;

public record GetContractByIdQuery(Guid Id) : IRequest<ContractModel>
{
}
