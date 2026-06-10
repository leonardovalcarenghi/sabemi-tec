using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sabemi.Application.Abstractions;
using Sabemi.Application.Features.Webhooks.ReceivePaymentWebhook;
namespace Sabemi.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("webhooks")]
public class WebhookController(IWebhookSecurityService securityService, IMediator mediator) : ControllerBase
{
    [HttpPost("payment")]
    public async Task<IActionResult> ReceivePayment([FromBody] ReceivePaymentWebhookCommand command, [FromHeader(Name = "ApiKey")] string apiKey, [FromHeader(Name = "ApiSecret")] string apiSecret)
    {
        if (!securityService.IsValidSecret(apiKey, apiSecret))
        {
            return Unauthorized();
        }

        await mediator.Send(command);
        return Ok();
    }
}
