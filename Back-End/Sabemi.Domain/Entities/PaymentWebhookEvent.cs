using Sabemi.Domain.Enums;
using Sabemi.Domain.Interfaces;
namespace Sabemi.Domain.Entities;

public class PaymentWebhookEvent : IEntity
{
    public Guid Id { get; init; }
    public Guid ContractId { get; private set; }
    public Guid TransactionId { get; private set; }
    public string Payload { get; private set; }
    public WebhookEventStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public virtual Contract? Contract { get; set; }

    public PaymentWebhookEvent()
    {
        Id = Guid.NewGuid();
    }

    public static PaymentWebhookEvent Create(Guid contractId, Guid transactionId, string payload)
    {
        return new PaymentWebhookEvent
        {
            ContractId = contractId,
            TransactionId = transactionId,
            Payload = payload,
            Status = WebhookEventStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessed()
    {
        Status = WebhookEventStatus.Processed;
        ErrorMessage = null;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessing()
    {
        Status = WebhookEventStatus.Processing;
    }

    public void MarkAsFailed(string error)
    {
        Status = WebhookEventStatus.Failed;
        ErrorMessage = error;
        RetryCount++;
    }

}
