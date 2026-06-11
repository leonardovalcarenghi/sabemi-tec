using Hangfire;
using MediatR;
using Sabemi.Application.BackgroundJobs;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Exceptions;
using Sabemi.Domain.Interfaces.Repositories;

namespace Sabemi.Application.Features.Webhooks.ReprocessPayment;

internal class ReprocessPaymentEventCommandHandler(IPaymentWebhookEventRepository repository, IBackgroundJobClient jobClient) : IRequestHandler<ReprocessPaymentEventCommand>
{
    public async Task Handle(ReprocessPaymentEventCommand request, CancellationToken cancellationToken)
    {
        PaymentWebhookEvent? paymentEvent = await repository.FindByTransactionAsync(request.TransactionId, cancellationToken)
            ?? throw new NotFoundException($"Evento de webhook para a transação {request.TransactionId} não encontrado.");

        if (paymentEvent.Status is not Domain.Enums.WebhookEventStatus.Failed)
            throw new InvalidOperationException($"Evento de webhook para a transação {request.TransactionId} não está em estado 'Failed' e não pode ser reprocessado.");

        paymentEvent.MarkAsPending();
        await repository.UpdateStatusAsync(paymentEvent, cancellationToken);

        jobClient.Enqueue<ProcessPaymentWebhookEventJob>(job => job.ExecuteAsync(request.TransactionId, cancellationToken));
    }
}

