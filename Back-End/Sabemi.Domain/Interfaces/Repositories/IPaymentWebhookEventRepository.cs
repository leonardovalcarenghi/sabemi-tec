using Sabemi.Domain.Entities;

namespace Sabemi.Domain.Interfaces.Repositories;

public interface IPaymentWebhookEventRepository : IRepository<PaymentWebhookEvent>
{
    Task<bool> ExistsAsync(Guid transactionId, CancellationToken cancellationToken);

    Task<PaymentWebhookEvent?> FindByTransaction(Guid transactionId, CancellationToken cancellationToken);
}
