using Sabemi.Domain.Interfaces;
namespace Sabemi.Domain.Entities;

public class Contract : IEntity
{
    public Contract()
    {
        Id = Guid.NewGuid();
        UpdatedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public Contract(string name, decimal amount) : this()
    {
        Name = name;
        TotalAmount = amount;
    }

    public Guid Id { get; init; }

    public required string Name { get; set; }

    public required decimal TotalAmount { get; set; }

    public required decimal PaidAmount { get; set; }

    public decimal PendingAmount => TotalAmount - PaidAmount;

    public DateTime UpdatedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public virtual ICollection<ContractPayment> Payments { get; set; } = [];

    public void AddPayment(Guid transactionId, decimal amount, DateTime paidAt)
    {
        PaidAmount += amount;
        Payments.Add(new ContractPayment
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            Amount = amount,
            PaidAt = paidAt,
        });
        UpdatedAt = DateTime.UtcNow;
    }
}
