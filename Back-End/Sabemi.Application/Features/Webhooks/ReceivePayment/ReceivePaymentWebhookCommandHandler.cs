using Hangfire;
using MediatR;
using Sabemi.Application.Abstractions;
using Sabemi.Application.BackgroundJobs;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
using System.Text.Json;
namespace Sabemi.Application.Features.Webhooks.ReceivePayment;

internal class ReceivePaymentWebhookCommandHandler(
    IPaymentWebhookEventRepository repository,
    IBackgroundJobClient jobClient,
    IUnitOfWork unitOfWork,
    INotificationService notificationService
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
        string payload = JsonSerializer.Serialize(request);
        PaymentWebhookEvent webhookEvent = PaymentWebhookEvent.Create(request.ContractId, request.TransactionId, payload);
        repository.Add(webhookEvent);

        if (!await unitOfWork.SaveChangesAsync(cancellationToken))
            throw new InvalidOperationException("Ocorreu um erro ao salvar o evento de webhook de pagamento. Tente novamente.");

        await notificationService.NotifyEventCreatedAsync(webhookEvent.Id, cancellationToken);

        // Enfileirar um job para processar o evento de webhook:
        jobClient.Enqueue<ProcessPaymentWebhookEventJob>(job => job.ExecuteAsync(request.TransactionId, cancellationToken));
    }
}
