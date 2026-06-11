using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sabemi.Application.Features.Contracts;
using Sabemi.Application.Features.Contracts.CreateContract;
using Sabemi.Application.Features.Contracts.FindContractById;
using Sabemi.Application.Features.Contracts.FindContracts;
namespace Sabemi.Api.Controllers;

[ApiController]
[Route("contracts")]
[Tags("Contratos")]
public class ContractController(IMediator mediator) : ControllerBase 
{
    [HttpGet]
    [EndpointSummary("Buscar Contratos")]
    [EndpointDescription("Busca todos os contratos cadastrados no sistema.")]
    public async Task<IActionResult> Find([FromQuery] FindContractsQuery query)
    {
        IEnumerable<ContractModel> contracts = await mediator.Send(query);
        return Ok(contracts);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Buscar Contrato por ID")]
    [EndpointDescription("Busca um contrato específico pelo seu ID.")]
    public async Task<IActionResult> FindById([FromRoute] Guid id)
    {
        ContractModel contract = await mediator.Send(new FindContractByIdQuery(id));
        return Ok(contract);
    }

    [HttpPost]
    [EndpointSummary("Criar Contrato")]
    [EndpointDescription("Cria um novo contrato com as informações fornecidas.")]
    public async Task<IActionResult> Create([FromBody] CreateContractCommand command)
    {
        Guid newContractId = await mediator.Send(command);
        return CreatedAtAction(nameof(FindById), new { id = newContractId }, null);
    }
}
