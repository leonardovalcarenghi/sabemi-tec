using MediatR;
namespace Sabemi.Application.Features.Contracts.GetAllContracts;

public class GetAllContractsQuery : IRequest<IEnumerable<ContractModel>>
{
}
