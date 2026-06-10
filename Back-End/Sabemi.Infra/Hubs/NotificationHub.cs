using Microsoft.AspNetCore.SignalR;

namespace Sabemi.Infra.Hubs;

public class NotificationHub : Hub
{
    /// <summary>
    /// Evento disparado quando um contrato é atualizado.
    /// </summary>
    public const string EVENT_CONTRACT_UPDATED = "ContractUpdated";
}
