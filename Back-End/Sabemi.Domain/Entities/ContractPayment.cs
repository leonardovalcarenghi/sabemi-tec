using Sabemi.Domain.Interfaces;
namespace Sabemi.Domain.Entities;

public class ContractPayment : IEntity
{
    public ContractPayment()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public ContractPayment(Guid transactionId, Guid contractId, decimal amount, DateTime paidAt):this()
    {
        TransactionId = transactionId;
        ContractId = contractId;
        Amount = amount;
        PaidAt = paidAt;
    }

    public Guid Id { get; init; }

    public Guid ContractId { get; private set; }

    public Guid TransactionId { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime PaidAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public virtual Contract? Contract { get; private set; }
}
