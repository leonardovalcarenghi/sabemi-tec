using Hangfire;
using MediatR;
using Sabemi.Application.BackgroundJobs;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
namespace Sabemi.Application.Features.Webhooks.ReceivePaymentWebhook;

internal class ReceivePaymentWebhookCommandHandler(
    IPaymentWebhookEventRepository repository,
    IBackgroundJobClient jobClient
) : IRequestHandler<ReceivePaymentWebhookCommand>
{
    public async Task Handle(ReceivePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        if (await repository.ExistsAsync(request.TransactionId, cancellationToken))
        {
            // logar: "Evento de webhook de pagamento recebido para transação {TransactionId} já processada. Ignorando."
            return;
        }

        // Salvar o evento de webhook para processamento assíncrono posterior:
        string payload = request.ToJson();
        PaymentWebhookEvent webhookEvent = PaymentWebhookEvent.Create(request.TransactionId, payload);
        await repository.AddAsync(webhookEvent, cancellationToken);

        // Enfileirar um job para processar o evento de webhook:
        jobClient.Enqueue<ProcessPaymentWebhookEventJob>(job => job.ExecuteAsync(request.TransactionId, cancellationToken));
    }
}
