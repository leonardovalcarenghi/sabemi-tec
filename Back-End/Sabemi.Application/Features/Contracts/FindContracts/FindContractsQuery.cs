using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace Sabemi.Application.Features.Contracts.FindContracts;

[BindProperties]
public class FindContractsQuery : IRequest<IEnumerable<ContractModel>>
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }

}
