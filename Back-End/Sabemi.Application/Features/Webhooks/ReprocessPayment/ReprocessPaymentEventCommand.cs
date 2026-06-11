using MediatR;
namespace Sabemi.Application.Features.Webhooks.ReprocessPayment;

public class ReprocessPaymentEventCommand : IRequest
{
    public Guid TransactionId { get; set; }
}
