using Microsoft.AspNetCore.SignalR;
using Sabemi.Application.Abstractions;
using Sabemi.Infra.Hubs;
namespace Sabemi.Infra.Services;

internal class NotificationService(IHubContext<NotificationHub> hub) : INotificationService
{
    public async Task NotifyEventCreatedAsync(Guid transactionId, CancellationToken cancellationToken)
        => await hub.Clients.All.SendAsync($"event-created", new { transactionId }, cancellationToken);

    public async Task NotifyContractCreatedAsync(Guid contractId, CancellationToken cancellationToken)
        => await hub.Clients.All.SendAsync($"contract-created", new { contractId }, cancellationToken);


    public async Task NotifyEventChangedAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        await hub.Clients.All.SendAsync($"event-changed", cancellationToken);
        await hub.Clients.All.SendAsync($"event-changed-#{transactionId}", cancellationToken);
    }

    public async Task NotifyContractChangedAsync(Guid contractId, CancellationToken cancellationToken)
    {
        await hub.Clients.All.SendAsync($"contract-changed", cancellationToken);
        await hub.Clients.All.SendAsync($"contract-changed-#{contractId}", cancellationToken);
    }


}
