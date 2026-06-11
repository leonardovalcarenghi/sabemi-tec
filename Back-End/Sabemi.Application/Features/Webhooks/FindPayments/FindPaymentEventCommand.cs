using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sabemi.Domain.Enums;
namespace Sabemi.Application.Features.Webhooks.FindPayments;

[BindProperties]
public class FindPaymentEventCommand : IRequest<IEnumerable<PaymentWebhookEventModel>>
{
    public Guid? ContractId { get; set; }
    public WebhookEventStatus? Status { get; set; }
}
