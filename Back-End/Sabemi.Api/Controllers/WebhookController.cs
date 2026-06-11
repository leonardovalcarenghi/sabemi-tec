using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sabemi.Application.Abstractions;
using Sabemi.Application.Features.Webhooks;
using Sabemi.Application.Features.Webhooks.FindPayments;
using Sabemi.Application.Features.Webhooks.ReceivePayment;

namespace Sabemi.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("webhooks")]
[Tags("Webhooks")]
public class WebhookController(IWebhookSecurityService securityService, IMediator mediator) : ControllerBase
{
    [HttpPost("payment")]
    [EndpointSummary("Receber Pagamento")]
    [EndpointDescription("Recebe notificações de pagamento via webhook.")]
    public async Task<IActionResult> ReceivePayment([FromBody] ReceivePaymentWebhookCommand command, [FromHeader(Name = "ApiKey")] string apiKey, [FromHeader(Name = "ApiSecret")] string apiSecret)
    {
        if (!securityService.IsValidSecret(apiKey, apiSecret))
        {
            return Unauthorized();
        }

        await mediator.Send(command);
        return Ok();
    }

    [HttpGet("payments/list")]
    public async Task<IActionResult> FindPaymentEvents([FromQuery] FindPaymentEventCommand query)
    {
        IEnumerable<PaymentWebhookEventModel> results = await mediator.Send(query);
        return Ok(results);
    }
}
