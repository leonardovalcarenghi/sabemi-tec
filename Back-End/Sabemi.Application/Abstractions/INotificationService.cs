namespace Sabemi.Application.Abstractions;

public interface INotificationService
{
    Task NotifyPaymentWebhookChangedAsync(Guid transactionId, CancellationToken cancellationToken);


    Task NotifyErrorOnPaymentWebhookProcessingAsync(Guid transactionId, string errorMessage, CancellationToken cancellationToken);
}
