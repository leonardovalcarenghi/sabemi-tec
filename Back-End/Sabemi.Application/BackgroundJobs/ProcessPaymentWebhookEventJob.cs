using Hangfire;
using MediatR;
using Sabemi.Application.Features.Webhooks.ProcessPayment;
namespace Sabemi.Application.BackgroundJobs;

public class ProcessPaymentWebhookEventJob(IMediator mediator)
{
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(Guid transactionId, CancellationToken cancellationToken)
        => await mediator.Send(new ProcessPaymentWebhookCommand(transactionId), cancellationToken);
}