using Microsoft.EntityFrameworkCore;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Enums;
using Sabemi.Domain.Interfaces.Repositories;
using Sabemi.Infra.Persistence.Contexts;
namespace Sabemi.Infra.Persistence.Repositories;

internal class PaymentWebhookEventRepository(ApplicationDbContext dbContext) : Repository<PaymentWebhookEvent>(dbContext), IPaymentWebhookEventRepository
{
    public Task<bool> ExistsAsync(Guid transactionId, CancellationToken cancellationToken)
        => _context.PaymentWebhookEvents.AnyAsync(e => e.TransactionId == transactionId, cancellationToken);

    public Task<PaymentWebhookEvent?> FindByTransactionAsync(Guid transactionId, CancellationToken cancellationToken)
        => _context.PaymentWebhookEvents.FirstOrDefaultAsync(_ => _.TransactionId == transactionId, cancellationToken);

    public async Task UpdateStatusAsync(PaymentWebhookEvent entity, CancellationToken cancellationToken = default)
    {
        await _context.PaymentWebhookEvents
            .Where(_ => _.Id == entity.Id)
            .ExecuteUpdateAsync(
            a => a
                .SetProperty(_ => _.Status, entity.Status)
                .SetProperty(_ => _.ErrorMessage, entity.ErrorMessage)
                .SetProperty(_ => _.RetryCount, entity.RetryCount)
                .SetProperty(_ => _.ProcessedAt, entity.ProcessedAt)
        , cancellationToken);
    }

}
