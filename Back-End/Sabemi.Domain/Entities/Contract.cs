using Sabemi.Domain.Enums;
using Sabemi.Domain.Interfaces;
namespace Sabemi.Domain.Entities;

public class Contract : IEntity
{
    public Guid Id { get; init; }

    public string Name { get; private set; }

    public decimal TotalAmount { get; private set; }

    public decimal PaidAmount { get; private set; }

    public decimal PendingAmount => TotalAmount - PaidAmount;

    public DateTime UpdatedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public virtual ContractStatus? Status { get; set; }

    public virtual ICollection<ContractPayment> Payments { get; set; } = [];

    public virtual ICollection<PaymentWebhookEvent> WebhookEvents { get; set; } = [];

    public Contract()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Contract Create(string name, decimal totalAmount)
    {
        return new Contract
        {
            Name = name,
            TotalAmount = totalAmount,
            PaidAmount = 0,
        };
    }

    public ContractPayment AddPayment(Guid transactionId, decimal amount, DateTime paidAt)
    {
        if (amount <= 0)
            throw new InvalidOperationException("O valor do pagamento deve ser maior que zero.");

        PaidAmount += amount;

        if (PaidAmount > 0)
            Status?.SetStatus(EContractStatus.InProgress);

        if (PaidAmount >= TotalAmount)
            Status?.SetStatus(EContractStatus.Completed);

        UpdatedAt = DateTime.UtcNow;
        return new ContractPayment(transactionId, Id, amount, paidAt);
    }
}
