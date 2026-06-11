using MediatR;
namespace Sabemi.Application.Features.Webhooks.ProcessPayment;

public class ProcessPaymentWebhookCommand(Guid transactionId) : IRequest
{
    public Guid TransactionId { get; set; } = transactionId;
}
