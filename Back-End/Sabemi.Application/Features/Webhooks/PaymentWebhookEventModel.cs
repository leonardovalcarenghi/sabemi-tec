using Sabemi.Application.Features.Contracts;
using Sabemi.Domain.Enums;
namespace Sabemi.Application.Features.Webhooks;

public class PaymentWebhookEventModel
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public Guid TransactionId { get; set; }
    public string Payload { get; set; }
    public WebhookEventStatus Status { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public ContractModel? Contract { get; set; }
}
