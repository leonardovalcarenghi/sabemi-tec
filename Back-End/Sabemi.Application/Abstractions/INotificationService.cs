namespace Sabemi.Application.Abstractions;

public interface INotificationService
{
    /// <summary>
    /// Notificar quando um evento de webhook for criado.
    /// </summary>
    Task NotifyEventCreatedAsync(Guid transactionId, CancellationToken cancellationToken);

    /// <summary>
    /// Notificar quando um contrato for criado.
    /// </summary>
    Task NotifyContractCreatedAsync(Guid contractId, CancellationToken cancellationToken);

    /// <summary>
    /// Notificar quando um evento for atualizado.
    /// </summary>
    Task NotifyEventChangedAsync(Guid transactionId, CancellationToken cancellationToken);

    /// <summary>
    /// Notificar quando um contrato for atualizado.
    /// </summary>
    Task NotifyContractChangedAsync(Guid contractId, CancellationToken cancellationToken);
}
