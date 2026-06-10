using Microsoft.AspNetCore.SignalR;
using Sabemi.Application.Abstractions;
using Sabemi.Infra.Hubs;
namespace Sabemi.Infra.Services;

internal class NotificationService(IHubContext<NotificationHub> hub) : INotificationService
{

    // To do: ajustar métodos e nomenclaturas. (esta muito verboso)
    public async Task NotifyErrorOnPaymentWebhookProcessingAsync(Guid transactionId, string errorMessage, CancellationToken cancellationToken)
    {
        await hub.Clients.All.SendAsync(NotificationHub.EVENT_CONTRACT_UPDATED, new { transactionId }, cancellationToken);
    }

    public async Task NotifyPaymentWebhookChangedAsync(Guid contractId, CancellationToken cancellationToken)
    {
        await hub.Clients.All.SendAsync(NotificationHub.EVENT_CONTRACT_UPDATED, new { contractId }, cancellationToken);
    }
}
