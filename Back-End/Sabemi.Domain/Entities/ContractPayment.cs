using Sabemi.Domain.Interfaces;
namespace Sabemi.Domain.Entities;

public class ContractPayment : IEntity
{
    public Guid Id { get; init; }

    public Guid ContractId { get; set; }

    public Guid TransactionId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Contract? Contract { get; set; }
}
