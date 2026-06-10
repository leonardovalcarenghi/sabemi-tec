using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sabemi.Application.Features.Contracts;
using Sabemi.Application.Features.Contracts.GetAllContracts;
using Sabemi.Application.Features.Contracts.GetContractById;
namespace Sabemi.Api.Controllers;

[ApiController]
[Route("contracts")]
public class ContractController(IMediator mediator) : ControllerBase 
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<ContractModel> contracts = await mediator.Send(new GetAllContractsQuery());
        return Ok(contracts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        ContractModel contract = await mediator.Send(new GetContractByIdQuery(id));
        return Ok(contract);
    }
}
