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

    public Task<PaymentWebhookEvent?> FindByTransaction(Guid transactionId, CancellationToken cancellationToken)
        => _context.PaymentWebhookEvents.FirstOrDefaultAsync(_ => _.TransactionId == transactionId, cancellationToken);
}
