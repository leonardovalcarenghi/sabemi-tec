using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sabemi.Application.Features.Webhooks;
using Sabemi.Application.Features.Webhooks.FindPayments;
using Sabemi.Application.Features.Webhooks.ReprocessPayment;
namespace Sabemi.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("payment-events")]
[Tags("Eventos de Pagamento")]
public class PaymentEventController(IMediator mediator) : ControllerBase
{

    [HttpGet]
    [EndpointSummary("Buscar Eventos de Pagamento")]
    [EndpointDescription("Busca todos os eventos de pagamento registrados no sistema.")]
    public async Task<IActionResult> Find([FromQuery] FindPaymentEventCommand query)
    {
        IEnumerable<PaymentWebhookEventModel> results = await mediator.Send(query);
        return Ok(results);
    }

    [HttpPost("reprocess")]
    [EndpointSummary("Reprocessar Evento de Pagamento")]
    [EndpointDescription("Reprocessa um evento de pagamento específico.")]
    public async Task<IActionResult> Reprocess([FromBody] ReprocessPaymentEventCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

}